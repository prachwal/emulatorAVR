using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddNLog();
});

Console.WriteLine("EmulatorAVR CLI");