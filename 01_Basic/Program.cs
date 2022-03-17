using System;
using System.Linq;

namespace _01_Basic
{
    class Program
    {
        static void Main(string[] args)
        {
            const int end = 100;

            Console.WriteLine("ascending");
            PrintAllNumbersAsc(1, end);

            Console.WriteLine("descending");
            PrintAllNumbersDesc(1, end);

            Console.WriteLine("SUM_A:" + Sum_A(end));
            Console.WriteLine("SUM_B:" + Sum_B(end));
            Console.WriteLine("SUM_V:" + Sum_C(end));
        }

        private static void PrintAllNumbersAsc(int start, int end)
        {
            for (int i = start; i <= end; i++)
            {
                Console.WriteLine(i);
            }
        }

        private static void PrintAllNumbersDesc(int start, int end)
        {
            for (int i = end; i >= start; i--)
            {
                Console.WriteLine(i);
            }
        }

        private static int Sum_A(int n)
        {
            int result = 0;
            for (int i = 1; i <= n; i++)
            {
                result += i;
            }

            return result;
        }
        private static int Sum_B(int n)
        {
            return n * (n + 1) / 2;
        }
        private static int Sum_C(int n)
        {
            return Enumerable.Range(1, n).Sum();
        }
    }
}
