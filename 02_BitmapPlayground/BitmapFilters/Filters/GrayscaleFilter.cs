using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _02_BitmapPlayground.Filters
{
    /// <summary>
    /// Filters the red component from an image.
    /// </summary>
    public class GrayscaleFilter : IFilter
    {
        public Color[,] Apply(Color[,] input)
        {
            int width = input.GetLength(0);
            int height = input.GetLength(1);
            Color[,] result = new Color[width, height];


            var t = new Thread(() => LoopOnPixels(input, 0, width / 2, height, result));
            var t1 = new Thread(() => LoopOnPixels(input, width / 2 - 1, width, height, result));
            t.Start();
            t1.Start();
            t.Join();
            t1.Join();

            return result;
        }

        private static void LoopOnPixels(Color[,] input, int startWidth, int endWidth, int height, Color[,] result)
        {
            for (int x = startWidth; x < endWidth; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var p = input[x, y];
                    int avg = (p.R + p.G + p.B) / 3;
                    result[x, y] = Color.FromArgb(p.A, avg, avg, avg);
                }
            }
        }

        public string Name => "Gray Scale Filter component";

        public override string ToString()
            => Name;
    }
}
