using System.Collections.Generic;

namespace BeatGeneration
{
    public class TrainingSet
    {
             
        public static List<Measure> Measures { get; set; }
        public static List<int[]> Samples { get; set; }

        static TrainingSet()
        {
            CompileMeasures();
            CompileTrainingSet();
        }


        public static void CompileTrainingSet()
        {
            Samples = new List<int[]>();

            Samples.Add(new int[] { 1, 2, 3, 4 });
            Samples.Add(new int[] { 4, 5, 6, 7 });
            Samples.Add(new int[] { 7, 8, 9, 10 });
            Samples.Add(new int[] { 10, 11, 12, 13 });
            Samples.Add(new int[] { 13, 14, 15, 16 });
            Samples.Add(new int[] { 16, 17, 18, 19 });
            Samples.Add(new int[] { 19, 20, 21, 22 });
            Samples.Add(new int[] { 23, 24, 25, 26 });
            Samples.Add(new int[] { 26, 27, 16, 25 });
            Samples.Add(new int[] { 17, 15, 27, 8 });
            Samples.Add(new int[] { 18, 4, 9, 8 });
            Samples.Add(new int[] { 22, 21, 15, 12 });
            Samples.Add(new int[] { 12, 7, 1, 8 });
            Samples.Add(new int[] { 19, 25, 14, 15 });
            Samples.Add(new int[] { 23, 19, 11, 14 });
        }

