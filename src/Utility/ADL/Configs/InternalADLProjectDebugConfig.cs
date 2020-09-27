namespace Utility.ADL.Configs
{
    public static class InternalADLProjectDebugConfig
    {

        public static readonly ProjectDebugConfig<LogType, Verbosity> Settings =
            new ProjectDebugConfig<LogType, Verbosity>(
                                                       "Utility.ADL",
                                                       LogType.All,
                                                       Verbosity.Level1,
                                                       PrefixLookupSettings.AddPrefixIfAvailable |
                                                       PrefixLookupSettings.OnlyOnePrefix
                                                      );

    }
}