using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _04_FileConverter
{
    public class Jpg2PngConverter : IFileConverter
    {
        public string InputFileFormat 
        { 
            get { return "jpg"; } 
        }

        public string OutputFileFormat
        {
            get { return "png"; }
        }
        public object Convert(object input)
        {
            Console.WriteLine("Png2jpgConverter executed");
            return null;
        }
    }
}
