namespace AutoBackup.Utilities;

public static class StringExtensions
{
    public static string ReplaceSeparators(this string str) => str?.Replace("\\", "/");
}
