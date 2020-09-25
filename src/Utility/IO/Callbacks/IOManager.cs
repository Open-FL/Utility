using System;
using System.IO;

namespace Utility.IO.Callbacks
{
    public static class IOManager
    {

        private static Action<OnFileReadEvent> OnFileRead = @event => { };
        private static Action<AfterFileReadEvent> AfterFileRead = @event => { };

        public static IOCallback Callback { get; private set; } = new DefaultIOCallback();

        public static void SetIOCallback(IOCallback callback)
        {
            Callback = callback;
        }

        public static void AttachOnFileReadEvent(Action<OnFileReadEvent> ev)
        {
            OnFileRead += ev;
        }

        public static void DetachOnFileReadEvent(Action<OnFileReadEvent> ev)
        {
            if (OnFileRead != null)
            {
                OnFileRead -= ev;
            }
        }

        public static void AttachAfterFileReadEvent(Action<AfterFileReadEvent> ev)
        {
            AfterFileRead += ev;
        }

        public static void DetachAfterFileReadEvent(Action<AfterFileReadEvent> ev)
        {
            if (OnFileRead != null)
            {
                AfterFileRead -= ev;
            }
        }

        public static bool FileExists(string file)
        {
            return Callback.FileExists(file);
        }

        public static bool DirectoryExists(string file)
        {
            return Callback.DirectoryExists(file);
        }

        public static string[] ReadAllLines(string file)
        {
            OnFileRead(new OnFileReadEvent(file));
            string[] ret = Callback.ReadLines(file);
            AfterFileRead(new AfterFileReadEvent(file, AfterFileReadEvent.FileOpenType.Lines, ret));
            return ret;
        }

        public static string ReadText(string file)
        {
            OnFileRead(new OnFileReadEvent(file));
            string ret = Callback.ReadText(file);
            AfterFileRead(new AfterFileReadEvent(file, AfterFileReadEvent.FileOpenType.Text, ret));
            return ret;
        }

        public static Stream GetStream(string file)
        {
            OnFileRead(new OnFileReadEvent(file));
            Stream ret = Callback.GetStream(file);
            AfterFileRead(new AfterFileReadEvent(file, AfterFileReadEvent.FileOpenType.Stream, ret));
            return ret;
        }

        public static string[] GetFiles(string path, string searchPattern = "*.*")
        {
            return Callback.GetFiles(path, searchPattern);
        }

        public class OnFileReadEvent
        {

            public OnFileReadEvent(string path)
            {
                Path = path;
            }

            public string Path { get; }

        }


        public class AfterFileReadEvent
        {

            public enum FileOpenType
            {

                Stream,
                Lines,
                Text

            }

            public AfterFileReadEvent(string path, FileOpenType openType, object data)
            {
                Path = path;
                OpenType = openType;
                Data = data;
            }

            public FileOpenType OpenType { get; }

            public string Path { get; }

            public object Data { get; }

        }

    }
}