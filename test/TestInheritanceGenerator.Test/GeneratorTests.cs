using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.Threading.Tasks;

namespace TestInheritanceGenerator.Test
{
    public abstract class GeneratorTests
    {
        [TestMethod]
        public async Task TestNoTests()
        {
            var tester = new CSharpSourceGeneratorTest<InheritanceGenerator, DefaultVerifier>()
            {
                TestState =
                {
                    Sources = { "" },
                    GeneratedSources = { },
                },
                ReferenceAssemblies = CreateReferenceAssemblies(),
            };

            await tester.RunAsync();
        }

        [TestMethod]
        public async Task TestWithoutBaseAssembly()
        {
            var sourceCode = CreateSourceCode("TestNamespace.V1", "TheTests");

            var tester = new CSharpSourceGeneratorTest<InheritanceGenerator, DefaultVerifier>
            {
                TestState =
                {
                    Sources = { sourceCode },
                    GeneratedSources = { },
                },
                ReferenceAssemblies = CreateReferenceAssemblies(),
            };

            await tester.RunAsync();
        }

        [TestMethod]
        public async Task TestStaticBaseType()
        {
            var sourceCode1 = $@"
namespace TestNamespace.V1
{{
    public static class TheTests
    {{
    }}
}}
";
            var generatedCode = CreateGeneratedCode("TestNamespace.V1", "TestNamespace.V2", "TheTests");

            var tester = new CSharpSourceGeneratorTest<InheritanceGenerator, DefaultVerifier>
            {
                TestState =
                {
                    Sources = { "" },
                    GeneratedSources = { },
                    AdditionalProjectReferences =
                    {
                        "Example.Tests1",
                    },
                    AdditionalProjects =
                    {
                        ["Example.Tests1"] =
                        {
                            Sources =
                            {
                                ("Tests.cs", sourceCode1),
                            },
                        },
                    },
                },
                ReferenceAssemblies = CreateReferenceAssemblies(),
            };

            tester.SolutionTransforms.Add((solution, projectId) =>
            {
                solution = SetMainProjectAssemblyName(solution, projectId, "Example.Tests2");
                return solution;
            });

            await tester.RunAsync();
        }

        [TestMethod]
        public async Task TestAbstractBaseType()
        {
            var sourceCode1 = CreateSourceCode("TestNamespace.V1", "TheTests", extraTypeKeywords: "abstract ");
            var generatedCode = CreateGeneratedCode("TestNamespace.V1", "TestNamespace.V2", "TheTests");

            var tester = new CSharpSourceGeneratorTest<InheritanceGenerator, DefaultVerifier>
            {
                TestState =
                {
                    Sources = { "" },
                    GeneratedSources = { },
                    AdditionalProjectReferences =
                    {
                        "Example.Tests1",
                    },
                    AdditionalProjects =
                    {
                        ["Example.Tests1"] =
                        {
                            Sources =
                            {
                                ("Tests.cs", sourceCode1),
                            },
                        },
                    },
                },
                ReferenceAssemblies = CreateReferenceAssemblies(),
            };

            tester.SolutionTransforms.Add((solution, projectId) =>
            {
                solution = SetMainProjectAssemblyName(solution, projectId, "Example.Tests2");
                return solution;
            });

            await tester.RunAsync();
        }

        [TestMethod]
        public async Task TestIrrelevantTypeAttribute()
        {
            var sourceCode1 = $@"
namespace TestNamespace.V1
{{
    [System.CLSCompliantAttribute(false)]
    public class TheTests
    {{
    }}
}}
";
            var generatedCode = CreateGeneratedCode("TestNamespace.V1", "TestNamespace.V2", "TheTests");

            var tester = new CSharpSourceGeneratorTest<InheritanceGenerator, DefaultVerifier>
            {
                TestState =
                {
                    Sources = { "" },
                    GeneratedSources = { },
                    AdditionalProjectReferences =
                    {
                        "Example.Tests1",
                    },
                    AdditionalProjects =
                    {
                        ["Example.Tests1"] =
                        {
                            Sources =
                            {
                                ("Tests.cs", sourceCode1),
                            },
                        },
                    },
                },
                ReferenceAssemblies = CreateReferenceAssemblies(),
            };

            tester.SolutionTransforms.Add((solution, projectId) =>
            {
                solution = SetMainProjectAssemblyName(solution, projectId, "Example.Tests2");
                return solution;
            });

            await tester.RunAsync();
        }

