// Copyright © Björn Hellander 2024
// Licensed under the MIT License. See LICENSE.txt in the repository root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace TestInheritanceGenerator
{
    [Generator]
    public partial class InheritanceGenerator : IIncrementalGenerator
    {
        private static readonly Regex AssemblyNameRegex = new("(.*)(\\d+(_\\d+)*)$");

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var assemblyData = context.CompilationProvider.Select((compilation, cancellationToken) =>
            {
                // System.Diagnostics.Debugger.Launch();
                if (!TryGetPrefixAndVersion(compilation, out var assemblyPrefix, out var assemblyVersion))
                {
                    return null;
                }

                if (!TryGetBaseAssembly(compilation, assemblyPrefix, out var baseAssembly, out var baseAssemblyVersion))
                {
                    return null;
                }

                var types = GetTestTypes(baseAssembly);

                return new AssemblyData(assemblyPrefix, assemblyVersion, baseAssemblyVersion, types);
            });

            context.RegisterSourceOutput(assemblyData, Execute);
        }

        private static bool TryGetPrefixAndVersion(
            Compilation compilation,
            [NotNullWhen(returnValue: true)] out string? assemblyPrefix,
            [NotNullWhen(returnValue: true)] out string? assemblyVersion)
        {
            var currentAssemblyName = compilation.AssemblyName ?? string.Empty;
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

        private static bool TryGetBaseAssembly(
            Compilation compilation,
            string assemblyPrefix,
            [NotNullWhen(returnValue: true)] out IAssemblySymbol? baseAssembly,
            [NotNullWhen(returnValue: true)] out string? baseAssemblyVersion)
        {
            (IAssemblySymbol Assembly, Version Version, string StrVersion)? bestMatch = null;
            foreach (var referencedAssembly in compilation.Assembly.Modules.First().ReferencedAssemblySymbols)
            {
                var match = AssemblyNameRegex.Match(referencedAssembly.Name);
                if (!match.Success)
                {
                    // Not a relevant reference
                    continue;
                }

                var referencedAssemblyPrefix = match.Groups[1].Value;
                if (referencedAssemblyPrefix != assemblyPrefix)
                {
                    // Not a relevant reference
                    continue;
                }

                var referencedAssemblyStrVersion = match.Groups[2].Value;
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

        private static ImmutableArray<TestTypeData> GetTestTypes(IAssemblySymbol assembly)
        {
            // System.Diagnostics.Debugger.Launch();
            var result = new List<TestTypeData>();
            var collector = new TestClassCollector(result);
            collector.Visit(assembly);
            return result.ToImmutableArray();
        }

        private static void Execute(SourceProductionContext context, AssemblyData? data)
        {
            if (data == null)
            {
                return;
            }

            foreach (var type in data.TestTypes)
            {
                var source = $@"
namespace {type.Namespace.Replace(data.BaseAssemblyVersion, data.Version)}
{{
    [{type.AttributeName}]
    public partial class {type.Name} : {type.Namespace}.{type.Name}
    {{
    }}
}}
    ";
                context.AddSource($"{type.Name}.g.cs", SourceText.From(source, System.Text.Encoding.UTF8));
            }
        }
    }
}
