using System;
using System.Collections.Generic;
using System.Text;

namespace BeatGeneration
{
    public class BeatPatternBuilder
    {
        private int beatsPerMinute { get; set; }
        private int timeSignature { get; set; }
        private List<Measure> measures = null;

        protected BeatPatternBuilder()
        {
            this.beatsPerMinute = 0;
            this.timeSignature = 0;
            measures = new List<Measure>();
        }

        public BeatPatternBuilder SetBeatsPerMinute(int bpm)
        {
            this.beatsPerMinute = bpm;
            return this;
        }

        public BeatPatternBuilder SetTimeSignature(int beatsPerMeasure)
        {
            this.timeSignature = beatsPerMeasure;
            return this;
        }

        public static BeatPatternBuilder NewPattern()
        {
            return new BeatPatternBuilder();
        }

        public double[] Build()
        {
            // Make sure we have all the data we need.
            if (beatsPerMinute <= 0 || measures == null)
            {
                throw new Exception("Invalid beat pattern parameters");
            }

            // Create pattern array and fill in header
            double[] pattern = new double[3 + (GetTotalNumSounds()*2)];
            pattern[0] = measures.Count;
            pattern[1] = beatsPerMinute;
            pattern[2] = timeSignature;

            // Fill in the rest
            int i = 3;
            for (int m = 0; m < measures.Count; m++)
            {
                foreach (Tuple<int, double> pair in measures[m].addedSounds)
                {
                    pattern[i] = pair.Item1;
                    pattern[i+1] = pair.Item2 + (m*timeSignature);
                    i += 2;
                }
            }
            return pattern;
        }

        private int GetTotalNumSounds()
        {
            int total = 0;
            foreach (Measure m in measures)
            {
                total += m.GetNumSounds();
            }
            return total;
        }

        public BeatPatternBuilder AppendMeasure(Measure m)
        {
            measures.Add(m);
            return this;             
        }

        
    }

    public class Measure
    {
        int numBeats;
        public List<Tuple<int, double>> addedSounds;

        private Measure(Builder builder)
        {
            this.numBeats = builder.numBeats;
            this.addedSounds = builder.addedSounds;
        }

        public int GetNumSounds()
        {
            return addedSounds.Count;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("{ ");
            sb.Append(numBeats);
            foreach (Tuple<int, double> sound in addedSounds)
            {
                sb.Append(", " + sound.Item1 + ", " + sound.Item2);
            }
            sb.Append(" }");
            return sb.ToString();
        }

        public class Builder
        {
            public int numBeats { get; set; }
            public List<Tuple<int, double>> addedSounds = new List<Tuple<int, double>>();

            public Measure Build()
            {
                if (numBeats <= 0)
                {
                    throw new Exception("Number of beats in a measure must be positive.");
                }else
                {
                    return new Measure(this);
                }
            }

            public Builder SetNumBeats(int beats)
            {
                this.numBeats = beats;
                return this;
            }

            public Builder AddSound(int soundId, params double[] beatOffsets)
            {
                for (int i = 0; i < beatOffsets.Length; i++)
                {
                    addedSounds.Add(new Tuple<int, double>(soundId, beatOffsets[i]));
                }
                return this;
            }

        }

    }
}
