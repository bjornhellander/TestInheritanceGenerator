﻿using System.Collections.Generic;
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
                if (!Helpers.TryGetAssemblyNamePrefixAndVersion(compilation.AssemblyName, out var assemblyPrefix, out var assemblyVersion))
                {
                    return null;
                }

                if (!Helpers.TryGetBaseAssembly(compilation, assemblyPrefix, out var baseAssembly, out var baseAssemblyVersion))
                {
                    return null;
                }

                var baseTypes = Helpers.GetTestTypes(baseAssembly);
                var currTypes = Helpers.GetTestTypes(compilation.Assembly); ;

                return new AssemblyData(assemblyPrefix, assemblyVersion, baseAssemblyVersion, baseTypes, currTypes);
            });

            context.RegisterSourceOutput(assemblyData, Execute);
        }

        private static void Execute(SourceProductionContext context, AssemblyData? data)
        {
            if (data == null)
            {
                return;
            }

            var currTestTypeNames = new HashSet<(string Namespace, string Name)>();
            foreach (var currTestType in data.CurrTestTypes)
            {
                currTestTypeNames.Add((currTestType.Namespace, currTestType.Name));
            }

            foreach (var baseTestType in data.BaseTestTypes)
            {
                var currNamespace = baseTestType.Namespace.Replace(data.BaseAssemblyVersion, data.Version);
                var addAttribute = baseTestType.AttributeName != null && !currTestTypeNames.Contains((currNamespace, baseTestType.Name));

                var source = $@"
namespace {currNamespace}
{{
    {(addAttribute ? $"[{baseTestType.AttributeName}]" : "")}
    public partial class {baseTestType.Name} : {baseTestType.Namespace}.{baseTestType.Name}
    {{
    }}
}}
";
                context.AddSource($"{baseTestType.Name}.g.cs", SourceText.From(source, System.Text.Encoding.UTF8));
            }
        }
    }
}
