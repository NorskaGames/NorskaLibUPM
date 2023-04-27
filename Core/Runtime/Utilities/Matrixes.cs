using System.Collections;
using System.Collections.Generic;

namespace NorskaLib
{
    public struct Matrix
    {
        public static T[,] Flip<T>(T[,] original, bool flipRows = true, bool flipColumns = false)
        {
            var I = original.GetLength(0);
            var J = original.GetLength(1);

            var result = new T[I, J];

            for (int i = I - 1; i >= 0; i--)
                for (int j = J - 1; j >= 0; j--)
                {
                    var r = flipRows
                        ? I - 1 - i
                        : i;
                    var c = flipColumns
                        ? J - 1 - j
                        : j;

                    result[i, j] = original[r, c];
                }

            return result;
        }

        // TO DO:
        public static T[,] Turn<T>(T[,] original, bool clockwise = true, bool full = false)
        {
            var I = original.GetLength(0);
            var J = original.GetLength(1);

            var result = new T[J, I];

            for (int i = 0; i < I; i++)
                for (int j = 0; j < J; j++)
                    result[j, i] = original[I - 1 - i, j];

            return result;
        }
    }
}