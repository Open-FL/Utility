using Utility.ADL;
using Utility.ADL.Configs;

namespace Utility.Threading.Utilities
{
    public static class UtilitiesThreadingDebugConfig
    {

        public static readonly ProjectDebugConfig<LogType, Verbosity> Settings =
            new ProjectDebugConfig<LogType, Verbosity>(
                                                       "OpenFL.Utility.Threading",
                                                       LogType.All,
                                                       Verbosity.Level1,
                                                       PrefixLookupSettings.AddPrefixIfAvailable |
                                                       PrefixLookupSettings.OnlyOnePrefix
                                                      );

    }
}