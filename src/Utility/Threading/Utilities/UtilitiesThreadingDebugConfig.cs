using Utility.ADL;
using Utility.ADL.Configs;

namespace Utility.Threading.Utilities
{
    public static class UtilitiesThreadingDebugConfig
    {

        public static readonly ProjectDebugConfig<LogType, Verbosity> Settings =
            new ProjectDebugConfig<LogType, Verbosity>(
                                                       "Threading",
                                                       LogType.All,
                                                       PrefixLookupSettings.AddPrefixIfAvailable |
                                                       PrefixLookupSettings.OnlyOnePrefix
                                                      );

    }
}