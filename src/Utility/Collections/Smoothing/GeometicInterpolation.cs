using System.Collections.Generic;
using System.Linq;

using Utility.Collections.Interfaces;

namespace Utility.Collections.Smoothing
{
    public static class GeometicInterpolation
    {

        public static IVec3 Lerp(IVec3 from, IVec3 to, float t)
        {
            IVec3 dir = VectorMath.Subtract(to, from);

            return VectorMath.Add(from, VectorMath.Scale(dir, t));
        }


        /// <summary>
        /// A really fast line smoothing algorithm that works for n dimensions
        /// </summary>
        /// <param name="pts"></param>
        /// <param name="smoothness"></param>
        /// <returns></returns>
        public static List<IVec3> Chaikin(List<IVec3> pts, int smoothness)
        {
            if (smoothness < 1)
            {
                return pts;
            }

            List<IVec3> ret = pts;
            for (int i = 0; i < smoothness; i++)
            {
                ret = Chaikin(ret);
            }

            return ret;
        }


        private static List<IVec3> Chaikin(List<IVec3> pts)
        {
            IVec3[] newPts = new IVec3[(pts.Count - 2) * 2 + 2];
            newPts[0] = pts[0];
            newPts[newPts.Length - 1] = pts[pts.Count - 1];

            int j = 1;
            for (int i = 0; i < pts.Count - 2; i++)
            {
                newPts[j] = VectorMath.Add(pts[i], VectorMath.Scale(VectorMath.Subtract(pts[i + 1], pts[i]), 0.75f));
                newPts[j + 1] = VectorMath.Add(
                                               pts[i + 1],
                                               VectorMath.Scale(VectorMath.Subtract(pts[i + 2], pts[i + 1]), 0.25f)
                                              );
                j += 2;
            }

            return newPts.ToList();
        }

    }
}