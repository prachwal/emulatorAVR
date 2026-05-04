using EmulatorAVR.Cli;

var (exitCode, stdout, stderr) = CliRunner.Run(args);

if (stdout.Length > 0)
    Console.WriteLine(stdout);

if (stderr.Length > 0)
    Console.Error.WriteLine(stderr);

if (exitCode != 0)
    Environment.Exit(exitCode);