        public static void CompileMeasures()
        {
            Measures = new List<Measure>();

//////////// OLD ///////////////////
             
            Measures.Add(new Measure.Builder()
                    .SetNumBeats(4)
                    .AddSound(1, new double[] { 1, 2, 3, 4 })
                    .AddSound(2, new double[] { 2, 4 })
                    .AddSound(3, new double[] { 1.5, 2.5, 3.5, 4.5 })
                    .Build()
            );

            Measures.Add(new Measure.Builder()
                    .SetNumBeats(4)
                    .AddSound(1, 1, 1.75, 2.25, 3)
                    .AddSound(2, 2, 4)
                    .AddSound(3, 1.5, 2.5, 3.5, 4.5)
                    .Build()
            );
             
            //Measure swingIntro = new Measure.Builder()
            //        .SetNumBeats(4)
            //        .AddSound(4, 1, 3)
            //        .AddSound(3, 2, 2.75, 4, 4.75)
            //        .Build();
            //Measures.Add(swingIntro);

            Measures.Add(new Measure.Builder()
                    .SetNumBeats(4)
                    .AddSound(5, 1, 2, 3)
                    .AddSound(6, 4)
                    .Build()
            );


            Measures.Add(new Measure.Builder()
                    .SetNumBeats(4)
                    .AddSound(1, new double[] { 1, 3, 3.5 })
                    .AddSound(2, 2, 4)
                    .Build()
            );

             
            Measures.Add(new Measure.Builder()
                   .SetNumBeats(4)
                   .AddSound(6, new double[] { 1, 1.5, 3, 3.5 })
                   .AddSound(2, 2, 4)
                   .Build()
            );

            Measures.Add(new Measure.Builder()
                   .SetNumBeats(4)
                   .AddSound(1, new double[] { 1, 2, 3, 4 })
                   .AddSound(2, 2, 4)
                   .AddSound(3, 1, 3)
                   .Build()
            );

            Measures.Add(new Measure.Builder()
                   .SetNumBeats(4)
                   .AddSound(1, new double[] { 1, 2, 3, 4 })
                   .AddSound(2, 2, 4)
                   .AddSound(5, 1, 1.5, 2, 2.5, 3, 3.5, 4, 4.5)
                   .Build()
            );

            Measures.Add(new Measure.Builder()
                   .SetNumBeats(4)
                   .AddSound(1, new double[] { 1, 2, 3 })
                   .AddSound(2, 2, 2.5)
                   .AddSound(3, 4)
                   .Build()
            );

            Measures.Add(new Measure.Builder()
                    .SetNumBeats(4)
                    .AddSound(1, new double[] { 1, 3 })
                    .AddSound(2, 2)
                    .AddSound(3, 3, 3.5, 4, 4.5)
                    .Build()
            );

            Measures.Add(new Measure.Builder()
                   .SetNumBeats(4)
                   .AddSound(1, 1, 3)
                   .AddSound(2, 2, 4)
                   .AddSound(3, 1, 3)
                   .AddSound(4, 2, 4)
                   .Build()
            );

             
            Measures.Add(new Measure.Builder()
                   .SetNumBeats(4)
                   .AddSound(1, 1, 1.75, 2.25, 3)
                   .AddSound(2, 2, 2.5, 4, 4.5)
                   .AddSound(3, 1, 3)
                   .AddSound(4, 2, 4)
                   .Build()
            );

            Measures.Add(new Measure.Builder()
                   .SetNumBeats(4)
                   .AddSound(1, 4, 4.75)
                   .AddSound(2, 4, 4.5)
                   .AddSound(3, 1, 2, 3, 4)
                   .AddSound(4, 1, 2, 3, 4)
                   .Build()
           );


            Measures.Add(new Measure.Builder()
                   .SetNumBeats(4)
                   .AddSound(5, 1, 2.25, 4, 4.5)
                   .AddSound(2, 2, 4)
                   .AddSound(6, 1, 3, 3.75)
                   .Build()
            );


            // Make this better
            Measures.Add(new Measure.Builder()
                   .SetNumBeats(4)
                   .AddSound(1, 1)
                   .AddSound(2, 3)
                   .AddSound(3, 1, 1.75, 2, 2.75, 3, 3.75, 4, 4.75)
                   .Build()
            );


            // And this
            Measures.Add(new Measure.Builder()
                   .SetNumBeats(4)
                   .AddSound(1, 1, 2)
                   .AddSound(2, 3)
                   .Build()

            );
            Measures.Add(new Measure.Builder()
                .SetNumBeats(4)
                .AddSound(7, 1, 3)
                .AddSound(6, 2, 4)
                .Build()

            );

            Measures.Add(new Measure.Builder()
                 .SetNumBeats(4)
                 .AddSound(7, 1, 3)
                 .AddSound(5, 2, 4)
                 .AddSound(3, 1.5, 2.5, 3.5, 4.5)
                 .Build()
            );

            /////////// NEW//////////////

            Measures.Add(new Measure.Builder()
                    .SetNumBeats(4)
                    .AddSound(1, new double[] { 1, 2, 3, 4 })
                    .AddSound(2, new double[] { 0.5, 1.5, 2.5, 3.5 })
                    .AddSound(3, new double[] { 0.5, 2, 3.5 })
                    .AddSound(4, new double[] { 2 })
                    .Build()
            );

            Measures.Add(new Measure.Builder()
                    .SetNumBeats(4)
                    .AddSound(1, new double[] { 0.5, 1, 1.5, 2, 2.5, 3, 3.5 })
                    .AddSound(2, new double[] { 1, 2, 3, 4 })
                    .AddSound(3, new double[] { 2, 4 })
                    .AddSound(4, new double[] { 1, 3 })
                    .Build()
            );

            Measures.Add(new Measure.Builder()
                    .SetNumBeats(4)
                    .AddSound(1, new double[] { 2, 3, 4 })
                    .AddSound(2, new double[] { 1, 2, 3 })
                    .AddSound(3, new double[] { 2, 4 })
                    .Build()
            );

            Measures.Add(new Measure.Builder()
                    .SetNumBeats(4)
                    .AddSound(1, new double[] { 3, 4 })
                    .AddSound(2, new double[] { 1, 2 })
                    .AddSound(3, new double[] { 1, 2, 3, 4 })
                    .Build()
            );

            Measures.Add(new Measure.Builder()
                    .SetNumBeats(4)
                    .AddSound(1, new double[] { 4 })
                    .AddSound(2, new double[] { 4 })
                    .AddSound(3, new double[] { 1, 2.5, 3 })
                    .AddSound(4, new double[] { 1, 2, 3.5 })
                    .Build()
            );

            Measures.Add(new Measure.Builder()
                    .SetNumBeats(4)
                    .AddSound(1, new double[] { 0.5, 4 })
                    .AddSound(2, new double[] { 1, 2, 3, })
                    .AddSound(3, new double[] { 2.5 })
                    .AddSound(4, new double[] { 1.5 })
                    .Build()
            );

            Measures.Add(new Measure.Builder()
                    .SetNumBeats(4)
                    .AddSound(1, new double[] { 1.5, 2, 2.5, 3 })
                    .AddSound(2, new double[] { 1, 4 })
                    .AddSound(3, new double[] { 3, 4 })
                    .Build()
            );

            Measures.Add(new Measure.Builder()
                    .SetNumBeats(4)
                    .AddSound(1, new double[] { 2, 4 })
                    .AddSound(2, new double[] { 0.5, 1.5, 2.5 })
                    .AddSound(3, new double[] { 3.5, 4 })
                    .Build()
            );

            Measures.Add(new Measure.Builder()
                    .SetNumBeats(4)
                    .AddSound(1, new double[] { 0.5, 1 })
                    .AddSound(2, new double[] { 0.5, 1 })
                    .AddSound(3, new double[] { 2, 3, 4 })
                    .Build()
            );

            Measures.Add(new Measure.Builder()
                    .SetNumBeats(4)
                    .AddSound(1, new double[] { 3, 3.5, 4 })
                    .AddSound(2, new double[] { 0.25, 4 })
                    .AddSound(3, new double[] { 1, 2, 3 })
                    .Build()
            );

            Measures.Add(new Measure.Builder()
                     .SetNumBeats(4)
                     .AddSound(1, new double[] { 1, 2, 3, 4 })
                     .AddSound(2, new double[] { 2, 4 })
                     .AddSound(3, new double[] { 1.5, 2.5, 3.5, 4.5 })
                     .Build()
            );
        }


    }
}
