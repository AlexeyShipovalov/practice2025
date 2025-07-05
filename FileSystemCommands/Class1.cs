using CommandLib;
using System.IO;

namespace FileSystemCommands
{
    public class DirectorySizeCommand : ICommand
    {
        private readonly string _path;
        public DirectorySizeCommand(string path)
        {
            _path = path;
        }
        public void Execute()
        {
            long size = 0;
            foreach (var file in Directory.GetFiles(_path))
            {
                size += new FileInfo(file).Length;
            }
        }
    }
    public class FindFilesCommand : ICommand
    {
        private readonly string _path;
        private readonly string _mask;
        public FindFilesCommand(string path, string mask)
        {
            _path = path;
            _mask = mask;
        }
        public void Execute()
        {
            var files = Directory.GetFiles(_path, _mask);
        }
    }
}