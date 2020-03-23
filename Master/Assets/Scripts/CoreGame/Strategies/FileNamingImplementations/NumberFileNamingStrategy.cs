using System.IO;
using CoreGame.Strategies.Interfaces;

namespace DefaultNamespace
{
    public class NumberBasedFileCreation : _FileNamingStrategy
    {
        public string CreateFile(string _directoryPath)
        {
            string filePath = null;
            int dataSetIndex = 0;
            bool uniqueNameFound = false;
            
            while (!uniqueNameFound)
            {
                filePath  = _directoryPath + "data" + dataSetIndex + ".csv";
            
                if (File.Exists(filePath))
                {
                    dataSetIndex++;
                }
                else
                {
                    uniqueNameFound = true;
                }
            }

            return filePath;
        }
    }
}