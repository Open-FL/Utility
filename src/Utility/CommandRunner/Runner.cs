using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Utility.ADL;

namespace Utility.CommandRunner
{
    /// <summary>
    ///     Contains the Logic for Running Commands
    /// </summary>
    public class Runner
    {

        private static readonly Runner instance = new Runner();

        private static ADLLogger<LogType> _logger;

        /// <summary>
        ///     All Commands currently loaded in the Library
        /// </summary>
        private readonly List<AbstractCommand> Commands = new List<AbstractCommand>();

        private static ADLLogger<LogType> Logger =>
            _logger ?? (_logger = new ADLLogger<LogType>(CommandRunnerDebugConfig.Settings));

        /// <summary>
        ///     Count of the Loaded Commands.
        /// </summary>
        public int _CommandCount => Commands.Count;

        public static int CommandCount => instance._CommandCount;

        public static void AddAssembly(string path)
        {
            instance._AddAssembly(path);
        }

        /// <summary>
        ///     Adds an Assemblys Commands by its Full Path
        /// </summary>
        /// <param name="path">Full path to assembly.</param>
        public void _AddAssembly(string path)
        {
            if (AssemblyHelper.TryLoadAssembly(path, out Assembly asm))
            {
                _AddAssembly(asm);
            }
        }


        public static void AddAssembly(Assembly asm)
        {
            instance._AddAssembly(asm);
        }

        /// <summary>
        ///     Adds an Assemblys Commands
        /// </summary>
        /// <param name="asm">Assembly to Add</param>
        public void _AddAssembly(Assembly asm)
        {
            List<AbstractCommand> cmds = AssemblyHelper.LoadCommandsFromAssembly(asm);
            for (int i = 0; i < cmds.Count; i++)
            {
                _AddCommand(cmds[i]);
            }
        }

        public static void AddCommand(AbstractCommand cmd)
        {
            instance._AddCommand(cmd);
        }

        /// <summary>
        ///     Adds a Single Command to the System.
        /// </summary>
        /// <param name="cmd"></param>
        public void _AddCommand(AbstractCommand cmd)
        {
            Logger.Log(LogType.Log, "Adding Command: " + cmd.GetType().FullName, 2);
            if (_IsInterfering(cmd))
            {
                Logger.Log(
                           LogType.Log,
                           "Command:" + cmd.GetType().FullName + " is interfering with other Commands.",
                           1
                          );
            }

            Commands.Add(cmd);
        }


        public static void RemoveAt(int index)
        {
            instance._RemoveAt(index);
        }

        public void _RemoveAt(int index)
        {
            if (index >= 0 && Commands.Count > index)
            {
                Commands.RemoveAt(index);
            }
        }

        public static void RemoveAllCommands()
        {
            instance._RemoveAllCommands();
        }

        public void _RemoveAllCommands()
        {
            Commands.Clear();
        }

        public static bool IsInterfering(AbstractCommand cmd)
        {
            return instance._IsInterfering(cmd);
        }

        /// <summary>
        ///     Checks if the system is already containing a command with the same command keys
        /// </summary>
        /// <param name="cmd">The Command</param>
        /// <returns>Returns true when interfering with other commands.</returns>
        private bool _IsInterfering(AbstractCommand cmd)
        {
            for (int i = 0; i < Commands.Count; i++)
            {
                if (cmd.IsInterfering(Commands[i]))
                {
                    return true;
                }
            }

            return false;
        }


        public static AbstractCommand GetCommandAt(int index)
        {
            return instance._GetCommandAt(index);
        }

        /// <summary>
        ///     Returns the command at index.
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>Command at index.</returns>
        public AbstractCommand _GetCommandAt(int index)
        {
            return Commands[index];
        }


        public static bool RunCommands(string[] args)
        {
            return instance._RunCommands(args);
        }

        /// <summary>
        ///     Runs the Commands with the Passed arguments.
        /// </summary>
        /// <param name="args">The arguments to use</param>
        public bool _RunCommands(string[] args)
        {
            bool didExecute = false;
            for (int i = 0; i < args.Length; i++)
            {
                for (int j = 0; j < Commands.Count; j++)
                {
                    if (Commands[j].CommandKeys.Contains(args[i]))
                    {
                        args[i] = Commands[j].CommandKeys[0]; //Make sure its the first command key.
                    }
                }
            }

            StartupArgumentInfo argumentInfo = new StartupArgumentInfo(args);


            if (argumentInfo.GetCommandEntries("noflag") != 0 ||
                argumentInfo.GetCommandEntries("noflag") == 0 && argumentInfo.CommandCount == 0)
            {
                List<AbstractCommand> cmds = Commands.Where(x => x.DefaultCommand).ToList();
                if (cmds.Count == 0)
                {
                    Logger.Log(LogType.Warning, "No Default Command Found", 1);
                    return didExecute;
                }

                didExecute = true;

                if (cmds.Count != 1)
                {
                    Logger.Log(LogType.Warning, "Found more than one Default Command.", 1);
                    Logger.Log(LogType.Warning, "Using Command: " + cmds[0].CommandKeys[0], 1);
                }

                if (argumentInfo.GetCommandEntries("noflag") != 0)
                {
                    for (int j = 0; j < argumentInfo.GetCommandEntries("noflag"); j++)
                    {
                        cmds[0].CommandAction?.Invoke(argumentInfo, argumentInfo.GetValues("noflag", j).ToArray());
                    }
                }
                else
                {
                    cmds[0].CommandAction?.Invoke(argumentInfo, new string[0]);
                }
            }

            for (int i = 0; i < Commands.Count; i++)
            {
                if (argumentInfo.GetCommandEntries(Commands[i].CommandKeys[0]) != 0)
                {
                    for (int j = 0; j < argumentInfo.GetCommandEntries(Commands[i].CommandKeys[0]); j++)
                    {
                        didExecute = true;
                        Commands[i].CommandAction?.Invoke(
                                                          argumentInfo,
                                                          argumentInfo
                                                              .GetValues(Commands[i].CommandKeys[0], j).ToArray()
                                                         );
                    }
                }
            }

            return didExecute;
        }

    }
}