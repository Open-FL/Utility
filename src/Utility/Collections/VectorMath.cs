using System;

using Utility.Collections.Interfaces;

namespace Utility.Collections
{
    public static class VectorMath
    {

        #region Vec3

        public static IVec3 Add(params IVec3[] vecs)
        {
            if (vecs.Length == 0)
            {
                return null;
            }

            IVec3 ret = vecs[0].GetNewInstance(0, 0, 0);
            for (int i = 0; i < vecs.Length; i++)
            {
                ret.X += vecs[i].X;
                ret.Y += vecs[i].Y;
                ret.Z += vecs[i].Z;
            }

            return ret;
        }


        public static IVec3 Subtract(IVec3 first, params IVec3[] vecs)
        {
            if (vecs.Length == 0)
            {
                return first;
            }

            IVec3 vec = first.GetNewInstance(first.X, first.Y, first.Z);
            for (int i = 0; i < vecs.Length; i++)
            {
                vec.X -= vecs[i].X;
                vec.Y -= vecs[i].Y;
                vec.Z -= vecs[i].Z;
            }

            return vec;
        }

        public static float GetDistance(IVec3 vec1, IVec3 vec2)
        {
            IVec3 dir = Subtract(vec1, vec2);
            return GetLength(dir);
        }

        public static float GetLength(IVec3 vec)
        {
            return (float) Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y + vec.Z * vec.Z);
        }

        public static IVec3 Scale(IVec3 vec, float scalar)
        {
            return vec.GetNewInstance(vec.X * scalar, vec.Y * scalar, vec.Z * scalar);
        }

        public static IVec3 Scale(IVec3 vec, IVec3 scalar)
        {
            return vec.GetNewInstance(vec.X * scalar.X, vec.Y * scalar.Y, vec.Z * scalar.Z);
        }

        public static IVec3 Normalized(IVec3 vec)
        {
            float dist = GetLength(vec);
            return vec.GetNewInstance(vec.X / dist, vec.Y / dist, vec.Z / dist);
        }

        public static IVec3 Dot(IVec3 left, IVec3 right)
        {
            return left.GetNewInstance(left.X * right.X, left.Y * right.Y, left.Z * right.Z);
        }

        #endregion

        #region Vec2

        public static IVec2 Add(params IVec2[] vecs)
        {
            if (vecs.Length == 0)
            {
                return null;
            }

            IVec2 ret = vecs[0].GetNewInstance(0, 0);
            for (int i = 0; i < vecs.Length; i++)
            {
                ret.X += vecs[i].X;
                ret.Y += vecs[i].Y;
            }

            return ret;
        }


        public static IVec2 Subtract(IVec2 first, params IVec2[] vecs)
        {
            if (vecs.Length == 0)
            {
                return first;
            }

            IVec2 vec = first.GetNewInstance(first.X, first.Y);
            for (int i = 0; i < vecs.Length; i++)
            {
                vec.X -= vecs[i].X;
                vec.Y -= vecs[i].Y;
            }

            return vec;
        }

        public static float GetDistance(IVec2 vec1, IVec2 vec2)
        {
            IVec2 dir = Subtract(vec1, vec2);
            return GetLength(dir);
        }

        public static float GetLength(IVec2 vec)
        {
            return (float) Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y);
        }

        public static IVec2 Scale(IVec2 vec, float scalar)
        {
            return vec.GetNewInstance(vec.X * scalar, vec.Y * scalar);
        }

        public static IVec2 Scale(IVec2 vec, IVec2 scalar)
        {
            return vec.GetNewInstance(vec.X * scalar.X, vec.Y * scalar.Y);
        }

        public static IVec2 Normalized(IVec2 vec)
        {
            float dist = GetLength(vec);
            return vec.GetNewInstance(vec.X / dist, vec.Y / dist);
        }

        public static IVec2 Dot(IVec2 left, IVec2 right)
        {
            return left.GetNewInstance(left.X * right.X, left.Y * right.Y);
        }

        #endregion

    }
}