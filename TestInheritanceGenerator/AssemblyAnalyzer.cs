using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

// TODO: Check these suppressions
#pragma warning disable RS1004 // Recommend adding language support to diagnostic analyzer
#pragma warning disable RS2008 // Enable analyzer release tracking

namespace TestInheritanceGenerator
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AssemblyAnalyzer : DiagnosticAnalyzer
    {
        public const string IncorrectAssemblyNameDiagnosticId = "TestInh001";
        public const string NoBaseAssemblyDiagnosticId = "TestInh002";
        public const string NoBaseTestsDiagnosticId = "TestInh003";

        private static readonly DiagnosticDescriptor IncorrectAssemblyNameDescriptor = new(
            IncorrectAssemblyNameDiagnosticId,
            "Incorrect assembly name",
            "Incorrect assembly name",
            "SourceGenerator",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "To use TestInheritanceGenerator, the assembly name needs to end with a version number.",
            customTags: WellKnownDiagnosticTags.CompilationEnd);

        private static readonly DiagnosticDescriptor NoBaseAssemblyDescriptor = new(
            NoBaseAssemblyDiagnosticId,
            "No base assembly",
            "No base assembly",
            "SourceGenerator",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "To use TestInheritanceGenerator, an assembly with a matching name needs to be referenced.",
            customTags: WellKnownDiagnosticTags.CompilationEnd);

        private static readonly DiagnosticDescriptor NoBaseTestsDescriptor = new(
            NoBaseTestsDiagnosticId,
            "No tests in base assembly",
            "No tests in base assembly",
            "SourceGenerator",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "No test classes where found in the base assembly.",
            customTags: WellKnownDiagnosticTags.CompilationEnd);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(
                IncorrectAssemblyNameDescriptor,
                NoBaseAssemblyDescriptor,
                NoBaseTestsDescriptor);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();

            context.RegisterCompilationAction(HandleCompilation);
        }

        private void HandleCompilation(CompilationAnalysisContext context)
        {
            var compilation = context.Compilation;
            if (!Helpers.TryGetAssemblyNamePrefixAndVersion(compilation.AssemblyName, out var assemblyPrefix, out _))
            {
                ReportDiagnostic(context, IncorrectAssemblyNameDescriptor);
                return;
            }

            if (!Helpers.TryGetBaseAssembly(compilation, assemblyPrefix, out var baseAssembly, out _))
            {
                ReportDiagnostic(context, NoBaseAssemblyDescriptor);
                return;
            }

            var types = Helpers.GetTestTypes(baseAssembly);
            if (types.Length == 0)
            {
                ReportDiagnostic(context, NoBaseTestsDescriptor);
                return;
            }
        }

        private static void ReportDiagnostic(CompilationAnalysisContext context, DiagnosticDescriptor descriptor)
        {
            var diagnostic = Diagnostic.Create(descriptor, null);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
