using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System;

namespace EmulatorAVR.Cli.Tests
{
    [TestClass]
    public class ProgramTests
    {
        [TestMethod]
        public void TestProgramOutput()
        {
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                Console.WriteLine("EmulatorAVR CLI");
                Console.WriteLine(EmulatorAVR.Core.DummyLibrary.GetMessage());

                var result = sw.ToString();
                Assert.IsTrue(result.Contains("EmulatorAVR CLI"));
                Assert.IsTrue(result.Contains("Hello from DummyLibrary!"));
            }
        }
    }
}
