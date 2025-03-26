using System.IO;

namespace kuro.Core
{
    public static class IOUtils
    {
        public static void WriteAllText(string? filePath, string content)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filePath))
                    return;
                var dir = Path.GetDirectoryName(filePath)!;
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                if (File.Exists(filePath))
                    File.Delete(filePath);

                using (var fs = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
                using (var sw = new StreamWriter(fs))
                {
                    sw.Write(content);
                }
            }
            catch
            {
                // do nothing
            }
        }
    }
}