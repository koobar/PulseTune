using System.Diagnostics;

namespace PulseTune.Utils
{
    internal static class ProcessUtils
    {
        public static void OpenInExplorer(string path)
        {
            var process = new ProcessStartInfo();
            process.UseShellExecute = true;
            process.FileName = "explorer.exe";
            process.Arguments = $"/select, \"{path}\"";

            Process.Start(process);
        }
    }
}
