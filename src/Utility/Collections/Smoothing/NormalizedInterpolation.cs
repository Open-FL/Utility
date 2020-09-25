using System.Collections.Generic;
using System.Linq;

namespace Utility.Collections.Smoothing
{
    public static class NormalizedInterpolation
    {

        public static void ExampleInterpolatorUse()
        {
            Interpolator
                interpolator =
                    Interpolator.CreateInterpolator(
                                                    Slerp,
                                                    Arch2
                                                   ); //Creates interpolation (first Slerp T then use it to create arch2)
            interpolator =
                Interpolator.CreateInterpolator(
                                                Flip,
                                                Slerp //Normal Interpolation Delegate
                                               ,
                                                time => SmoothStart(
                                                                    time,
                                                                    1
                                                                   ) //Sometimes you need to use anonymous functions.(GOD BLESS C#)
                                               ); //Creates interpolation (first Slerp T then use it to plug a SmoothStartCurve)

            Interpolator test = Interpolator.CreateInterpolator(Flip, Flip);


            for (int i = 0; i < 10000; i++)
            {
                float t = i / 10000f;
                float ff = test.Interpolate(t);
            }

            test = Interpolator.CreateInterpolator(Flip);
            for (int i = 0; i < 10000; i++)
            {
                float t = i / 10000f;
                float hh = interpolator.Interpolate(t);
            }
        }

        public static float Slerp(float t)
        {
            return t * t;
        }

        public static float SmoothStart(float t, float smoothness = 1)
        {
            int pow = (int) smoothness;
            return Mix(SmoothStart(t, pow), SmoothStart(t, pow + 1), smoothness - pow);
        }

        public static float SmoothStop(float t, float smoothness = 1)
        {
            int pow = (int) smoothness;
            return Mix(SmoothStop(t, pow), SmoothStop(t, pow + 1), smoothness - pow);
        }

        public static float SmoothStart(float t, int smoothness = 1)
        {
            float ret = t;
            for (int i = 0; i < smoothness; i++)
            {
                ret *= t;
            }

            return ret;
        }

        public static float SmoothStop(float t, int smoothness = 1)
        {
            return Flip(SmoothStart(Flip(t), smoothness));
        }

        public static float SmoothStep(float t, int smoothnessStart = 1, int smoothnessStop = 1)
        {
            return SmoothStep(t, smoothnessStart, smoothnessStop);
        }

        public static float SmoothStep(float t, float smoothnessStart = 1, float smoothnessStop = 1)
        {
            return Mix(SmoothStart(t, smoothnessStart), SmoothStop(t, smoothnessStop), t);
        }

        public static float Mix(float a, float b, float weightB)
        {
            return a + weightB * (b - a);
        }

        public static float Arch2(float t)
        {
            return (1 - t) * t;
        }

        public static float SmoothStopArch3(float t)
        {
            return Scale(Arch2(t), 1 - t);
        }

        public static float NormalizedBezier(float b, float c, float t)
        {
            float s = 1f - t;
            float t2 = t * t;
            float s2 = s * s;
            float t3 = t2 * t;
            return 3f * b * s2 * t + 3f * c * s * t2 + t3;
        }

        public static float SmoothStepArch4(float t)
        {
            return Scale(SmoothStartArch3(t), 1 - t);
        }

        public static float BellCurve(float t, float smoothness = 1)
        {
            return SmoothStart(t, smoothness) * SmoothStop(t, smoothness);
        }

        public static float SmoothStartArch3(float t)
        {
            return Scale(Arch2(t), t);
        }

        public static float Flip(float t)
        {
            return 1 - t;
        }

        public static float Scale(float t, float scale)
        {
            return t * scale;
        }

        public class Interpolator
        {

            public delegate float InterpolatingDelegate(float time);

            private List<InterpolatingDelegate> interpolations;


            private Interpolator()
            {
                interpolations = new List<InterpolatingDelegate>();
            }

            public float Interpolate(float t)
            {
                float ret = t;
                foreach (InterpolatingDelegate iDel in interpolations)
                {
                    ret = iDel(ret);
                }

                return ret;
            }

            public static Interpolator CreateInterpolator(params InterpolatingDelegate[] interpolations)
            {
                Interpolator ret = new Interpolator { interpolations = interpolations.ToList() };
                return ret;
            }

        }

    }
}