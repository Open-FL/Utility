namespace Utility.ConsoleInternals
{
    public abstract class AConsole
    {

        public abstract string ConsoleKey { get; }

        public virtual string ConsoleTitle { get; } = "No Name Specified";

        public abstract bool Run(string[] args);

    }
}