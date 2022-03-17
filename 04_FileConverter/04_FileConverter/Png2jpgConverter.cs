using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _04_FileConverter
{
    public class Png2jpgConverter : IFileConverter
    {
        public string InputFileFormat 
        { 
            get { return "png"; } 
        }

        public string OutputFileFormat
        {
            get { return "jpg"; }
        }
        public object Convert(object input)
        {
            Console.WriteLine("Png2jpgConverter executed");
            return null;
        }
    }
}
