namespace KubernetesVolume.Addon.Extensions
{
    using System.Security;

    internal static class StringExtensions
    {
        internal static SecureString ToSecureString(this string source)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return null;
            }

            var result = new SecureString();

            foreach (var c in source.ToCharArray())
            {
                result.AppendChar(c);
            }

            return result;
        }
    }
}
