namespace UNIOOP.App.Helpers
{
    public static class InputNormalizationHelper
    {
        public static string NormalizeText(string input)
        {
            return input.Trim();
        }

        public static string NormalizeEmail(string input)
        {
            return input.Trim().ToLowerInvariant();
        }

        public static string NormalizeSsn(string input)
        {
            return NormalizeText(input);
        }
    }
}