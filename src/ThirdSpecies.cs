using GAF;
using GAF.Extensions;
using GAF.Operators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountermelodyGenerator
{
    class ThirdSpecies
    {
        public static Note[] cantusFirmus;

        internal void GenerateMelody(Note[] cf)
        {
            //Assign cantus firmus melody
            cantusFirmus = cf;

            //Melody constants - zero indexed
            const int melodyLength = 40;

            //Default GA constants
            const int populationSize = 500;
            const int elitePercentage = 10;
            const double crossoverRate = 0.9;
            const double mutationRate = 0.9;

            //Get available notes
            var notes = CreateNotes().ToList();

            //Create Population
            var population = new Population();

            for(var p = 0; p < populationSize; p++)
            {
                var chromosome = new Chromosome();
                foreach(var note in notes)
                {
                    chromosome.Genes.Add(new Gene(note));
                    chromosome.Genes.Add(new Gene(note));
                    chromosome.Genes.Add(new Gene(note));
                    chromosome.Genes.Add(new Gene(note));
                    chromosome.Genes.Add(new Gene(note));
                }

                //10 bars of 4 crotchets then ends with semibreve
                var rnd = GAF.Threading.RandomProvider.GetThreadRandom();
                chromosome.Genes.ShuffleFast(rnd);
                chromosome.Genes.RemoveRange(0, 120);

                population.Solutions.Add(chromosome);
            }


            population.EvaluateInParallel = false;
            population.ParentSelectionMethod = ParentSelectionMethod.StochasticUniversalSampling;
            population.LinearlyNormalised = true;

            //Initialise elite operator
            var elite = new Elite(elitePercentage);

            //Initialise crossover operator
            var crossover = new Crossover(crossoverRate,
                                            true,
                                            CrossoverType.DoublePoint,
                                            ReplacementMethod.GenerationalReplacement
                                          );

            //Initialise mutation operator
            var mutate = new SwapMutate(mutationRate);

            //Initialise Genetic Algorithm object
            var ga = new GeneticAlgorithm(population, CalculateFitness);

            //Subscribe events to genetic algorithm
            ga.OnGenerationComplete += ga_OnGenerationComplete;
            ga.OnRunComplete += ga_OnRunComplete;
            ga.Operators.Add(elite);
            ga.Operators.Add(crossover);
            ga.Operators.Add(mutate);

            ga.Run(Terminate);

            Console.Read();
        }

        static void ga_OnRunComplete(object sender, GaEventArgs e)
        {
            var fittest = e.Population.GetTop(1)[0];

            Console.WriteLine();

            string s = null;
            string s2 = null;

            foreach (var gene in fittest.Genes)
            {
                var i = ((Note)gene.ObjectValue);
                int midinote = i.ToMIDI(i.NoteName);
                s += midinote.ToString();
            }

            foreach (var note in cantusFirmus)
            {
                var i = note;
                int midinote = i.ToMIDI(i.NoteName);
                s2 += midinote.ToString();
            }

            System.IO.File.WriteAllText(@"C:\\Users\\Jack Taylor\\Desktop\\GeneratedCM.txt", s);
            System.IO.File.WriteAllText(@"C:\\Users\\Jack Taylor\\Desktop\\Cantusfirmus.txt", s2);

            foreach (var gene in fittest.Genes)
            {
                Note temp = ((Note)gene.ObjectValue);
                if(temp.Sharp == false)
                    Console.WriteLine(((Note)gene.ObjectValue).NoteName);
                else
                    Console.WriteLine(((Note)gene.ObjectValue).NoteName + "#");
            }

            Console.WriteLine();

            //Program.Play();
            Program.Initialise();
            Program.GenerateCounterpoint();
        }

        private static void ga_OnGenerationComplete(object sender, GaEventArgs e)
        {
            var fittest = e.Population.GetTop(1)[0];
            System.Threading.Thread.Sleep(1);
            Console.WriteLine("Generation: {0}, Fittest: {1}, Avg Fitness: {2} ", e.Generation, fittest.Fitness, e.Population.AverageFitness);
        }

        public static double CalculateFitness(Chromosome chromosome)
        {
            //Fitness score 
            double score = 1;
            int repeat_threshold = 12;
            int thirds_threshold = 2;
            int parallelthirds = 0;
            int thirds = 0;
            int sixths_threshold = 1;
            int sixths = 0;
            int fifths = 0;
            int octaves = 0;
            int leaps = 0;
            int parallels = 0;
            int steps = 0;
            int dissonantvalues = 0;
            int octaveleaps = 0;
            int largeleaps = 0;
            int unisons = 0;
            bool crossedover = false;
            bool weakleaps = false;

            int[] interval = new int[40];
            int intervalrepeats = 0;

            //Reward
            double reward = 0;

            //For readability - assign chromosome to new countermelody variable & add all invididual genes (notes) into list
            var countermelody = chromosome;
            List<Note> cmNotes = new List<Note>();
            List<int> cmNumericNotes = new List<int>();
            List<string> cmNoteNames = new List<string>();
            List<Note> cfNotes = new List<Note>();
            List<int> cfNumericNotes = new List<int>();
            List<string> cfNoteNames = new List<string>();
            for(int i = 0; i < chromosome.Genes.Count; i++)
            {
                cmNotes.Add((Note)chromosome.Genes[i].ObjectValue);
                cmNumericNotes.Add(((Note)chromosome.Genes[i].ObjectValue).GetNumericValue());
                cmNoteNames.Add(((Note)chromosome.Genes[i].ObjectValue).NoteName);
            }

            for(int i = 0; i < cantusFirmus.Length; i++)
            {
                cfNotes.Add(cantusFirmus[i]);
                cfNumericNotes.Add((cantusFirmus[i]).GetNumericValue());
                cfNoteNames.Add(cantusFirmus[i].NoteName);
            }

            //Work out vertical difference from the countermelody and cantus firmus
            List<int> harmonicintervals = new List<int>();
            for(int i = 0; i < cantusFirmus.Length; i++)
            {

                if(i == cantusFirmus.Length - 1)
                {
                    harmonicintervals.Add(cfNumericNotes[i] - cmNumericNotes[(i * 4) - 1]);
                }

                else
                {
                    harmonicintervals.Add(cfNumericNotes[i] - cmNumericNotes[(i * 4)]);
                    harmonicintervals.Add(cfNumericNotes[i] - cmNumericNotes[(i * 4) + 1]);
                    harmonicintervals.Add(cfNumericNotes[i] - cmNumericNotes[(i * 4) + 2]);
                    harmonicintervals.Add(cfNumericNotes[i] - cmNumericNotes[(i * 4) + 3]);
                }
            }

            //Work out melodic differences from the melodies
            List<int> cmIntervals = new List<int>();
            List<int> cfIntervals = new List<int>();
            for(int i = 0; i < (chromosome.Genes.Count - 1); i++)
            {
                cmIntervals.Add(cmNumericNotes[i] - cmNumericNotes[i + 1]);
            }
            for(int i = 0; i < (cantusFirmus.Length - 1); i++)
            {
                cfIntervals.Add(cmNumericNotes[i] - cfNumericNotes[i + 1]);
            }

            //Sort the numeric values of countermelody notes
            List<int> orderedcmNotes = cmNumericNotes.OrderByDescending(c => c).ToList();

            var distinctpitches = orderedcmNotes.GroupBy(i => i);

            foreach (var pitch in distinctpitches)
            {
                if (pitch.Count() > 5)
                    score -= 0.05;
            }

            if(orderedcmNotes[0] == orderedcmNotes[1])
                score -= 0.05;

            if(orderedcmNotes[1] == orderedcmNotes[2])
                score -= 0.05;

            //Ensure that the countermelody starts at a 5th (above) or octave (above/below)
            if (System.Math.Abs(harmonicintervals[0]) != 7 && harmonicintervals[0] != 4)
                score -= 0.05;

            //Ensure that the countermelody starts at a 5th (above) or octave (above/below)
            if (System.Math.Abs(harmonicintervals[harmonicintervals.Count - 1]) != 7)
                score -= 0.05;

            //Ensure that the penultimate note is stepwise onto final note
            if (System.Math.Abs(cmIntervals[cmIntervals.Count - 1]) != 1)
                score -= 0.05;

            //contrary motion end
            int cfMotion = cfIntervals[cfIntervals.Count - 1];
            int cmMotion = cmIntervals[cfIntervals.Count - 1];
            if(!((cfMotion < 0 && cmMotion > 0) || (cfMotion > 0 && cmMotion < 0)))
                score -= 0.05;

            //Ensure penultimate note isn't repeated
            if(System.Math.Abs(cmIntervals[cmIntervals.Count - 2]) == 0)
                score -= 0.05;
            if(System.Math.Abs(cmIntervals[cmIntervals.Count - 2]) > 2)
                score -= 0.05;


            //Check for note repeats
            for(int i = 0; i < chromosome.Genes.Count; i++)
            {
                //No crossover of melodies
                if(harmonicintervals[0] > 0)
                {
                    if (harmonicintervals[i] < 0)
                        score -= 0.005;
                }

                //No crossover of melodies
                if(harmonicintervals[0] < 0)
                {
                    if(harmonicintervals[i] > 0)
                        score -= 0.005;
                }

                if(i < cmIntervals.Count)
                {
                    //Count leaps
                    if(System.Math.Abs(cmIntervals[i]) > 4)
                        leaps++;
                    //Count octave leaps
                    if(System.Math.Abs(cmIntervals[i]) > 7)
                        octaveleaps++;
                    //Count large leaps
                    if(System.Math.Abs(cmIntervals[i]) > 9)
                        largeleaps++;
                    //Count step movements
                    if(System.Math.Abs(cmIntervals[i]) == 1)
                        steps++;
                    if(cmIntervals[i] == 0)
                        score -= 0.01;
                }

                //No harmonic unison
                if(harmonicintervals[i] == 0)
                    unisons++;

                //No intervals outside octave range
                if(harmonicintervals[i] > 11 || harmonicintervals[i] < -11)
                    score -= 0.01;

                //Count repeated notes in melody
                //List<string> distinctnotes = cmNoteNames.Distinct().ToList();
                //int repeats = (harmonicintervals.Count - distinctnotes.Count);
                //if(repeats > repeat_threshold)
                //score -= 0.01;


                //Check for consecutive parallels
                if (i > 0)
                {
                    if ((System.Math.Abs(harmonicintervals[i]) == 4 && System.Math.Abs(harmonicintervals[i - 1]) == 4))
                        parallels++;
                    if ((System.Math.Abs(harmonicintervals[i]) == 7 && System.Math.Abs(harmonicintervals[i - 1]) == 7))
                        parallels++;
                    if ((System.Math.Abs(harmonicintervals[i]) == 2 && System.Math.Abs(harmonicintervals[i - 1]) == 2))
                        parallels++;
                    if ((System.Math.Abs(harmonicintervals[i]) == 5 && System.Math.Abs(harmonicintervals[i - 1]) == 5))
                        parallelthirds++;

                    //Punish consecutive parallel thirds
                    if ((System.Math.Abs(harmonicintervals[i]) == 2))
                        thirds++;

                    //Punish consecutive parallel sisxths
                    if ((System.Math.Abs(harmonicintervals[i]) == 5))
                        sixths++;

                    //Punish consecutive parallel sisxths
                    if ((System.Math.Abs(harmonicintervals[i]) == 4))
                        fifths++;

                    //Punish consecutive parallel sisxths
                    if ((System.Math.Abs(harmonicintervals[i]) == 7))
                        octaves++;
                }

                //Check if any dissonant harmonies if so make sure its just a passing note
                if (System.Math.Abs(harmonicintervals[i]) == 1 || System.Math.Abs(harmonicintervals[i]) == 3 || System.Math.Abs(harmonicintervals[i]) == 6 || System.Math.Abs(harmonicintervals[i]) == 8
                    || System.Math.Abs(harmonicintervals[i]) == 10 || System.Math.Abs(harmonicintervals[i]) == 13)
                {
                    if (i < chromosome.Count - 1 && i > 1)
                    {
                        var diff = 0;

                        diff = cmNumericNotes[i - 1] - cmNumericNotes[i];



                        //If on a weak beat
                        if (i % 2 == 0)
                        {
                            if (harmonicintervals[i] == 0)
                                unisons--;
                                
                            if (System.Math.Abs(harmonicintervals[i]) == 1 || System.Math.Abs(harmonicintervals[i]) == 3 || System.Math.Abs(harmonicintervals[i]) == 6
                                || System.Math.Abs(harmonicintervals[i]) == 8)
                            {


                                //Passing note must follow same direction
                                if (diff > 0)
                                {
                                    if (System.Math.Abs(cmNumericNotes[i + 1] - cmNumericNotes[i]) != -1)
                                    {
                                        if (System.Math.Abs(harmonicintervals[i + 1]) == 1 || System.Math.Abs(harmonicintervals[i + 1]) == 3 || System.Math.Abs(harmonicintervals[i + 1]) == 6
                                            || System.Math.Abs(harmonicintervals[i + 1]) == 8 || System.Math.Abs(harmonicintervals[i + 1]) == 10 || System.Math.Abs(harmonicintervals[i + 1]) == 13)
                                            score -= 0.025;

                                        else
                                        {
                                            score += 0.0001;
                                            reward += 0.05;
                                        }
                                    }

                                    else
                                    {
                                        score -= 0.025;
                                    }
                                }

                                //Passing note must follow same direction
                                if (diff < 0)
                                {
                                    if (System.Math.Abs(cmNumericNotes[i + 1] - cmNumericNotes[i]) != 1)
                                    {
                                        if (System.Math.Abs(harmonicintervals[i + 1]) == 1 || System.Math.Abs(harmonicintervals[i + 1]) == 3 || System.Math.Abs(harmonicintervals[i + 1]) == 6
                                            || System.Math.Abs(harmonicintervals[i + 1]) == 8 || System.Math.Abs(harmonicintervals[i + 1]) == 10 || System.Math.Abs(harmonicintervals[i + 1]) == 13)
                                            score -= 0.025;


                                        else
                                        {
                                            score += 0.0001;
                                            reward += 0.05;
                                        }
                                    }

                                    else
                                    {
                                        score -= 0.025;
                                    }
                                }
                            }
                        }

                        //If on a strong beat
                        else
                        {

                            interval[intervalrepeats] = harmonicintervals[i];
                            intervalrepeats++;
                            if (intervalrepeats > 1)
                            {
                                //if consequtive same interval on strong beats
                                if (interval[intervalrepeats - 2] == interval[intervalrepeats - 1] && interval[intervalrepeats - 1] == interval[intervalrepeats])
                                    score -= 0.01;
                            }

                            if (System.Math.Abs(harmonicintervals[i]) == 1 || System.Math.Abs(harmonicintervals[i]) == 3 || System.Math.Abs(harmonicintervals[i]) == 6
                                || System.Math.Abs(harmonicintervals[i]) == 8)

                                dissonantvalues++;
                        }
                    }
                }

            }


            if (dissonantvalues > 0)
                score -= 0.25;
            if (parallels > 0)
                score -= 0.05;
            if (leaps > 4)
                score -= 0.05;
            if (octaveleaps > 1)
                score -= 0.05;
            if (largeleaps > 0)
                score -= 0.05;
            if (steps < 8)
            {
                score -= 0.2;
            }
            if (weakleaps == true)
                score -= 0.05;
            if (octaves > 1)
                score -= 0.05;
            if (fifths + octaves > thirds + sixths)
                score -= 0.05;
            if (unisons > 0)
                score -= 0.05;
            if (crossedover == true)
                score -= 0.05;


            //Count repeated notes in melody
            //List<string> distinctnotes = cmNoteNames.Distinct().ToList();
            //int repeats = (harmonicintervals.Count - distinctnotes.Count);
            //if(repeats > repeat_threshold)
            // score -= 0.1;

            if (score < 0)
                score = 0;
            else if (score > 1)
                score = 1;

            return score;

            return score;

        }

        public static bool Terminate(Population population, int currentGeneration, long currentEvaluation)
        {
            return (population.GetTop(1)[0].Fitness >= 1 || currentGeneration > 499);
        }

        private static IEnumerable<Note> CreateNotes()
        {
            var notes = new List<Note>
            {
                new Note("C1", false, false, false, 1),
                new Note("D1", false, false, false, 1),
                new Note("E1", false, false, false, 1),
                new Note("F1", false, false, false, 1),
                new Note("G1", false, false, false, 1),
                new Note("A1", false, false, false, 1),
                new Note("B1", false, false, false, 1),
                new Note("C2", false, false, false, 1),
                new Note("D2", false, false, false, 1),
                new Note("E2", false, false, false, 1),
                new Note("F2", false, false, false, 1),
                new Note("G2", false, false, false, 1),
                new Note("A2", false, false, false, 1),
                new Note("B2", false, false, false, 1),
                new Note("C3", false, false, false, 1),
                new Note("D3", false, false, false, 1),
                new Note("E3", false, false, false, 1),
                new Note("F3", false, false, false, 1),
                new Note("G3", false, false, false, 1),
                new Note("A3", false, false, false, 1),
                new Note("B3", false, false, false, 1),
                new Note("C4", false, false, false, 1),
                new Note("D4", false, false, false, 1),
                new Note("E4", false, false, false, 1),
                new Note("F4", false, false, false, 1),
                new Note("G4", false, false, false, 1),
                new Note("A4", false, false, false, 1),
                new Note("B4", false, false, false, 1),
            };

            return notes;
        }
    }
}
