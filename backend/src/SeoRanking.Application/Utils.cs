namespace SeoRanking.Application;

public static class Utils
{
    public static string ConvertEnumToString<T>(this T eff) where T : Enum
    {
        return Enum.GetName(eff.GetType(), eff) ?? string.Empty;
    }
}