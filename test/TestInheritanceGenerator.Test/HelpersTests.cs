using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestInheritanceGenerator.Test
{
    [TestClass]
    public class HelpersTests
    {
        [TestMethod]
        [DataRow("Example.MSTest1", "Example.MSTest", "1")]
        [DataRow("Example.MSTest1_2", "Example.MSTest", "1_2")]
        [DataRow("Example.MSTest1_2_3", "Example.MSTest", "1_2_3")]
        [DataRow("Example.MSTest1_2_3_4", "Example.MSTest", "1_2_3_4")]
        [DataRow("Example.Test.V1", "Example.Test.V", "1")]
        [DataRow("Example.Test.V1_2", "Example.Test.V", "1_2")]
        [DataRow("Example.Test.V1_2_3", "Example.Test.V", "1_2_3")]
        [DataRow("Example.Test.V1_2_3_4", "Example.Test.V", "1_2_3_4")]
        [DataRow("Stuff.Test.V10_20_30", "Stuff.Test.V", "10_20_30")]
        public void TestSuccessfulTryGetAssemblyNamePrefixAndVersion(string input, string expectedPrefix, string expectedVersion)
        {
            var ok = Helpers.TryGetAssemblyNamePrefixAndVersion(input, out var prefix, out var version);
            Assert.IsTrue(ok);
            Assert.AreEqual(expectedPrefix, prefix);
            Assert.AreEqual(expectedVersion, version);
        }

        [TestMethod]
        [DataRow("Example.MSTest")]
        [DataRow("Example.Test.V")]
        [DataRow("Stuff.Test.V")]
        public void TestFailedTryGetAssemblyNamePrefixAndVersion(string input)
        {
#pragma warning disable IDE0059 // Unnecessary assignment of a value
            var ok = Helpers.TryGetAssemblyNamePrefixAndVersion(input, out var prefix, out var version);
#pragma warning restore IDE0059 // Unnecessary assignment of a value
            Assert.IsFalse(ok);
        }
    }
}
