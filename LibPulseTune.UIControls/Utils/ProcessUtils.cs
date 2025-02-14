using System.Diagnostics;

namespace LibPulseTune.UIControls.Utils
{
    public static class ProcessUtils
    {
        /// <summary>
        /// 指定されたパスのファイルを選択して、Windowsのエクスプローラを起動する。
        /// </summary>
        /// <param name="path"></param>
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
