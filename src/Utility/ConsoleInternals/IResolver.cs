using System;

namespace Utility.ConsoleInternals
{
    public interface IResolver : IDisposable
    {

        string FileExtension { get; }

        string ResolveLibrary(string libraryFile);

    }
}