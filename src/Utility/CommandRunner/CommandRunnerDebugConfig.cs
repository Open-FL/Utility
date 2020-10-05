using Utility.ADL;
using Utility.ADL.Configs;

namespace Utility.CommandRunner
{
    public static class CommandRunnerDebugConfig
    {

        public static readonly ProjectDebugConfig<LogType, Verbosity> Settings =
            new ProjectDebugConfig<LogType, Verbosity>(
                                                       "CmdRunner",
                                                       LogType.All,
                                                       PrefixLookupSettings.AddPrefixIfAvailable |
                                                       PrefixLookupSettings.OnlyOnePrefix
                                                      );

    }
}