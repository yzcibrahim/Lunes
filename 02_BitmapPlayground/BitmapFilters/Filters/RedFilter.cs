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
    public class RedFilter : IFilter
    {
        public Color[,] Apply(Color[,] input)
        {
            int width = input.GetLength(0);
            int height = input.GetLength(1);
            Color[,] result = new Color[width, height];

            var t = new Thread(() => LoopOnPixel(input, 0, width / 2, height, result));
            var t1 = new Thread(() => LoopOnPixel(input, width / 2, width, height, result));
            t.Start();
            t1.Start();
            t.Join();
            t.Join();

            return result;
        }

        private static void LoopOnPixel(Color[,] input,int startWidth, int endWidth, int height, Color[,] result)
        {
            for (int x = startWidth; x < endWidth; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var p = input[x, y];
                    result[x, y] = Color.FromArgb(p.A, 0, p.G, p.B);
                }
            }
        }

        public string Name => "Filter red component";

        public override string ToString()
            => Name;
    }
}
