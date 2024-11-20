using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace TestInheritanceGenerator
{
    [Generator]
    public class InheritanceGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var assemblyData = context.CompilationProvider.Select((compilation, cancellationToken) =>
            {
                // System.Diagnostics.Debugger.Launch();
                if (!Helpers.TryGetAssemblyNamePrefixAndVersion(compilation.AssemblyName, out var assemblyPrefix, out var assemblyVersion))
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

        private static bool TryGetBaseAssembly(
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
    {(type.AttributeName != null ? $"[{type.AttributeName}]" : "")}
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
