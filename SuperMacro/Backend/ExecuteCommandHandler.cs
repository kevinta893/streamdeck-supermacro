using BarRaider.SdTools;
using System;
using System.Diagnostics;
using System.Text;

namespace SuperMacro.Backend
{
    public static class ExecuteCommandHandler
    {
        /// <summary>
        /// Tells windows to execute a process with the given command
        /// in a Windows command prompt enviroment. Command is executed asynchronously and 
        /// cmd hidden window is closed.
        /// </summary>
        /// <param name="startCommand">The program/process with args(optional) to start</param>
        public static void HandleExecuteCmdProcess(string startCommand)
        {
            // Setup process
            var startInfo = new ProcessStartInfo()
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                FileName = "cmd.exe",
                Arguments = $"/C {startCommand}",
                RedirectStandardError = true,
                RedirectStandardOutput = true,
            };

            var process = new Process()
            {
                StartInfo = startInfo,
            };

            // Start
            try
            {
                process.Start();
            }
            catch (Exception ex)
            {
                var stdErrorStream = new StringBuilder();
                while (!process.HasExited)
                {
                    stdErrorStream.Append(process.StandardOutput.ReadToEnd());
                }
                stdErrorStream.Append(process.StandardError.ReadToEnd());
                Logger.Instance.LogMessage(TracingLevel.ERROR, $"Failed to execute macro with command: {startCommand}.\nProcess Error log:\n{stdErrorStream.ToString()}\nError Details: {ex}");
            }
        }
    }
}
