using Newtonsoft.Json;
using SnitchCommon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp_testing_pooya
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //string jsonString = ReadDataBase_generic("Database");
            string path = 
                $"{Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName).FullName).FullName).FullName}" +
                $"{Path.DirectorySeparatorChar}Grasshopper" +
                $"{Path.DirectorySeparatorChar}Database.json";
            string json = string.Empty;

            using (StreamReader streamReader = new StreamReader(path, Encoding.UTF8))
            {
                json = streamReader.ReadToEnd();
            }

            Building databse = JsonConvert.DeserializeObject<Building>(json);


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
