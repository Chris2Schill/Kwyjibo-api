using System;
using System.Diagnostics;
using System.Text;
using System.IO;
using Logging;
using Database.Models;


//trim            command.Execute(@"{sox} {filepath} {filepath} silence 1 0.1 1% reverse silence 1 0.1 1% reverse");
//delay           command.Execute($"{sox} {filepath} pad {seconds} 0");
//concat            command.Execute($"{sox} {filepath1} -p pad {padLength} {padOffset} | {sox} - -m {filepath2} sox-output.wav ");

namespace Sox
{
    public partial class SoxUtils
    {
        private static string silentWav = @"C:\musicgroup\soundclips\silence.wav";
        private static string SoxPath = @"C:\Program Files (x86)\sox-14-4-2\sox.exe";

        private static void AppendSilence(string filepath, Decimal length)
        {
            
            string tempFile = filepath.Substring(0,filepath.Length-4)+"_temp.wav";

            Process sox = new Process();
            sox.StartInfo.FileName = SoxPath;
            sox.StartInfo.Arguments = string.Format("{0} {1} pad 0 {2}", filepath, tempFile, length);
            sox.StartInfo.CreateNoWindow = true;
            sox.StartInfo.RedirectStandardOutput = true;
            sox.StartInfo.RedirectStandardError = true;
            sox.StartInfo.UseShellExecute = false;
            sox.Start();
            sox.WaitForExit();
            File.Delete(filepath);
            File.Move(Path.GetFullPath(tempFile), filepath);
        }

        public static bool Construct(double[] beatPattern, SoundClipInfo[] sounds, string outputFilepath)
        {
            Logger.Log("Constructing song for " + outputFilepath);
            // Check if we have a valid output file
            if (outputFilepath.Equals("") || outputFilepath == null)
            {
                Logger.Log("Construct() failed. You must pass in a valid outputfile to the mix command.");
                throw new Exception("You must pass in a valid output file to the MixCommand");
            }

            // Create process
            string commandArgs = BuildSoxMergeCommandArgs(beatPattern, sounds, outputFilepath);
            Logger.Log(commandArgs);
            Process sox = new Process()
            {
                StartInfo =
                {
                    FileName = SoxPath,
                    Arguments = commandArgs,
                    CreateNoWindow = true,
                    UseShellExecute = false
                }
            };

            // Run process
            sox.Start();
            sox.WaitForExit();


            //MaximizeVolume(outputFilepath);
            return true;
        }

        private static string BuildSoxMergeCommandArgs(double[] beatPattern, SoundClipInfo[] sounds, string outputFilepath)
        {
            // We need the number of seconds to beat ratio to adjust the tempo
            double numMeasures = beatPattern[0];
            int beatsPerMinute = (int)beatPattern[1];
            double secondsPerBeat = 60.0 / beatsPerMinute;
            double secondsPerMeasure = secondsPerBeat * beatPattern[2];

            // Our initial guess at how long the song clip will be. It may end up being shorter or longer and
            // we will account for that after this loop
            double estimatedLength = numMeasures * secondsPerMeasure;

            // Build command args and get the length of the final clip
            StringBuilder command = new StringBuilder(String.Format("-m {0} ", silentWav));
            double maxLength = 0;
            Logger.Log("BeatPatternLength: " + beatPattern.Length + ", sounds.Length: " + sounds.Length);
            if (beatPattern.Length > 3 && sounds.Length != 0)
            {
                Logger.Log("MAX LENGTH LOOP");
                for (int i = 3; i < beatPattern.Length; i += 2)
                {

                    // Add this sound at its offset to the sox command
                    SoundClipInfo clip = sounds[(int)beatPattern[i] - 1];
                    double beatOffset = (beatPattern[i+1] - 1) * secondsPerBeat; // We subtract 1 because the beatPattern is not zero-indexed and we want sounds on beat 1 to play immediately
                    command.Append(String.Format("\"|sox {0} -p pad {1} \" ", clip.Filepath, beatOffset));

                    // We calculate the length of the clip after this addition and record it if it's longer than any others. 
                    double endLoc = beatOffset + Convert.ToDouble(clip.Length);
                    maxLength = endLoc > maxLength ? endLoc : maxLength;
                }
            }

            //Append silence to the end of file so that it can loop and stay on beat.
            Logger.Log("Max Length: " + maxLength);
            double lenClipShouldBe = 0;
            while (lenClipShouldBe < maxLength)
            {
                lenClipShouldBe += secondsPerMeasure;
            }
            Logger.Log("Length clip should be: " + lenClipShouldBe);
            command.Append(String.Format("\"|sox -n -r 44100 -c 1 -p trim 0.0 {0} \" ", lenClipShouldBe));


            return command.Append(" -r 44100 -b 16 " + outputFilepath + " gain -n").ToString();
        }


