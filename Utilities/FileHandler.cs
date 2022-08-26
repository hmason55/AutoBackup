using AutoBackup.Models;
using System.Text.RegularExpressions;

namespace AutoBackup.Utilities;

public class FileHandler
{
    private static Regex _dateRegex = new("^.*[0-9]{8}-[0-9]{6}.*$");
    
    public static bool Backup(List<string> filePaths)
    {
        foreach (string path in filePaths)
        {
            string date = DateTime.Now.ToString(Config.DATE_FORMAT);

            string directory = Path.GetDirectoryName(path);
            if(directory == null)
            {
                continue;
            }

            string destination = Path.Combine(directory, $"{Path.GetFileNameWithoutExtension(path)}-{date}{Path.GetExtension(path)}").ReplaceSeparators();
            Console.WriteLine($"Creating backup file {destination}");
            File.Copy(path, destination);
        }

        return true;
    }

    public static bool Clean(List<string> filePaths, int history)
    {
        foreach(string path in filePaths)
        {
            string directory = Path.GetDirectoryName(path);
            if(directory == null)
            { 
                continue;
            }

            string[] fileHistory = Directory.GetFiles(directory);
            List<string> files = fileHistory.Where(p => _dateRegex.IsMatch(p)).ToList();
            files.Sort();
            files.Reverse();

            List<string> filesToDelete = files.Except(files.Take(history)).ToList();
            foreach(string file in filesToDelete)
            {
                Console.WriteLine($"Cleaning stale backup file {file}");
                File.Delete(file);
            }
        }

        return true;
    }
}