        [TestMethod]
        public async Task TestIrrelevantMethodAttribute()
        {
            var sourceCode1 = $@"
namespace TestNamespace.V1
{{
    public class TheTests
    {{
        [System.CLSCompliantAttribute(false)]
        public void Test1()
        {{
        }}
    }}
}}
";
            var generatedCode = CreateGeneratedCode("TestNamespace.V1", "TestNamespace.V2", "TheTests");

            var tester = new CSharpSourceGeneratorTest<InheritanceGenerator, DefaultVerifier>
            {
                TestState =
                {
                    Sources = { "" },
                    GeneratedSources = { },
                    AdditionalProjectReferences =
                    {
                        "Example.Tests1",
                    },
                    AdditionalProjects =
                    {
                        ["Example.Tests1"] =
                        {
                            Sources =
                            {
                                ("Tests.cs", sourceCode1),
                            },
                        },
                    },
                },
                ReferenceAssemblies = CreateReferenceAssemblies(),
            };

            tester.SolutionTransforms.Add((solution, projectId) =>
            {
                solution = SetMainProjectAssemblyName(solution, projectId, "Example.Tests2");
                return solution;
            });

            await tester.RunAsync();
        }

        [TestMethod]
        public async Task TestNonTestBaseType()
        {
            var sourceCode1 = $@"
namespace TestNamespace.V1
{{
    public class TheTests
    {{
    }}
}}
";
            var generatedCode = CreateGeneratedCode("TestNamespace.V1", "TestNamespace.V2", "TheTests");

            var tester = new CSharpSourceGeneratorTest<InheritanceGenerator, DefaultVerifier>
            {
                TestState =
                {
                    Sources = { "" },
                    GeneratedSources = { },
                    AdditionalProjectReferences =
                    {
                        "Example.Tests1",
                    },
                    AdditionalProjects =
                    {
                        ["Example.Tests1"] =
                        {
                            Sources =
                            {
                                ("Tests.cs", sourceCode1),
                            },
                        },
                    },
                },
                ReferenceAssemblies = CreateReferenceAssemblies(),
            };

            tester.SolutionTransforms.Add((solution, projectId) =>
            {
                solution = SetMainProjectAssemblyName(solution, projectId, "Example.Tests2");
                return solution;
            });

            await tester.RunAsync();
        }

