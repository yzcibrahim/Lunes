﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _04_FileConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputFormat = "jpg";
            string outputFormat = "phng";
            FileConversionManager.Convert(new object(), inputFormat, outputFormat);
        }
    }
}
