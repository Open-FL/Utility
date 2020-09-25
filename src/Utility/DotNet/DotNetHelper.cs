using System.Diagnostics;
using System.IO;

using Utility.ADL;
using Utility.Threading.Utilities;

namespace Utility.DotNet
{
    public static class DotNetHelper
    {

        public static readonly ADLLogger<LogType> Logger =
            new ADLLogger<LogType>(UtilitiesDotNetDebugConfig.Settings, "DotNetHelper");

        public static void DotnetAction(
            string msbuildCommand, string targetCommand, string arguments,
            string workingDir)
        {
            CommandInfo info =
                new CommandInfo($"{msbuildCommand} {targetCommand} {arguments}", workingDir, true)
                {
                    CreateWindow = false,
                    CaptureConsoleOut = true,
                    OnErrorReceived = OnErrorReceived,
                    OnOutputReceived = OnOutReceived
                };
            ProcessRunner.RunCommand(info);
        }

        public static void NewCommandLine(string msbuildCommand, string workingDir, string projectName)
        {
            string arguments = $"console -n {projectName}";
            DotnetAction(msbuildCommand, "new", arguments, workingDir);
        }

        public static void NewLib(string msbuildCommand, string workingDir, string projectName)
        {
            string arguments = $"classlib -n {projectName}";
            DotnetAction(msbuildCommand, "new", arguments, workingDir);
        }

        public static string BuildProject(
            string msbuildCommand, string projectFile, AssemblyDefinition definitions,
            bool lib = true)
        {
            Logger.Log(LogType.Log, "Building Assembly: " + definitions.AssemblyName, 1);
            string arguments = $"-c {definitions.BuildConfiguration}";
            if (!definitions.NoTargetRuntime)
            {
                arguments = $"--runtime {definitions.BuildTargetRuntime} {arguments}";
            }

            string workingDir = Path.GetDirectoryName(projectFile);
            DotnetAction(msbuildCommand, "build", arguments, workingDir);
            string ret = Path.Combine(
                                      workingDir,
                                      "bin",
                                      definitions.BuildConfiguration,
                                      lib ? "netstandard2.0" : "netcoreapp2.2"
                                     );
            if (!definitions.NoTargetRuntime)
            {
                ret = Path.Combine(ret, definitions.BuildTargetRuntime);
            }

            return ret;
        }

        public static string PublishProject(
            string msbuildCommand, string projectFile, AssemblyDefinition definitions,
            bool lib = true)
        {
            Logger.Log(LogType.Log, "Publishing Assembly: " + definitions.AssemblyName, 1);
            string arguments = $"-c {definitions.BuildConfiguration}";
            if (!definitions.NoTargetRuntime)
            {
                arguments = $"--runtime {definitions.BuildTargetRuntime} {arguments}";
            }

            string workingDir = Path.GetDirectoryName(projectFile);
            DotnetAction(msbuildCommand, "publish", arguments, workingDir);
            string ret = Path.Combine(
                                      workingDir,
                                      "bin",
                                      definitions.BuildConfiguration,
                                      lib ? "netstandard2.0" : "netcoreapp2.2"
                                     );
            if (!definitions.NoTargetRuntime)
            {
                ret = Path.Combine(ret, definitions.BuildTargetRuntime);
            }

            ret = Path.Combine(ret, "publish");
            return ret;
        }


        private static void OnErrorReceived(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Data))
            {
                return;
            }

            Logger.Log(LogType.Error, "\t[ERR]" + e.Data, 5);
        }

        private static void OnOutReceived(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Data))
            {
                return;
            }

            Logger.Log(LogType.Log, "\t" + e.Data, 5);
        }

    }
}