        public static Decimal GetClipLength(string filepath)
        {
            Decimal clipLength = 0;

            Process sox = new Process();
            sox.StartInfo.FileName = SoxPath;
            sox.StartInfo.Arguments = filepath + " -n stat";
            sox.StartInfo.CreateNoWindow = true;
            sox.StartInfo.RedirectStandardOutput = true;
            sox.StartInfo.RedirectStandardError = true;
            sox.StartInfo.UseShellExecute = false;

            try {

                sox.Start();
                sox.WaitForExit();
                string output = sox.StandardOutput.ReadToEnd();
                string error2 = sox.StandardError.ReadLine();
                string error = sox.StandardError.ReadLine();
                string[] tokens = error.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                clipLength = Convert.ToDecimal(tokens[2]);
            }catch(Exception e)
            {
                Logger.Log("GetClipLength() Failed. " + e.Message);
            }
            return clipLength;
        }
        
        public static bool MaximizeVolume(string filepath)
        {
            string temp = "temp.wav";
            double maxVolume = GetClipMaxVolume(filepath);

            // Max Volume and send to temp file
            Process sox = new Process()
            {
                StartInfo =
                {
                    FileName = SoxPath,
                    Arguments = String.Format("{0} {1} gain -n", filepath, temp),
                    CreateNoWindow = true,
                    UseShellExecute = false
                }
            };
            sox.Start();
            sox.WaitForExit();

            // Copy the temp back to original file location and delete temp
            sox.StartInfo.Arguments = String.Format("{0} {1}", temp, filepath);
            sox.Start();
            sox.WaitForExit();
            File.Delete(temp);

            return true;
        }

        public static string GenerateSilence(Decimal length)
        {
            string outputPath = @"C:/musicgroup/soundclips/temp_silence.wav";
            Process sox = new Process();
            sox.StartInfo.FileName = SoxPath;
            sox.StartInfo.Arguments = string.Format("-n -r 44100 -c 2 {0} trim 0.0 {1}", outputPath, length);
            sox.StartInfo.CreateNoWindow = true;
            sox.StartInfo.RedirectStandardOutput = true;
            sox.StartInfo.RedirectStandardError = true;
            sox.StartInfo.UseShellExecute = false;
            sox.Start();
            sox.WaitForExit();
            return outputPath;
        }

        public static void Concatatenate(string file1, string file2, string outputFile)
        {
            Process sox = new Process();
            sox.StartInfo.FileName = SoxPath;
            sox.StartInfo.Arguments = string.Format("{0} {1} {2}", file1, file2, outputFile);
            sox.StartInfo.CreateNoWindow = true;
            sox.StartInfo.RedirectStandardOutput = true;
            sox.StartInfo.RedirectStandardError = true;
            sox.StartInfo.UseShellExecute = false;
            sox.Start();
            sox.WaitForExit();
        }

        public static double GetClipMaxVolume(string filepath)
        {
            // Create process
            Process sox = new Process()
            {
                StartInfo =
                {
                    FileName = SoxPath,
                    Arguments = String.Format("{0} -n stat -v", filepath),
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    UseShellExecute = false
                }
            };
            sox.Start();

            // Get the max volume and wait for exit
            string error = sox.StandardError.ReadToEnd();
            double maxVolume = Convert.ToDouble(error);
            sox.WaitForExit();

            return maxVolume;
        }
    }

}
