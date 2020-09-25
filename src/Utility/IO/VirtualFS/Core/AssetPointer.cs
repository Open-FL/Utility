using System;

namespace Utility.IO.VirtualFS.Core
{
    /// <summary>
    /// Pointer Structure used to find a file in a raw byte stream
    /// </summary>
    [Serializable]
    public class AssetPointer
    {

        public int Length { get; set; }

        public int Offset { get; set; }

        public int PackageId { get; set; }

        public int PackageSize { get; set; }

        public AssetPackageType PackageType { get; set; }

        public string Path { get; set; }

        public static int GetPackageCount(int offset, int length, int packageSize)
        {
            if (offset + length <= packageSize)
            {
                return 1;
            }

            int leftBytes = length - (packageSize - offset);
            int ceil = (int) Math.Ceiling(1 + leftBytes / (float) packageSize);
            return ceil;
        }

        public override string ToString()
        {
            return $"Path: {Path}, PID: {PackageId}, Offset: {Offset}, Length: {Length}";
        }

    }
}