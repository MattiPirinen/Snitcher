using Newtonsoft.Json;
using SnitchCommon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp_testing_pooya
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string jsonString = ReadDataBase_generic("Database");
            Building databse = JsonConvert.DeserializeObject<Building>(jsonString);


            Building building = new Building();

        }


        private static string ReadDataBase_generic(string dbName)
        {
            string text = "";

            string path = GetResourcePath(dbName);

            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream(path))
            using (StreamReader reader = new StreamReader(stream))
            {
                text = reader.ReadToEnd();
            }

            return text;
        }

        private static string GetResourcePath(string keyword)
        {
            string path = "";

            List<string> resrourcePaths = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames().ToList();

            path = resrourcePaths.First(rp => rp.Contains(keyword));

            return path;
        }

    }
}
