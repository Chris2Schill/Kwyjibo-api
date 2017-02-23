using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
//using SongConstruction;
using Logging;
using Database.Models;
using System.Configuration;

namespace BeatGeneration
{
    public class BeatGenerator
    {

        private int[][] trainingSet;
        public static string markovTableFilepath = "C:\\musicgroup\\markov.txt";
        public static double[,] markovTable; 

        static BeatGenerator()
        {
            markovTable = ReadMatrix(markovTableFilepath);
            markovTableFilepath = ConfigurationManager.AppSettings["MarkovTablePath"];
        }

        public double[] GenerateBeat(int bpm, int timeSignature, int numClips)
        {
            List<int> measureOrder = new List<int>();

            int alpha = 0, beta = 0, gamma = 0, delta = 0;

            int m = 0;
            while (m++ < 4)
            {
                Random rand = new Random();
                double probability = rand.NextDouble();

                int row = Convert.ToInt32(Math.Pow(TrainingSet.Measures.Count, 3.0)) * alpha
                    + (beta * Convert.ToInt32(Math.Pow(TrainingSet.Measures.Count, 2.0))
                    + (gamma * Convert.ToInt32(Math.Pow(TrainingSet.Measures.Count, 1.0))
                    + delta));

                for (int i = 0; i < TrainingSet.Measures.Count; i++)
                {
                    if (probability < markovTable[row, i])
                    {
                        measureOrder.Add(i);

                        alpha = beta;
                        beta = gamma;
                        gamma = delta;
                        delta = i;
                        break;
                    }
                }
            }

            BeatPatternBuilder pb = BeatPatternBuilder.NewPattern();
            pb.SetBeatsPerMinute(bpm);
            pb.SetTimeSignature(timeSignature);
            foreach (int id in measureOrder)
            {
                pb.AppendMeasure(TrainingSet.Measures[id]);
            }
            double[] beatPattern = pb.Build();
            AdjustBeatForStation(ref beatPattern, numClips);
            return beatPattern;
        }

        public void Train()
        {
            int alpha = 0, beta = 0, gamma = 0, delta = 0;
            int numMeasures = TrainingSet.Measures.Count;

            Random randNumGenerator = new Random();
            double randomNumber = randNumGenerator.NextDouble();


            int[,] heardTransition = new int[Convert.ToInt32(Math.Pow(numMeasures, 4.0)), numMeasures];
            markovTable = new double[Convert.ToInt32(Math.Pow(numMeasures, 4.0)), numMeasures];


            int[][] trainingSet = TrainingSet.Samples.ToArray();
            for (int ts = 0; ts < trainingSet.Count(); ts++)
            {
                for (int measure = 0; measure < trainingSet[ts].Count(); measure++)
                {
                    int row = Convert.ToInt32(Math.Pow(numMeasures, 3.0)) * alpha
                        + (beta * Convert.ToInt32(Math.Pow(numMeasures, 2.0))
                        + (gamma * Convert.ToInt32(Math.Pow(numMeasures, 1.0))
                        + delta));

                    int column = trainingSet[ts][measure];
                    heardTransition[row, column]++;

                    alpha = beta;
                    beta = gamma;
                    gamma = delta;
                    delta = column;
                    Debug.WriteLine(string.Format("Alpha: {0}, Beta: {1}, Gamma: {2}, Delta: {3}", alpha, beta, gamma, delta));

                }
                alpha = 0; beta = 0; gamma = 0; delta = 0;
            }

            for (int i = 0; i < heardTransition.GetLength(0); i++)
            {
                int j = 0;
                int totalInRow = 0;
                for (j = 0; j < heardTransition.GetLength(1); j++)
                {
                    totalInRow += heardTransition[i, j];
                }

                int totalForThisRowSoFar = 0;
                for (j = 0; j < heardTransition.GetLength(1); j++)
                {
                    totalForThisRowSoFar += heardTransition[i, j];
                    if (totalInRow != 0)
                    {
                        markovTable[i, j] = totalForThisRowSoFar / totalInRow;
                    }
                }
            }

            WriteMatrix(markovTableFilepath, markovTable);
        }

        public void printMatrix(double[,] array)
        {
            var rowCount = array.GetLength(0);
            var colCount = array.GetLength(1);
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    Debug.Write(String.Format("{0}\t", array[i,j]));
                }
                Debug.Write(String.Format(" | Row #: {0}", i));
                Debug.WriteLine("");
            }
        }

        public static void Main()
        {
            new BeatGenerator().printMatrix(markovTable);
            var beatGenerator = new BeatGenerator();
            beatGenerator.Train();
        }

        public static void WriteMatrix(string filepath, double[,] matrix)
        {
            if (File.Exists(filepath)) { File.Delete(filepath); }

            int rows = matrix.GetLength(0);
            int columns = matrix.GetLength(1);

            using (StreamWriter stream = File.AppendText(filepath))
            {
                // Write the dimensions (row 0)
                stream.WriteLine(string.Format("{0} {1}", rows, columns));

                // Write the data
                for (int row = 1; row < rows; row++)
                {
                    for (int col = 0; col < columns; col++)
                    {
                        stream.Write(matrix[row, col] + " ");
                    }
                    stream.WriteLine();
                }
            }
        }

        public static double[,] ReadMatrix(string filepath)
        {
            double[,] matrix = new double[1,1]; // We initialize just in case it fails. But it should never fail..
            if (File.Exists("C:\\musicgroup\\markov.txt"))
            {
                using (StreamReader reader = new StreamReader(filepath))
                {
                    // Get the dimensions of the matrix
                    string[] dimensions = reader.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    int totalRows = Convert.ToInt32(dimensions[0]);
                    int totalCols = Convert.ToInt32(dimensions[1]); 

                    // Initialize the matrix with num rows and cols
                    matrix = new double[totalRows, totalCols];

                    // Read the data
                    string line;
                    int row = 0, col = 0; // Current row and col
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] rowValues = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string s in rowValues)
                        {
                            matrix[row, col] = Convert.ToDouble(s);
                            col++;
                        }
                        row++;
                        col = 0;
                    }
                }
            }else
            {
                Debug.WriteLine("ReadMatrix() failed. File does not exist.");
            }
            return matrix;
        }

        public void AdjustBeatForStation(ref double[] beatPattern, int numClips)
        {
            //for (int i = 3; i < beatPattern.Count(); i += 2)
            //{
            //    if (beatPattern[i] > numClips)
            //    {
            //        int modVal = numClips - 1;
            //        if (modVal > 0)
            //        {
            //            beatPattern[i] = (beatPattern[i] % (numClips - 1)) + 1;
            //        }
            //        else
            //        {
            //            beatPattern[i] = 1;
            //        }
            //    }
            //    Debug.Write(string.Format("{0} ", beatPattern[i]));
            //}
            //Debug.WriteLine("");

            for (int i = 3; i < beatPattern.Count(); i += 2)
            {
                Random random = new Random();
                if (beatPattern[i] > numClips)
                {
                    int randClipId = random.Next(1, numClips + 1);
                    beatPattern[i] = randClipId;
                }
            }
        }
    }
}
