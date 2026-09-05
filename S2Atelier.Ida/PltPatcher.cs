using System.Text;
using S2Atelier.Ida.Generated;

namespace S2Atelier.Ida;

public sealed record PltPatchResult(bool Applicable, int Patched, int Unresolved);

public static unsafe class PltPatcher
{

    private const int DtPltRelSz = 2;
    private const int DtStrtab = 5;
    private const int DtSymtab = 6;
    private const int DtJmpRel = 0x17;
    private const uint RX8664JumpSlot = 7;

    private const int SnForce = 0x800;
    private const int DrO = 1;
    private const int FlCn = 17;
    private const uint TinfoDefinite = 0x0001;
    private const ulong FuncThunk = 0x00000080;
    private const ulong BadAddr = ulong.MaxValue;

    private const int FuncFlagsOffset = 16;

    public static PltPatchResult Run()
    {
        if (!IsElf64())
        {
            return new PltPatchResult(false, 0, 0);
        }

        ulong imageBase = FirstSegmentStart();
        if (imageBase == BadAddr || !TryFindDynamicSection(imageBase, out ulong dyn, out ulong dynSize))
        {
            return new PltPatchResult(true, 0, 0);
        }

        ulong? jmprel = FindDynamicEntry(dyn, dynSize, DtJmpRel);
        ulong? strtab = FindDynamicEntry(dyn, dynSize, DtStrtab);
        ulong? symtab = FindDynamicEntry(dyn, dynSize, DtSymtab);
        ulong? relszEntry = FindDynamicEntry(dyn, dynSize, DtPltRelSz);

        if (jmprel is null || strtab is null || symtab is null)
        {
            return new PltPatchResult(true, 0, 0);
        }

        ulong relsz = relszEntry ?? SegmentEndContaining(jmprel.Value) - jmprel.Value;

        int patched = 0;
        int unresolved = 0;

        for (ulong i = 0; i < relsz; i += 24)
        {
            ulong gotPltOffset = IdaNative.get_qword(jmprel.Value + i);
            uint relType = IdaNative.get_dword(jmprel.Value + i + 8);
            uint symIndex = IdaNative.get_dword(jmprel.Value + i + 0xC);

            if (relType != RX8664JumpSlot)
            {
                continue;
            }

            uint nameOffset = IdaNative.get_dword(symtab.Value + symIndex * 0x18);
            string funcName = ReadCString(strtab.Value + nameOffset);

            if (funcName.Length == 0)
            {
                continue;
            }

            ulong target = ResolveExisting(funcName);

            SetNameForce(gotPltOffset, funcName + "_ptr");

            if (target == BadAddr)
            {
                unresolved++;
                continue;
            }

            IdaNative.put_qword(gotPltOffset, target);
            IdaNative.add_dref(gotPltOffset, target, DrO);

            var tif = new TypeInfo();

            ulong from = IdaNative.get_first_dref_to(gotPltOffset);
            while (from != BadAddr)
            {
                IdaNative.add_cref(from, target, FlCn);

                void* refFunc = IdaNative.get_func(from);
                if (refFunc != null)
                {
                    ulong start = *(ulong*)refFunc;
                    SetNameForce(start, "_" + funcName);
                    MarkThunk(refFunc);
                }

                if (IdaNative.get_tinfo(&tif, from) != 0)
                {
                    IdaNative.apply_tinfo(target, &tif, TinfoDefinite);
                }

                from = IdaNative.get_next_dref_to(gotPltOffset, from);
            }

            tif.Dispose();
            patched++;
        }

        return new PltPatchResult(true, patched, unresolved);
    }

    private static bool IsElf64()
    {
        byte* buf = stackalloc byte[64];
        nuint len = IdaNative.get_file_type_name(buf, 64);
        if (len == 0)
        {
            return false;
        }

        string typeName = Encoding.UTF8.GetString(buf, (int)len);
        return typeName.Contains("ELF64", StringComparison.Ordinal);
    }

