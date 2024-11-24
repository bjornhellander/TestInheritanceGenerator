using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestInheritanceGenerator.Test
{
    [TestClass]
    public class XUnitGeneratorTests : GeneratorTests
    {
        protected override ReferenceAssemblies AddReferenceAssemblies(ReferenceAssemblies input)
        {
            var result = input.AddPackages([new PackageIdentity("xunit", "2.9.2")]);
            return result;
        }

        protected override string CreateSourceCode(string @namespace, string name, string extraTypeKeywords)
        {
            var result = $@"
using Xunit;

namespace {@namespace}
{{
    public {extraTypeKeywords}class {name}
    {{
        [Fact]
        public void Test1()
        {{
        }}
    }}
}}";
            return result;
        }

        protected override string CreateGeneratedCode(string namespace1, string namespace2, string name, bool addAttribute)
        {
            _ = addAttribute;

            var result = $@"
namespace {namespace2}
{{
    
    public partial class {name} : {namespace1}.{name}
    {{
    }}
}}
";
            return result;
        }
    }
}
