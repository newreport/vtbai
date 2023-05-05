namespace CommonHelper
{
    public class FileHelper
    {
        public static void CreatePath(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static bool CreateFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                using (FileStream fs = new FileStream(fileName, FileMode.CreateNew))
                {
                }
            }
            return true;
        }
    }
}
