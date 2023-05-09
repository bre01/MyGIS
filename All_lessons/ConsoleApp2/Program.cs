using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string fileName = @"C:\Users\bre\Documents\Txt\a.txt";
            string content = "TextReader is the abstract base " +
            "class of StreamReader and StringReader, which read " +
            "characters from streams and strings, respectively." +

            "Create an instance of TextReader to open a text file " +
            "for reading a specified range of characters, or to " +
            "create a reader based on an existing stream." +

            "You can also use an instance of TextReader to read " +
            "text from a custom backing store using the same " +
            "APIs you would use for a string or a stream.";
            StreamWriter streamWriter = new StreamWriter(fileName);
            streamWriter.WriteLine((double)343.3243);
            streamWriter.WriteLine(typeof(double));


            streamWriter.Close();
            StreamReader reader = new StreamReader(fileName);
            Console.WriteLine(Convert.ToDouble(reader.ReadLine()));
            Console.WriteLine(Type.GetType(reader.ReadLine()));
            //string f = "    ";
            //string feature = "Layer Name" + f + "Xmin" + f + "Y min" + f + "X max" + f + "Y max" + "feature shape" + f + "features" + f + "fileds";
            //string feature_value = string.Format("{0,-35}{1,-20}", "hello", "what");
            //Console.WriteLine(feature_value);
        }
    }
}
