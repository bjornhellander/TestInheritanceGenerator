using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace TestInheritanceGenerator
{
    internal class Helpers
    {
        private static readonly Regex AssemblyNameRegex = new("(.*)(\\d+(_\\d+)*)$");

        public static bool TryGetAssemblyNamePrefixAndVersion(
            string? assemblyName,
            [NotNullWhen(returnValue: true)] out string? assemblyPrefix,
            [NotNullWhen(returnValue: true)] out string? assemblyVersion)
        {
            var currentAssemblyName = assemblyName ?? string.Empty;
            var match = AssemblyNameRegex.Match(currentAssemblyName);
            if (!match.Success)
            {
                assemblyPrefix = null;
                assemblyVersion = null;
                return false;
            }

            assemblyPrefix = match.Groups[1].Value;
            assemblyVersion = match.Groups[2].Value;
            return true;
        }
    }
}
