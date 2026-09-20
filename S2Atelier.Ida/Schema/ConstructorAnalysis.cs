namespace S2Atelier.Ida.Schema;

/// <summary>A function that stores one or more primary vtables into offset 0 of its <c>this</c> argument.</summary>
/// <param name="Function">Function start.</param>
/// <param name="Classes">Classes of the stored vtables, in instruction order.</param>
/// <param name="CallBeforeFirstWrite">A call precedes the first vtable store.</param>
/// <param name="FirstCall">Target of the first direct call before the first store, if any.</param>
/// <param name="TableReferenced">A vtable slot or a data pointer references the function itself.</param>
public sealed record VptrWriter(
    ulong Function,
    IReadOnlyList<string> Classes,
    bool CallBeforeFirstWrite,
    ulong? FirstCall,
    bool TableReferenced);

public static class ConstructorAnalysis
{
    /// <summary>
    /// Selects each class's constructor. MSVC constructors call their base constructor, which stores the
    /// base vtable, before storing their own, so the class is the last vtable stored. Destructors store in
    /// the opposite order, before any call, and the ones that do not are reached through a vtable slot, so a
    /// table reference rules a function out. A class with several matches (overloads, or several functions
    /// that construct it inline) is left out rather than guessed.
    /// </summary>
    public static IReadOnlyDictionary<string, ulong> SelectConstructors(IReadOnlyCollection<VptrWriter> writers)
    {
        var byFunction = writers.ToDictionary(x => x.Function);
        return writers
            .Where(x => x.Classes.Count > 0 && !x.TableReferenced && x.CallBeforeFirstWrite &&
                        x.FirstCall is ulong call && byFunction.TryGetValue(call, out VptrWriter? baseWriter) &&
                        !baseWriter.Classes.Contains(x.Classes[^1], StringComparer.Ordinal))
            .GroupBy(x => x.Classes[^1], StringComparer.Ordinal)
            .Where(x => x.Count() == 1)
            .ToDictionary(x => x.Key, x => x.Single().Function, StringComparer.Ordinal);
    }

    /// <summary>The unqualified constructor name: <c>ns::Foo&lt;T&gt;</c> becomes <c>Foo</c>.</summary>
    public static string ConstructorName(string className)
    {
        int depth = 0, start = 0, end = className.Length;
        for (int i = 0; i < className.Length; i++)
        {
            char c = className[i];
            if (c == '<')
            {
                if (depth++ == 0) end = i;
            }
            else if (c == '>') depth--;
            else if (depth == 0 && c == ':' && i + 1 < className.Length && className[i + 1] == ':')
            {
                start = i + 2;
                end = className.Length;
                i++;
            }
        }
        return $"{className}::{className[start..end]}";
    }
}
