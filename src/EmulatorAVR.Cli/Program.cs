using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using EmulatorAVR.Core;

var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddNLog();
});

Console.WriteLine("EmulatorAVR CLI");
Console.WriteLine(DummyLibrary.GetMessage());