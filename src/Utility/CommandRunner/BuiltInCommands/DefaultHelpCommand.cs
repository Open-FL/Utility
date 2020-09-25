using Utility.ADL;

namespace Utility.CommandRunner.BuiltInCommands
{
    public class DefaultHelpCommand : AbstractCommand
    {

        public DefaultHelpCommand(bool defaultCommand = false) : base(
                                                                      new[] { "--help", "-h", "-?" },
                                                                      "Prints this help text",
                                                                      defaultCommand
                                                                     )
        {
            CommandAction = (info, strings) => DefaultHelp();
        }

        private void DefaultHelp()
        {
            for (int i = 0; i < Runner.CommandCount; i++)
            {
                Logger.Log(
                           LogType.Log,
                           "__________________________________________________________",
                           MIN_COMMAND_SEVERITY
                          );
                Logger.Log(LogType.Log, "", MIN_COMMAND_SEVERITY);
                Logger.Log(LogType.Log, Runner.GetCommandAt(i).ToString(), MIN_COMMAND_SEVERITY);
            }
        }

    }
}