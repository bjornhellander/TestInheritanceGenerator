using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;

namespace TestInheritanceGenerator
{
    internal class Helpers
    {
        private static readonly Regex AssemblyNameRegex = new("^(.*?)(\\d+(_\\d+)*)$");

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

        public static bool TryGetBaseAssembly(
            Compilation compilation,
            string assemblyPrefix,
            [NotNullWhen(returnValue: true)] out IAssemblySymbol? baseAssembly,
            [NotNullWhen(returnValue: true)] out string? baseAssemblyVersion)
        {
            (IAssemblySymbol Assembly, Version Version, string StrVersion)? bestMatch = null;
            var referencedAssemblies = compilation.Assembly.Modules.First().ReferencedAssemblySymbols;
            foreach (var referencedAssembly in referencedAssemblies)
            {
                if (!Helpers.TryGetAssemblyNamePrefixAndVersion(referencedAssembly.Name, out var referencedAssemblyPrefix, out var referencedAssemblyStrVersion))
                {
                    // Not a relevant reference
                    continue;
                }

                if (referencedAssemblyPrefix != assemblyPrefix)
                {
                    // Not a relevant reference
                    continue;
                }

                var referencedAssemblyVersion = ParseVersion(referencedAssemblyStrVersion);
                if (bestMatch != null && referencedAssemblyVersion < bestMatch.Value.Version)
                {
                    // We have already seen a reference to a later test project
                    continue;
                }

                bestMatch = (referencedAssembly, referencedAssemblyVersion, referencedAssemblyStrVersion);
            }

            baseAssembly = bestMatch?.Assembly;
            baseAssemblyVersion = bestMatch?.StrVersion;
            return bestMatch != null;
        }

        private static Version ParseVersion(string referencedAssemblyStrVersion)
        {
            if (referencedAssemblyStrVersion.All(char.IsDigit))
            {
                return new Version(int.Parse(referencedAssemblyStrVersion), 0);
            }
            else
            {
                return new Version(referencedAssemblyStrVersion.Replace('_', '.'));
            }
        }

        public static ImmutableArray<TestTypeData> GetTestTypes(IAssemblySymbol assembly)
        {
            var result = new List<TestTypeData>();
            var collector = new TestClassCollector(result);
            collector.Visit(assembly);
            return result.ToImmutableArray();
        }
    }
}
