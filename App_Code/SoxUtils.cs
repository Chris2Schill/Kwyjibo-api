using System.Diagnostics;
using System.IO;
using Logging;
using System;

namespace Sox
{
    public partial class SoxUtils
    {
        private static string ExecutableFilePath = @"C:\Program Files (x86)\sox-14-4-2\sox.exe";

        public static void Trim(string filepath){

			string tempFile =  filepath.Substring(0, filepath.Length-4)+"_test.wav";
            Logger.Log("Trimming: " + filepath);
		
			Process sox = new Process();
            sox.StartInfo.FileName = @"C:\Program Files (x86)\sox-14-4-2\sox.exe";
            //sox.StartInfo.Arguments = String.Format("{0} {1} silence 1 0.1 1% reverse silence 1 0.1 1% reverse", filepath, tempFile);
            sox.StartInfo.Arguments = String.Format("{0} {1} silence 1 1 1% reverse silence 1 1 1% reverse", filepath, tempFile);
            sox.StartInfo.CreateNoWindow = true;
            sox.StartInfo.RedirectStandardOutput = true;
            sox.StartInfo.RedirectStandardError = true;
            sox.StartInfo.UseShellExecute = false;
            sox.Start();
            sox.WaitForExit();	
			File.Delete(filepath);
			File.Move(tempFile, filepath);
        }

        public static string Stats(string filepath){
            Process cmd = NewCommandPromptCommand(@"\C {ExecutableFilePath} {filepath} -e stat");
            cmd.Start();
            cmd.WaitForExit();
            return cmd.StandardOutput.ReadToEnd();
        }

        public static Decimal GetClipLength2(string filepath)
        {   
            Process sox = new Process();
            sox.StartInfo.FileName = @"C:\Program Files (x86)\sox-14-4-2\sox.exe";
            sox.StartInfo.Arguments = filepath + " -n stat";
            sox.StartInfo.CreateNoWindow = true;
            sox.StartInfo.RedirectStandardOutput = true;
            sox.StartInfo.RedirectStandardError = true;
            sox.StartInfo.UseShellExecute = false;
            sox.Start();
            sox.WaitForExit();
            string output = sox.StandardOutput.ReadToEnd();
            // Gets second line of sox stat output
            string error2 = sox.StandardError.ReadLine();
            string error = sox.StandardError.ReadLine();
            string[] tokens = error.Split(new char [0], StringSplitOptions.RemoveEmptyEntries);
            Decimal cliplength = Convert.ToDecimal(tokens[2]);
            return cliplength;
        }

        private static Process NewCommandPromptCommand(string command){
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.Arguments = command;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.UseShellExecute = false;
            return p;
        }


    }

}
