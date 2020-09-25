namespace Utility.CommandRunner
{
    public abstract class AbstractCommandModuleInfo
    {

        public abstract string ModuleName { get; }

        public virtual string[] Dependencies => new string[0];

        public abstract void RunArgs(string[] args);

    }
}