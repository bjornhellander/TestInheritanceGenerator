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

                var types = Helpers.GetTestTypes(baseAssembly);

                return new AssemblyData(assemblyPrefix, assemblyVersion, baseAssemblyVersion, types);
            });

            context.RegisterSourceOutput(assemblyData, Execute);
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
