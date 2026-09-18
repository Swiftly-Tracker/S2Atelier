using System.Runtime.InteropServices;
using S2Atelier.Ida.Generated;

namespace S2Atelier.Ida;

/// <summary>
/// Gives every imported template instantiation a plain alias, typedef CConVar&lt;float&gt; CConVar_float_:
/// the legacy parser, which the databases keep for the GUI, cannot read a type name with template
/// arguments, so an instantiation could otherwise only be applied by a pass. The alias is built as a
/// tinfo_t that refers to the template by name, so creating it does not go through a parser either.
/// </summary>
public static unsafe class TemplateAliases
{
    private const byte BtfTypedef = 0x3d;
    private const int NtfType = 0x0001;

    public static int Run()
    {
        void* til = IdaNative.get_idati();
        var names = new HashSet<string>(StringComparer.Ordinal);
        var templates = new List<string>();
        for (uint ordinal = 1, limit = IdaNative.get_ordinal_limit(til); ordinal < limit; ordinal++)
        {
            byte* native = IdaNative.get_numbered_type_name(til, ordinal);
            if (native == null)
            {
                continue;
            }

            string name = Marshal.PtrToStringUTF8((nint)native) ?? string.Empty;
            names.Add(name);
            if (IsTemplate(name))
            {
                templates.Add(name);
            }
        }

        int created = 0;
        foreach (string template in templates)
        {
            string alias = Alias(template);
            // A previous run's alias, or a type that already has this name.
            if (names.Add(alias) && Create(til, template, alias))
            {
                created++;
            }
        }

        return created;
    }

    // Instantiations the user can refer to: not the CRT's or the standard library's internals, and not
    // the anonymous types nested in them.
    private static bool IsTemplate(string name)
        => name.Contains('<') && !name.StartsWith('_') && !name.StartsWith("std::", StringComparison.Ordinal) &&
           !name.Contains("::$", StringComparison.Ordinal);

    /// <summary>CConVar&lt;float&gt; becomes CConVar_float_: every character an identifier cannot hold is an underscore.</summary>
    public static string Alias(string template)
        => string.Create(template.Length, template, static (alias, name) =>
        {
            for (int i = 0; i < name.Length; i++)
            {
                alias[i] = char.IsAsciiLetterOrDigit(name[i]) || name[i] == '_' ? name[i] : '_';
            }
        });

    private static bool Create(void* til, string template, string alias)
    {
        byte* target = Utf8.Allocate(template);
        byte* name = Utf8.Allocate(alias);
        TypeInfo type = default;
        try
        {
            var data = new TypedefData { Til = til, Name = target };
            // tinfo_t::create_typedef: the second type byte asks for an ordinal reference where one exists.
            return IdaNative.create_tinfo(&type, BtfTypedef, BtfTypedef, &data) != 0 &&
                   IdaNative.save_tinfo(&type, til, 0, name, NtfType) == 0;
        }
        finally
        {
            type.Dispose();
            Utf8.Free(target);
            Utf8.Free(name);
        }
    }

    // IDA SDK typedef_type_data_t.
    private struct TypedefData
    {
        internal void* Til;
        internal byte* Name;
        internal byte IsOrdinal, Resolve;
    }
}
