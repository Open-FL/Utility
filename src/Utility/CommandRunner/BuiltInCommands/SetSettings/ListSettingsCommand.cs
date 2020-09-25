using Utility.ADL;
using Utility.FastString;

namespace Utility.CommandRunner.BuiltInCommands.SetSettings
{
    public class ListSettingsCommand : AbstractCommand
    {

        private readonly SetSettingsCommand ss;

        public ListSettingsCommand(SetSettingsCommand cmd) : base(
                                                                  new[] { "--list-settings", "-ls" },
                                                                  "Lists all settings that can be changed with the command --set-settings"
                                                                 )
        {
            CommandAction = (info, strings) => ListSettings();
            ss = cmd;
        }

        private void ListSettings()
        {
            Logger.Log(LogType.Log, "Available Settings: \n" + ss.AllPaths.Unpack("\n"), 1);
        }

    }
}