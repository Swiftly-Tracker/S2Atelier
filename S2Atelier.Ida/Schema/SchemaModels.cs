namespace S2Atelier.Ida.Schema;

public enum SchemaFieldKind
{
    Reference,
    Atomic,
    FixedArray,
    Pointer,
    Bitfield,
}

public sealed record SchemaTemplateArgument(string Text, bool IsLiteral);

public sealed record SchemaField(
    string Name,
    ulong NameHash,
    SchemaFieldKind Kind,
    string TypeName,
    int Offset,
    int Size,
    int Alignment,
    bool Networked,
    string? TemplatedType = null,
    IReadOnlyList<SchemaTemplateArgument>? TemplateArguments = null,
    int ElementCount = 0,
    int ElementSize = 0,
    int ElementAlignment = 0,
    int BitCount = 0);

public sealed record SchemaClass(
    string Name,
    ulong NameHash,
    string Project,
    int Size,
    int Alignment,
    bool IsStruct,
    bool HasChainer,
    IReadOnlyList<string> BaseClasses,
    IReadOnlyList<SchemaField> Fields,
    bool Synthetic = false);

public sealed record SchemaEnumValue(string Name, long SignedValue, ulong UnsignedValue, bool IsUnsigned);

public sealed record SchemaEnum(
    string Name,
    ulong NameHash,
    string Project,
    int Size,
    int Alignment,
    IReadOnlyList<SchemaEnumValue> Fields);

public sealed record SchemaSelection(
    string Project,
    IReadOnlyDictionary<string, SchemaClass> Classes,
    IReadOnlyDictionary<string, SchemaEnum> Enums,
    IReadOnlySet<string> RootClassNames,
    IReadOnlySet<string> RootEnumNames,
    IReadOnlySet<string> SyntheticClassNames)
{
    public int TypeCount => Classes.Values.Count(x => !x.Synthetic) + Enums.Count;
}

public sealed class SchemaFormatException(string message) : Exception(message);
