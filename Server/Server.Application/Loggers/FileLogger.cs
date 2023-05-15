using System.Diagnostics;
using System.IO;
using Server.Application.Interfaces;

namespace Server.Application.Loggers
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
            var path = Path.Combine(basePath, filename);

            _path = path;
        }

        public void Log(string info)
        {
            using (var sw = new StreamWriter(_path, true))
            {
                sw.WriteLine($"{typeof(TClass).Name}: {info}");
            }
        }
    }
}