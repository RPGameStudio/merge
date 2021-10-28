using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Data
{
    public interface IDataStorage
    {
        Task<string> Load();
        Task Save(string content);
    }

    public abstract class DataStorage : IDataStorage
    {
        protected string Path { get; }

        public DataStorage(string path) => Path = path;

        public abstract Task<string> Load();
        public abstract Task Save(string content);
    }
}