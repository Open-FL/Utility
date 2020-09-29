namespace Utility.Collections.Algorithms
{
    public static class Analysis
    {

        /// <summary>
        ///     Finds the largest sub array in the give array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public static void KadanesAlgorithm(int[] array, out int start, out int end)
        {
            int s;
            int e = s = array[0];
            end = start = 0;
            int potentialNext;
            for (int i = 1; i < array.Length; i++)
            {
                potentialNext = e + array[i];
                if (array[i] > potentialNext)
                {
                    start = i;
                    e = array[i];
                }
                else
                {
                    e = potentialNext;
                }

                if (s < e)
                {
                    s = e;
                    end = i;
                }
            }
        }

        /// <summary>
        ///     Finds the largest sub array in the give array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public static void KadanesAlgorithm(float[] array, out int start, out int end)
        {
            float s;
            float e = s = array[0];
            end = start = 0;

            for (int i = 1; i < array.Length; i++)
            {
                if (array[i] > e + array[i])
                {
                    start = i;
                    e = array[i];
                }
                else
                {
                    e = array[i] + e;
                }

                if (s < e)
                {
                    s = e;
                    end = i;
                }
            }
        }

    }
}