using System.Diagnostics;

namespace Utility.Threading.Utilities
{
    public class CommandInfo
    {

        public CommandInfo(string command, string workingDirectory = ".\\", bool useShell = false)
        {
            UseShell = useShell;
            WorkingDirectory = workingDirectory;
            Command = command;
        }

        public string Command { get; set; }

        public string WorkingDirectory { get; set; } = ".\\";

        public bool WaitForExit { get; set; } = true;

        public int WaitForExitTimeout { get; set; } = -1;

        public bool CreateWindow { get; set; }

        public bool UseShell { get; set; }

        public bool CaptureConsoleOut { get; set; }

        public DataReceivedEventHandler OnErrorReceived { get; set; }

        public DataReceivedEventHandler OnOutputReceived { get; set; }

    }
}