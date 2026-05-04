using Microsoft.VisualStudio.TestTools.UnitTesting;
using EmulatorAVR.Core;

namespace EmulatorAVR.Core.Tests
{
    [TestClass]
    public class CoreTests
    {
        [TestMethod]
        public void TestDummyLibraryMessage()
        {
            var message = DummyLibrary.GetMessage();
            Assert.AreEqual("Hello from DummyLibrary!", message);
        }
    }
}
