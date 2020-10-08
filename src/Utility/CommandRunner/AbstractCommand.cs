﻿using System;
using System.Linq;
using System.Text;

using Utility.ADL;

namespace Utility.CommandRunner
{
    /// <summary>
    ///     Abstract Command Class that contains all the info for a Command.
    /// </summary>
    public abstract class AbstractCommand : ALoggable<LogType>
    {

        protected const int MIN_COMMAND_SEVERITY = 3;

        /// <summary>
        ///     Protected Constructor
        /// </summary>
        /// <param name="action">Command Implementation</param>
        /// <param name="keys">Keys of the Command</param>
        /// <param name="helpText">Optional Help Text</param>
        /// <param name="defaultCommand">Flag that indicates if this command is a default command.</param>
        protected AbstractCommand(
            Action<StartupArgumentInfo, string[]> action, string[] keys,
            string helpText = "No Help Text Available", bool defaultCommand = false) : base(
             CommandRunnerDebugConfig
                 .Settings,
             GetCommandName(keys.First())
            )
        {
            CommandAction = action;
            CommandKeys = keys;
            HelpText = helpText;
            DefaultCommand = defaultCommand;
        }

        /// <summary>
        ///     Protected Constructor
        /// </summary>
        /// <param name="keys">Keys of the Command</param>
        /// <param name="helpText">Optional Help Text</param>
        /// <param name="defaultCommand">Flag that indicates if this command is a default command.</param>
        protected AbstractCommand(
            string[] keys, string helpText = "No Help Text Available",
            bool defaultCommand = false) : base(CommandRunnerDebugConfig.Settings, GetCommandName(keys.First()))
        {
            CommandKeys = keys;
            HelpText = helpText;
            DefaultCommand = defaultCommand;
        }

        /// <summary>
        ///     The Command Implementation that is getting called
        /// </summary>
        public Action<StartupArgumentInfo, string[]> CommandAction { get; protected set; }

        /// <summary>
        ///     The Keys that are used to indicate that the Command should be executed.
        /// </summary>
        public string[] CommandKeys { get; }

        /// <summary>
        ///     Optional Help text that can be used to create a help command.
        /// </summary>
        public string HelpText { get; }

        /// <summary>
        ///     When set to true, the parameters that do not have a command key infront of it will be passed to this command.
        /// </summary>
        public bool DefaultCommand { get; }

        private static string GetCommandName(string key)
        {
            string ret = key;
            while (ret.StartsWith("-"))
            {
                ret = ret.Substring(1);
            }

            return ret;
        }


        public bool IsInterfering(AbstractCommand other)
        {
            for (int i = 0; i < CommandKeys.Length; i++)
            {
                if (other.CommandKeys.Contains(CommandKeys[i]))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     To String Override, displaying the Help Text and other useful information.
        /// </summary>
        /// <returns>String Representation of the Command.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(CommandKeys[0]);
            for (int i = 1; i < CommandKeys.Length; i++)
            {
                sb.Append(" | " + CommandKeys[i]);
            }

            sb.AppendLine("\nDefault Command: " + DefaultCommand + "\n");
            string[] helpText = HelpText.Split('\n');
            for (int i = 0; i < helpText.Length; i++)
            {
                sb.AppendLine($"\t{helpText[i]}");
            }

            return sb.ToString();
        }

    }
}