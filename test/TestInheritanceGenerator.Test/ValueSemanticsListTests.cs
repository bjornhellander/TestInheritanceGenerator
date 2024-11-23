using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestInheritanceGenerator.Test
{
    [TestClass]
    public class ValueSemanticsListTests
    {
        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        [DataRow(5)]
        public void TestEqual(int count)
        {
            var input1 = Create(count);
            var input2 = Create(count);

            Assert.AreEqual(input1, input2);
            Assert.AreEqual(input1.GetHashCode(), input2.GetHashCode());
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        [DataRow(5)]
        public void TestNotEqual(int count)
        {
            var input1 = Create(count);
            input1.Add(100);
            var input2 = Create(count);
            input2.Add(101);

            Assert.AreNotEqual(input1, input2);
            Assert.AreNotEqual(input1.GetHashCode(), input2.GetHashCode()); // NOT: Strictly not necessary, but likely
        }

        private static ValueSemanticsList<int> Create(int count)
        {
            var result = new ValueSemanticsList<int>();
            for (var i = 0; i < count; i++)
            {
                result.Add(i);
            }
            return result;
        }
    }
}
