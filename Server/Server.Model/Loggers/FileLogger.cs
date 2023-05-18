using System;
using System.Diagnostics;
using System.IO;
using Server.Model.Interfaces;

namespace Server.Model.Loggers
{
    public class FileLogger<TClass> : ILogger<TClass>
    {
        private readonly string _path;

        public FileLogger(string filename, string basePath = null)
        {
            if (basePath is null)
            {
                basePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            }

            Debug.Assert(basePath != null, nameof(basePath) + " != null");
            var folderPath = Path.Combine(basePath, "Rilevazioni");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            
            var path = Path.Combine(folderPath, filename);

            _path = path;
            
            Console.WriteLine(_path);
        }

        public void Log(string info)
        {
            using (var sw = new StreamWriter(_path, true))
            {
                sw.WriteLine(info);
            }
        }
    }
}