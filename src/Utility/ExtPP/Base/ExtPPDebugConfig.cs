using Utility.ADL;
using Utility.ADL.Configs;

namespace Utility.ExtPP.Base
{
    public static class ExtPPDebugConfig
    {

        public static readonly ProjectDebugConfig<LogType, Verbosity> Settings =
            new ProjectDebugConfig<LogType, Verbosity>(
                                                       "Utility.ExtPP",
                                                       LogType.All,
                                                       Verbosity.Level1,
                                                       PrefixLookupSettings.AddPrefixIfAvailable |
                                                       PrefixLookupSettings.OnlyOnePrefix
                                                      );

    }
}