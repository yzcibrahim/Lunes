using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _04_FileConverter
{
    public static class FileConversionManager
    {
        static List<IFileConverter> KnownConverters=new List<IFileConverter> { };
        public static void InitConverters()
        {
            if (KnownConverters.Count <= 0)
            {
                KnownConverters.Add(new Jpg2PngConverter());
                KnownConverters.Add(new Png2jpgConverter());
            }

        }
        public static object Convert(object input, string inputFileFormat, string outputFileFormat)
        {
            InitConverters();
            IFileConverter converter = KnownConverters.FirstOrDefault(c => c.InputFileFormat == inputFileFormat && c.OutputFileFormat == outputFileFormat);
            if(converter==null)
            {
                throw new KeyNotFoundException($"No converter found from {inputFileFormat} to {outputFileFormat}");
            }
            return converter.Convert(input);
        }
    }
}
