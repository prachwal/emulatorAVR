using Microsoft.VisualStudio.TestTools.UnitTesting;
using EmulatorAVR.Core;

namespace EmulatorAVR.Core.Tests
{
    [TestClass]
    public class DummyTest
    {
        [TestMethod]
        public void TestOneEqualsOne()
        {
            Assert.AreEqual(1, 1);
        }

        [TestMethod]
        public void TestDummyLibrary()
        {
            Assert.AreEqual("Hello from DummyLibrary!", DummyLibrary.GetMessage());
        }
    }
}
