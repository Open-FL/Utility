using System;

namespace Utility.Collections.Algorithms
{
    public static class Shuffling
    {

        private static readonly Random RandomSource = new Random();

        public static void FisherYates<T>(T[] array)
        {
            int j;
            for (int i = array.Length - 1; i > 0; i--)
            {
                j = RandomSource.Next(0, i + 1);
                Swap(array, i, j);
            }
        }

        public static void FisherYatesInverse<T>(T[] array)
        {
            int j;
            for (int i = 0; i < array.Length - 2; i++)
            {
                j = RandomSource.Next(i, array.Length);
                Swap(array, i, j);
            }
        }

        public static void Swap<T>(T[] array, int indexA, int indexB)
        {
            T tmp = array[indexA];
            array[indexA] = array[indexB];
            array[indexB] = tmp;
        }

    }
}