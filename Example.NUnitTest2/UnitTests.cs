using NUnit.Framework;

namespace Example.NUnitTest2
{
    public class UnitTests : NUnitTest1.UnitTests
    {

        [Test]
        public void Test2()
        {
            Assert.Pass();
        }
    }
}
