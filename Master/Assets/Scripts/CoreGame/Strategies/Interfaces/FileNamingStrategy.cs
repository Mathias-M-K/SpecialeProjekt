using System.IO;

namespace CoreGame.Strategies.Interfaces
{
    public interface FileNamingStrategy
    {
        string CreateFile(string directoryPath);
    }
}