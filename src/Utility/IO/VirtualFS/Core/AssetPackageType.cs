namespace Utility.IO.VirtualFS.Core
{
    /// <summary>
    /// The Type of Asset Packaging
    /// </summary>
    public enum AssetPackageType
    {

        /// <summary>
        /// File will be only kept in memory
        /// </summary>
        Memory,

        /// <summary>
        /// File will be written to disk in the initialization
        /// </summary>
        Unpack

    }
}