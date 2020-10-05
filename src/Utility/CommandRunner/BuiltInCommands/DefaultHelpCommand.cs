using Utility.ADL;

namespace Utility.CommandRunner.BuiltInCommands
{
    public class DefaultHelpCommand : AbstractCommand
    {

        private readonly Runner runner;

        public DefaultHelpCommand(Runner instance, bool defaultCommand = false) : base(
             new[] { "--help", "-h", "-?" },
             "Prints this help text",
             defaultCommand
            )
        {
            runner = instance;
        }

        public DefaultHelpCommand(bool defaultCommand = false) : this(null, defaultCommand)
        {
            CommandAction = (info, strings) => DefaultHelp();
        }

        private void DefaultHelp()
        {
            int count = runner?._CommandCount ?? Runner.CommandCount;
            for (int i = 0; i < count; i++)
            {
                Logger.Log(
                           LogType.Log,
                           "__________________________________________________________",
                           MIN_COMMAND_SEVERITY
                          );

                string commandText = runner?._GetCommandAt(i)?.ToString() ?? Runner.GetCommandAt(i).ToString();

                Logger.Log(LogType.Log, "", MIN_COMMAND_SEVERITY);
                Logger.Log(LogType.Log, commandText, MIN_COMMAND_SEVERITY);
            }
        }

    }
}