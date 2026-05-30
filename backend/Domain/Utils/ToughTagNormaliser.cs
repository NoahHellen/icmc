using System.Linq;

namespace Domain.Utils;

/// <summary>
/// Normalises tough tag strings to avoid duplicate entries.
/// </summary>
public static class ToughTagNormaliser
{
    /// <summary>
    /// Performs the purpose of the class.
    /// </summary>
    /// <param name="toughTag"></param>
    /// <returns></returns>
    public static string? Normalise(string? toughTag)
    {
        if (string.IsNullOrWhiteSpace(toughTag))
        {
            return null;
        }

        return new string(toughTag.Where(c => !char.IsWhiteSpace(c)).ToArray()).ToLowerInvariant();
    }
}
