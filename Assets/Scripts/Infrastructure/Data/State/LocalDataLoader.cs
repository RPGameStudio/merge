using System.IO;
using System.Threading.Tasks;

namespace Data
{
    internal class LocalDataStorage : DataStorage
    {
        public LocalDataStorage(string path) : base(path)
        {
        }

        public override async Task<string> Load()
        {
            if (!File.Exists(Path))
            {
                return null;
            }

            return File.ReadAllText(Path);
        }

        public override async Task Save(string content) => File.WriteAllText(Path, content);
    }
}