using Utility.ADL;
using Utility.ADL.Configs;

namespace Utility.IO.VirtualFS
{
    public static class ManifestIODebugConfig
    {

        public static readonly ProjectDebugConfig<LogType, Verbosity> Settings =
            new ProjectDebugConfig<LogType, Verbosity>(
                                                       "VirtFS",
                                                       LogType.All,
                                                       PrefixLookupSettings.AddPrefixIfAvailable |
                                                       PrefixLookupSettings.OnlyOnePrefix
                                                      );

    }
}