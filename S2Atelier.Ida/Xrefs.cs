using S2Atelier.Ida.Generated;

namespace S2Atelier.Ida;

public static class Xrefs
{
    private const ulong BadAddr = ulong.MaxValue;

    public static IEnumerable<ulong> DataTo(ulong ea)
    {
        ulong x = IdaNative.get_first_dref_to(ea);
        while (x != BadAddr)
        {
            yield return x;
            x = IdaNative.get_next_dref_to(ea, x);
        }
    }

    public static IEnumerable<ulong> CodeTo(ulong ea)
    {
        ulong x = IdaNative.get_first_cref_to(ea);
        while (x != BadAddr)
        {
            yield return x;
            x = IdaNative.get_next_cref_to(ea, x);
        }
    }

    public static IEnumerable<ulong> AllTo(ulong ea) => CodeTo(ea).Concat(DataTo(ea));
}
