using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestInheritanceGenerator.Test
{
    [TestClass]
    public class MsTestGeneratorTests : GeneratorTests
    {
        protected override ReferenceAssemblies AddReferenceAssemblies(ReferenceAssemblies input)
        {
            var result = input.AddPackages([new PackageIdentity("MSTest.TestFramework", "3.6.3")]);
            return result;
        }

        protected override string CreateSourceCode(string @namespace, string name, string extraTypeKeywords)
        {
            var result = $@"
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace {@namespace}
{{
    [TestClass]
    public {extraTypeKeywords}class {name}
    {{
    }}
}}";
            return result;
        }

        protected override string CreateGeneratedCode(string namespace1, string namespace2, string name)
        {
            var result = $@"
namespace {namespace2}
{{
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute]
    public partial class {name} : {namespace1}.{name}
    {{
    }}
}}
";
            return result;
        }
    }
}
