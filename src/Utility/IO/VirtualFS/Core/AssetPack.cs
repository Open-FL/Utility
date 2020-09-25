using System.IO;

namespace Utility.IO.VirtualFS.Core
{
    /// <summary>
    /// A Container for the raw byte content of a package
    /// </summary>
    public class AssetPack
    {

        private readonly Stream Content;

        public AssetPack(Stream s)
        {
            Content = s;
        }

        public int BytesWritten { get; private set; }

        public int SpaceLeft => AssetPacker.MaxsizeKilobytes * AssetPacker.Kilobyte - BytesWritten;

        public void Save()

        {
            Content.Close();
        }

        public void Write(byte[] buf, int start, int count)
        {
            Content.Write(buf, start, count);
            BytesWritten += count;
        }

    }
}