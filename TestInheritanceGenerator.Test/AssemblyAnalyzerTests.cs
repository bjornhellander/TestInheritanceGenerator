using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

using VerifyCS = TestInheritanceGenerator.Test.Verifiers.CSharpAnalyzerVerifier<
    TestInheritanceGenerator.AssemblyAnalyzer>;

namespace TestInheritanceGenerator.Test
{
    [TestClass]
    public class AssemblyAnalyzerTests
    {
        [TestMethod]
        public async Task TestNoTests()
        {
            var test = new VerifyCS.Test()
            {
                TestState =
                {
                    Sources = { "" },
                },
                ExpectedDiagnostics = { VerifyCS.Diagnostic(AssemblyAnalyzer.IncorrectAssemblyNameDiagnosticId) },
            };

            await test.RunAsync();
        }

        [TestMethod]
        [DataRow("1")]
        [DataRow("1_1")]
        [DataRow("1_1_1")]
        [DataRow("1_1_1_1")]
        public async Task TestWithoutBaseAssembly(string version)
        {
            var test = new VerifyCS.Test()
            {
                TestState =
                {
                    Sources = { "" },
                },
                ExpectedDiagnostics = { VerifyCS.Diagnostic(AssemblyAnalyzer.NoBaseAssemblyDiagnosticId) },
            };

            test.SolutionTransforms.Add((solution, projectId) =>
            {
                var project = solution.GetProject(projectId);
                if (project.Name == "TestProject")
                {
                    solution = solution.WithProjectAssemblyName(projectId, "Example.Tests" + version);
                }

                return solution;
            });

            await test.RunAsync();
        }

        [TestMethod]
        [DataRow("1", "2")]
        [DataRow("1_1", "1_2")]
        [DataRow("1_1_1", "1_1_2")]
        [DataRow("1_1_1_1", "1_1_1_2")]
        public async Task TestWithBaseAssembly(string version1, string version2)
        {
            var test = new VerifyCS.Test()
            {
                TestState =
                {
                    Sources = { "" },
                    AdditionalProjectReferences =
                    {
                        "Example.Tests" + version1,
                    },
                    AdditionalProjects =
                    {
                        ["Example.Tests" + version1] =
                        {
                            Sources =
                            {
                                ("Tests.cs", ""),
                            },
                        },
                    },
                },
                ExpectedDiagnostics = {
                    VerifyCS.Diagnostic(AssemblyAnalyzer.NoBaseTestsDiagnosticId), // From the main project
                    VerifyCS.Diagnostic(AssemblyAnalyzer.NoBaseAssemblyDiagnosticId), // From the additional project
                },
            };

            test.SolutionTransforms.Add((solution, projectId) =>
            {
                var project = solution.GetProject(projectId);
                if (project.Name == "TestProject")
                {
                    solution = solution.WithProjectAssemblyName(projectId, "Example.Tests" + version2);
                }
                return solution;
            });

            await test.RunAsync();
        }
    }
}
