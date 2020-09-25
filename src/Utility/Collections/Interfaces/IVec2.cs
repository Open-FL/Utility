namespace Utility.Collections.Interfaces
{
    public interface IVec2
    {

        float X { get; set; }

        float Y { get; set; }

        IVec2 GetNewInstance(float x, float y);

    }
}