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
    public class MovingAvarageFilter : IFilter
    {
        public Color[,] Apply(Color[,] input)
        {
            int width = input.GetLength(0);
            int height = input.GetLength(1);
            Color[,] result = new Color[width, height];
            
            var t = new Thread(() => LoopOnPixels(input, 1, width/2, height, result));
            var t1 = new Thread(() => LoopOnPixels(input, width / 2-1, width, height, result));
            t.Start();
            t1.Start();
            t.Join();
            t1.Join();


            return result;
        }

        private static void LoopOnPixels(Color[,] input, int startWidth , int endWidth, int height, Color[,] result)
        {
            for (int x = startWidth; x < endWidth - 1; x++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    var p = input[x, y];
                    var pRight = input[x + 1, y];
                    var pLeft = input[x - 1, y];
                    var pAbove = input[x, y - 1];
                    var pBelow = input[x, y + 1];

                    var avgR = (pRight.R + pLeft.R + pAbove.R + pBelow.R) / 4;
                    var avgG = (pRight.G + pLeft.G + pAbove.G + pBelow.G) / 4;
                    var avgB = (pRight.B + pLeft.B + pAbove.B + pBelow.B) / 4;
                    result[x, y] = Color.FromArgb(p.A, avgR, avgG, avgB);
                }
            }
        }

        public string Name => "Moving Avarage Filter component";

        public override string ToString()
            => Name;
    }
}