    private static ulong FirstSegmentStart()
    {
        void* seg = IdaNative.get_first_seg();
        return seg == null ? BadAddr : *(ulong*)seg;
    }

    private static ulong SegmentEndContaining(ulong ea)
    {
        void* seg = IdaNative.getseg(ea);
        return seg == null ? ea : *((ulong*)seg + 1);
    }

    private static bool TryFindDynamicSection(ulong imageBase, out ulong dynAddr, out ulong dynSize)
    {
        dynAddr = 0;
        dynSize = 0;

        ulong phoff = IdaNative.get_qword(imageBase + 0x20) + imageBase;
        ushort phentsize = IdaNative.get_word(imageBase + 0x36);
        ushort phnum = IdaNative.get_word(imageBase + 0x38);

        for (ushort i = 0; i < phnum; i++)
        {
            ulong entry = phoff + (ulong)phentsize * i;
            uint pType = IdaNative.get_dword(entry);

            if (pType == 2)
            {
                dynAddr = IdaNative.get_qword(entry + 0x10);
                dynSize = IdaNative.get_qword(entry + 0x28);
                return true;
            }
        }

        return false;
    }

    private static ulong? FindDynamicEntry(ulong dyn, ulong dynSize, long tag)
    {
        for (ulong i = 0; i < dynSize; i += 16)
        {
            long entryTag = (long)IdaNative.get_qword(dyn + i);
            ulong value = IdaNative.get_qword(dyn + i + 8);

            if (entryTag == 0 && value == 0)
            {
                break;
            }

            if (entryTag == tag)
            {
                return value;
            }
        }

        return null;
    }

    private static string ReadCString(ulong ea)
    {
        var bytes = new List<byte>();

        for (ulong i = 0; i < 4096; i++)
        {
            byte b = IdaNative.get_byte(ea + i);
            if (b == 0)
            {
                break;
            }

            bytes.Add(b);
        }

        return Encoding.UTF8.GetString([.. bytes]);
    }

    private static ulong ResolveExisting(string name)
    {
        byte* namePtr = Utf8.Allocate(name);
        try
        {
            ulong direct = IdaNative.get_name_ea(BadAddr, namePtr);
            if (direct != BadAddr)
            {
                return direct;
            }
        }
        finally
        {
            Utf8.Free(namePtr);
        }

        byte* externName = Utf8.Allocate("extern");
        void* seg;
        try
        {
            seg = IdaNative.get_segm_by_name(externName);
        }
        finally
        {
            Utf8.Free(externName);
        }

        if (seg == null)
        {
            return BadAddr;
        }

        ulong segStart = *(ulong*)seg;
        ulong segEnd = *((ulong*)seg + 1);

        nuint qty = IdaNative.get_func_qty();
        for (nuint i = 0; i < qty; i++)
        {
            void* pfn = IdaNative.getn_func(i);
            if (pfn == null)
            {
                continue;
            }

            ulong start = *(ulong*)pfn;
            if (start < segStart || start >= segEnd)
            {
                continue;
            }

            var qs = new QString();
            nint len = IdaNative.get_func_name(&qs, start);
            string candidate = len > 0 ? qs.Read() : string.Empty;
            qs.Dispose();

            if (candidate == name)
            {
                return start;
            }
        }

        return BadAddr;
    }

    private static void SetNameForce(ulong ea, string name)
    {
        byte* namePtr = Utf8.Allocate(name);
        try
        {
            IdaNative.set_name(ea, namePtr, SnForce);
        }
        finally
        {
            Utf8.Free(namePtr);
        }
    }

    private static void MarkThunk(void* pfn)
    {
        ulong* flags = (ulong*)((byte*)pfn + FuncFlagsOffset);
        *flags |= FuncThunk;
        IdaNative.update_func(pfn);
    }
}
