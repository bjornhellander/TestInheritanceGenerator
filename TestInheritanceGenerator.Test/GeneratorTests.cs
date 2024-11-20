﻿using Microsoft.CodeAnalysis.CSharp.Testing;
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
                var project = solution.GetProject(projectId);
                if (project.Name == "TestProject")
                {
                    solution = solution.WithProjectAssemblyName(projectId, "Example.Tests" + version2);
                }

                return solution;
            });

            await tester.RunAsync();
        }

        private ReferenceAssemblies CreateReferenceAssemblies()
        {
            return AddReferenceAssemblies(ReferenceAssemblies.Net.Net80);
        }

        protected abstract ReferenceAssemblies AddReferenceAssemblies(ReferenceAssemblies input);

        protected abstract string CreateSourceCode(string @namespace, string name);

        protected abstract string CreateGeneratedCode(string namespace1, string namespace2, string name);
    }
}
