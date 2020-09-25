namespace Utility.Collections.Interfaces
{
    public interface IVec3 : IVec2
    {

        float Z { get; set; }

        IVec3 GetNewInstance(float x, float y, float z);

    }
}