        [TestMethod]
        [DataRow("1", "2")]
        [DataRow("1_1", "1_2")]
        [DataRow("1_1_1", "1_1_2")]
        [DataRow("1_1_1_1", "1_1_1_2")]
        public async Task TestWithBaseAssembly(string version1, string version2)
        {
            var sourceCode1 = CreateSourceCode("TestNamespace.V" + version1, "TheTests");
            var generatedCode = CreateGeneratedCode("TestNamespace.V" + version1, "TestNamespace.V" + version2, "TheTests");

            var tester = new CSharpSourceGeneratorTest<InheritanceGenerator, DefaultVerifier>
            {
                TestState =
                {
                    Sources = { "" },
                    GeneratedSources =
                    {
                        (typeof(InheritanceGenerator), $"TheTests.g.cs", SourceText.From(generatedCode, Encoding.UTF8)),
                    },
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
                                ("Tests.cs", sourceCode1),
                            },
                        },
                    },
                },
                ReferenceAssemblies = CreateReferenceAssemblies(),
            };

            tester.SolutionTransforms.Add((solution, projectId) =>
            {
                solution = SetMainProjectAssemblyName(solution, projectId, "Example.Tests" + version2);
                return solution;
            });

            await tester.RunAsync();
        }

        [TestMethod]
        public async Task TestMultipleBaseTypes()
        {
            var sourceCode1 = CreateSourceCode("TestNamespace.V1", "FooTests");
            var sourceCode2 = CreateSourceCode("TestNamespace.V1", "BarTests");
            var generatedCode1 = CreateGeneratedCode("TestNamespace.V1", "TestNamespace.V2", "FooTests");
            var generatedCode2 = CreateGeneratedCode("TestNamespace.V1", "TestNamespace.V2", "BarTests");

            var tester = new CSharpSourceGeneratorTest<InheritanceGenerator, DefaultVerifier>
            {
                TestState =
                {
                    Sources = { "" },
                    GeneratedSources =
                    {
                        (typeof(InheritanceGenerator), $"FooTests.g.cs", SourceText.From(generatedCode1, Encoding.UTF8)),
                        (typeof(InheritanceGenerator), $"BarTests.g.cs", SourceText.From(generatedCode2, Encoding.UTF8)),
                    },
                    AdditionalProjectReferences =
                    {
                        "Example.Tests1",
                    },
                    AdditionalProjects =
                    {
                        ["Example.Tests1"] =
                        {
                            Sources =
                            {
                                ("FooTests.cs", sourceCode1),
                                ("BarTests.cs", sourceCode2),
                            },
                        },
                    },
                },
                ReferenceAssemblies = CreateReferenceAssemblies(),
            };

            tester.SolutionTransforms.Add((solution, projectId) =>
            {
                solution = SetMainProjectAssemblyName(solution, projectId, "Example.Tests2");
                return solution;
            });

            await tester.RunAsync();
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        public async Task TestMultipleBaseAssemblies(int order)
        {
            var sourceCode1 = CreateSourceCode("TestNamespace.V2", "FooTests");
            var sourceCode2 = CreateSourceCode("TestNamespace.V2", "BarTests");
            var generatedCode1 = CreateGeneratedCode("TestNamespace.V2", "TestNamespace.V3", "FooTests");
            var generatedCode2 = CreateGeneratedCode("TestNamespace.V2", "TestNamespace.V3", "BarTests");
            var projectReference1 = order == 0 ? "Example.Tests1" : "Example.Tests2";
            var projectReference2 = order == 0 ? "Example.Tests2" : "Example.Tests1";

            var tester = new CSharpSourceGeneratorTest<InheritanceGenerator, DefaultVerifier>
            {
                TestState =
                {
                    Sources = { "" },
                    GeneratedSources =
                    {
                        (typeof(InheritanceGenerator), $"FooTests.g.cs", SourceText.From(generatedCode1, Encoding.UTF8)),
                        (typeof(InheritanceGenerator), $"BarTests.g.cs", SourceText.From(generatedCode2, Encoding.UTF8)),
                    },
                    AdditionalProjectReferences =
                    {
                        projectReference1,
                        projectReference2,
                    },
                    AdditionalProjects =
                    {
                        ["Example.Tests1"] =
                        {
                            Sources = { "" },
                        },
                        ["Example.Tests2"] =
                        {
                            Sources =
                            {
                                ("FooTests.cs", sourceCode1),
                                ("BarTests.cs", sourceCode2),
                            },
                        },
                    },
                },
                ReferenceAssemblies = CreateReferenceAssemblies(),
            };

            tester.SolutionTransforms.Add((solution, projectId) =>
            {
                solution = SetMainProjectAssemblyName(solution, projectId, "Example.Tests3");
                return solution;
            });

            await tester.RunAsync();
        }

        [TestMethod]
        public async Task TestClassAlreadyExists()
        {
            var sourceCode1 = CreateSourceCode("TestNamespace.V1", "TheTests");
            var sourceCode2 = CreateSourceCode("TestNamespace.V2", "TheTests", extraTypeKeywords: "partial ");
            var generatedCode1 = CreateGeneratedCode("TestNamespace.V1", "TestNamespace.V2", "TheTests", addAttribute: false);

            var tester = new CSharpSourceGeneratorTest<InheritanceGenerator, DefaultVerifier>
            {
                TestState =
                {
                    Sources =
                    {
                        ("TheTests.cs", sourceCode2),
                    },
                    GeneratedSources =
                    {
                        (typeof(InheritanceGenerator), $"TheTests.g.cs", SourceText.From(generatedCode1, Encoding.UTF8)),
                    },
                    AdditionalProjectReferences =
                    {
                        "Example.Tests1",
                    },
                    AdditionalProjects =
                    {
                        ["Example.Tests1"] =
                        {
                            Sources =
                            {
                                ("TheTests.cs", sourceCode1),
                            },
                        },
                    },
                },
                ReferenceAssemblies = CreateReferenceAssemblies(),
            };

            tester.SolutionTransforms.Add((solution, projectId) =>
            {
                solution = SetMainProjectAssemblyName(solution, projectId, "Example.Tests2");
                return solution;
            });

            await tester.RunAsync();
        }

        [TestMethod]
        public async Task BaseTestClassHasSkipInheritanceGenerationAttribute()
        {
            var sourceCode1 = CreateSourceCode("TestNamespace.V1", "TheTests", addSkipInheritanceGenerationAttribute: true);

            var tester = new CSharpSourceGeneratorTest<InheritanceGenerator, DefaultVerifier>
            {
                TestState =
                {
                    Sources = { "" },
                    GeneratedSources = { },
                    AdditionalProjectReferences =
                    {
                        "Example.Tests1",
                    },
                    AdditionalProjects =
                    {
                        ["Example.Tests1"] =
                        {
                            Sources =
                            {
                                ("SkipInheritanceGenerationAttribute.cs", "internal class SkipInheritanceGenerationAttribute : System.Attribute { }"),
                                ("TheTests.cs", sourceCode1),
                            },
                        },
                    },
                },
                ReferenceAssemblies = CreateReferenceAssemblies(),
            };

            tester.SolutionTransforms.Add((solution, projectId) =>
            {
                solution = SetMainProjectAssemblyName(solution, projectId, "Example.Tests2");
                return solution;
            });

            await tester.RunAsync();
        }

        private ReferenceAssemblies CreateReferenceAssemblies()
        {
            return AddReferenceAssemblies(ReferenceAssemblies.Net.Net80);
        }

        private static Solution SetMainProjectAssemblyName(Solution solution, ProjectId projectId, string assemblyName)
        {
            solution = solution.WithProjectAssemblyName(projectId, assemblyName);
            return solution;
        }

        protected abstract ReferenceAssemblies AddReferenceAssemblies(ReferenceAssemblies input);

        protected abstract string CreateSourceCode(string @namespace, string name, string extraTypeKeywords = "", bool addSkipInheritanceGenerationAttribute = false);

        protected abstract string CreateGeneratedCode(string namespace1, string namespace2, string name, bool addAttribute = true);
    }
}
