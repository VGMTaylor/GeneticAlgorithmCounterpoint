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
    class FifthSpecies
    {
        public static Note[] cantusFirmus;
        public static List<string> countermelody = new List<string>();
        List<Note> notes = new List<Note>();
        public static List<int> barlengths = new List<int>();

        internal void GenerateMelody(Note[] cf)
        {
            barlengths = new List<int>();

            //Assign cantus firmus melody
            cantusFirmus = cf;

            //Melody constants - zero indexed
            const int melodyLength = 6;

            //Default GA constants
            const int populationSize = 20;
            const int elitePercentage = 33;
            const double crossoverRate = 0.5;
            const double mutationRate = 1;

            //Create Population
            var population = new Population();

            //Values for different note durations
            var wholenote = 1;
            var halfnote = 0.5;
            var quarternote = 0.25;
            var eigthnote = 0.125;

            //Get available notes
            var wholenotes = CreateWholeNotes().ToList();
            var halfenotes = CreateHalfNotes().ToList();
            var quarternotes = CreateQuarterNotes().ToList();
            var eighthnotes = CreateEigthNotes().ToList();

            notes.AddRange(wholenotes);
            notes.AddRange(halfenotes);
            notes.AddRange(quarternotes);
            notes.AddRange(eighthnotes);
            notes.AddRange(quarternotes);
            notes.AddRange(eighthnotes);

            bool correct = false;
            var chromosome = new Chromosome();
            var tempchromosome = new Chromosome();

            while (!correct)
            {
                for (var p = 0; p < populationSize; p++)
                {
                    correct = false;

                    var replacechromosome = new Chromosome();
                    chromosome = new Chromosome();

                    var shufflednotes = notes.OrderBy(a => Guid.NewGuid()).ToList();

                    foreach (var note in shufflednotes)
                    {
                        chromosome.Genes.Add(new Gene(note));
                    }

                    //Index
                    var i = 0;
                    double total = 0;

                    while (total < melodyLength)
                    {
                        i++;
                        tempchromosome = chromosome.DeepClone();
                        tempchromosome.Genes.RemoveRange(0, tempchromosome.Count - i);

                        total = 0;

                        var i2 = 0;

                        for (int index = 0; index < tempchromosome.Count; index++)
                        {
                            var note = ((Note)tempchromosome.Genes[index].ObjectValue);
                            total += note.Length;
                        }
                    }

                    if (total == melodyLength)
                    {
                        double j = 0;
                        bool complete = true;
                        var i2 = 0;
                        for (var note = 0; note < tempchromosome.Count; note++)
                        {
                            i2++;
                            var k = ((Note)tempchromosome.Genes[note].ObjectValue);
                            j += k.Length;

                            //Ensure each bar is made up of right notes 
                            if (j == 1)
                            {
                                j = 0;
                                barlengths.Add(i2);
                                i2 = 0;
                            }

                            if (j > 1)
                            {
                                complete = false;
                            }
                        }

                        if (complete == true)
                        {
                            int rangetoremove = i;
                            chromosome.Genes.RemoveRange(0, chromosome.Count - rangetoremove);
                            break;
                        }
                    }
                }

                Console.WriteLine("Population Size:" + population.Solutions.Count);
                population.Solutions.Add(tempchromosome);
                Console.WriteLine("Population Size:" + population.Solutions.Count);

                if (population.Solutions.Count == populationSize)
                    correct = true;
            }

            population.Solutions.Sort((a, b) => a.Count - b.Count);


            var maxrange = population.Solutions[population.Solutions.Count - 1].Count;
            for(int i = 0; i < population.PopulationSize - 1; i++)
            {
                if(population.Solutions[i].Count != maxrange)
                {
                    var diff = maxrange - population.Solutions[i].Count - 1;
                    for(int i2 = 1; i2 < diff; i2++)
                    {
                        population.Solutions[i].Add(new Gene("Null"));
                    }
                }
            }

            population.Solutions.Sort((a, b) => a.Count - b.Count);

            for (int i = 0; i < population.PopulationSize; i++)
            {
                if (population.Solutions[i].Count > population.Solutions[0].Count)
                {
                    while(population.Solutions[i].Count != population.Solutions[0].Count)
                    {
                        population.Solutions[i].Genes.Remove(population.Solutions[i].Genes[population.Solutions[i].Genes.Count - 1]);
                    }
                }

                if (population.Solutions[i].Count > population.Solutions[0].Count)
                {
                    while (population.Solutions[i].Count != population.Solutions[0].Count)
                    {
                        population.Solutions[i].Add(new Gene("Null"));
                    }
                }

            }

            population.EvaluateInParallel = true;
            population.ParentSelectionMethod = ParentSelectionMethod.TournamentSelection;

            //Initialise elite operator
            var elite = new Elite(elitePercentage);

            //Initialise crossover operator
            var crossover = new Crossover(crossoverRate,
                                            true,
                                            CrossoverType.SinglePoint,
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

            try
            {
                ga.Run(Terminate);
            }

            catch
            {
                Program.Initialise();
                Program.GenerateCounterpoint();
            }

            Console.Read();
        }

        static void ga_OnRunComplete(object sender, GaEventArgs e)
        {
            var fittest = e.Population.GetTop(1)[0];
            string s = null;
            string s2 = null;

            Console.WriteLine();
            foreach (var gene in fittest.Genes)
            {
                if(gene.ObjectValue != "Null")
                {
                    var i = ((Note)gene.ObjectValue);
                    int midinote = i.ToMIDI(i.NoteName);
                    s += midinote.ToString();
                }
            }

            foreach (var note in cantusFirmus)
            {
                var i = note;
                int midinote = i.ToMIDI(i.NoteName);
                s2 += midinote.ToString();
            }

            System.IO.File.WriteAllText(@"C:\\Users\\Jack Taylor\\Desktop\\GeneratedCM.txt", s);
            System.IO.File.WriteAllText(@"C:\\Users\\Jack Taylor\\Desktop\\Cantusfirmus.txt", s2);

            double totallength = 0;
            foreach (var gene in fittest.Genes)
            {
                if(gene.ObjectValue != "Null")
                {
                    Note temp = ((Note)gene.ObjectValue);
                    if (temp.Sharp == false)
                    {
                        if (totallength == 6)
                            break;

                        var text = "";

                        if ((((Note)gene.ObjectValue).Length == 1))
                            text = "Semibreve";
                        if ((((Note)gene.ObjectValue).Length == 0.5))
                            text = "Minim";
                        if ((((Note)gene.ObjectValue).Length == 1))
                            text = "Crotchet";
                        if ((((Note)gene.ObjectValue).Length == 0.5))
                            text = "Quaver";

                        Console.WriteLine((((Note)gene.ObjectValue).NoteName) + " - " + (((Note)gene.ObjectValue).Length));
                        totallength += ((Note)gene.ObjectValue).Length;
                    }

                    else
                        Console.WriteLine(((Note)gene.ObjectValue).NoteName + "#");
                }
            }

            Console.WriteLine();
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
            bool badleaps = false;
            bool badstart = false;
            int bar = 0;
            int thirds = 0;
            int sixths = 0;
            int leaps = 0;
            int largeleaps = 0;
            int octaveleaps = 0;
            int steps = 0;
            int wholenotes = 0;
            int halfnotes = 0;
            int quarternotes = 0;
            int eigthnotes = 0;

            //For readability - assign chromosome to new countermelody variable & add all invididual genes (notes) into list
            var countermelody = chromosome;
            List<int> harmonicintervals = new List<int>();
            List<Note> cmNotes = new List<Note>();
            List<int> cmNumericNotes = new List<int>();
            List<string> cmNoteNames = new List<string>();
            List<double> cmDurations = new List<double>();
            List<Note> cfNotes = new List<Note>();
            List<int> cfNumericNotes = new List<int>();
            List<string> cfNoteNames = new List<string>();
            List<int> individualbarlengths = new List<int>();
            int suspensions = 0;


            for (int i = 0; i < chromosome.Genes.Count; i++)
            {
                if (chromosome.Genes[i].ObjectValue == "Null")
                    break;

                cmNotes.Add((Note)chromosome.Genes[i].ObjectValue);
                cmNumericNotes.Add(((Note)chromosome.Genes[i].ObjectValue).GetNumericValue());
                cmNoteNames.Add(((Note)chromosome.Genes[i].ObjectValue).NoteName);
                cmDurations.Add(((Note)chromosome.Genes[i].ObjectValue).Length);
            }

            for (int i = 0; i < cantusFirmus.Length; i++)
            {
                cfNotes.Add(cantusFirmus[i]);
                cfNumericNotes.Add((cantusFirmus[i]).GetNumericValue());
                cfNoteNames.Add(cantusFirmus[i].NoteName);
            }

            //Work out melodic differences from the melodies
            List<int> cmIntervals = new List<int>();
            List<int> cfIntervals = new List<int>();
            for (int i = 0; i < (chromosome.Genes.Count - 1); i++)
            {
                if (chromosome.Genes[i].ObjectValue == "Null" || chromosome.Genes[i+1].ObjectValue == "Null")
                    break;

                cmIntervals.Add(cmNumericNotes[i] - cmNumericNotes[i + 1]);
            }

            for (int i = 0; i < cfNotes.Count - 1; i++)
            {
                if (chromosome.Genes[i].ObjectValue == "Null" || chromosome.Genes[i + 1].ObjectValue == "Null")
                    break;

                cfIntervals.Add(cfNumericNotes[i] - cfNumericNotes[i + 1]);
            }

            double j = 0;
            int k = 0;
            for (int i = 0; i < chromosome.Genes.Count; i++)
            {
                if (chromosome.Genes[i].ObjectValue == "Null")
                    break;

                j += cmNotes[i].Length;
                k++;

                if (j == 1)
                {
                    individualbarlengths.Add(k);
                    j = 0;
                    k = 0;
                }

            }

            int index = 0;
            double bartotal = 0;
            bool correctarrangement = true;

            foreach(var note in cmDurations)
            {
                if(bartotal < 1)
                {
                    harmonicintervals.Add(cfNumericNotes[0] - cmNumericNotes[index]);
                    bartotal += note;
                    if(bartotal > 1)
                        harmonicintervals.Add(cfNumericNotes[1] - cmNumericNotes[index]);
                    index++;
                }

                else if (bartotal < 2)
                {
                    harmonicintervals.Add(cfNumericNotes[1] - cmNumericNotes[index]);
                    bartotal += note;
                    if (bartotal > 2)
                        harmonicintervals.Add(cfNumericNotes[2] - cmNumericNotes[index]);
                    index++;
                }

                else if (bartotal < 3)
                {
                    harmonicintervals.Add(cfNumericNotes[2] - cmNumericNotes[index]);
                    bartotal += note;
                    if (bartotal > 3)
                        harmonicintervals.Add(cfNumericNotes[3] - cmNumericNotes[index]);
                    index++;
                }

                else if (bartotal < 4)
                {
                    harmonicintervals.Add(cfNumericNotes[3] - cmNumericNotes[index]);
                    bartotal += note;
                    if (bartotal > 4)
                        harmonicintervals.Add(cfNumericNotes[4] - cmNumericNotes[index]);
                    index++;
                }

                else if (bartotal < 5)
                {
                    harmonicintervals.Add(cfNumericNotes[4] - cmNumericNotes[index]);
                    bartotal += note;
                    if (bartotal > 5)
                        harmonicintervals.Add(cfNumericNotes[5] - cmNumericNotes[index]);
                    index++;
                }

                else if (bartotal < 6)
                {
                    harmonicintervals.Add(cfNumericNotes[5] - cmNumericNotes[index]);
                    bartotal += note;
                    index++;
                }
            }


            double runningrhythmtotal = 0;

            if (cmNotes.Count == harmonicintervals.Count && cmNotes.Count > 5)
            {


                //Ensure that the countermelody starts at a 5th (above) or octave (above/below)
                if (System.Math.Abs(harmonicintervals[0]) != 7)
                    score -= 0.05;

                //Ensure that the countermelody end at a 5th (above) or octave (above/below)
                if (System.Math.Abs(harmonicintervals[harmonicintervals.Count - 1]) != 7)
                    score -= 0.05;

                for (int i = 0; i < (cmNotes.Count - 1); i++)
                {
                    runningrhythmtotal += cmDurations[i];

                    //Truncate value to get decimal - 0 - strong beat
                    var beat = runningrhythmtotal - System.Math.Truncate(runningrhythmtotal);

                    if (cmDurations[i] == 1)
                        wholenotes++;
                    if (cmDurations[i] == 0.5)
                        halfnotes++;
                    if (cmDurations[i] == 0.25)
                        quarternotes++;
                    if (cmDurations[i] == 0.125)
                        quarternotes++;


                    if(i > 0)
                    {
                        //contrary motion end
                        int cfMotion = cfIntervals[cfIntervals.Count - 1];
                        int cmMotion = cmIntervals[cmIntervals.Count - 1];
                        if (!((cfMotion < 0 && cmMotion > 0) || (cfMotion > 0 && cmMotion < 0)))
                            score -= 0.05;
                        }

                    //Dissonances

                    //Check if any dissonant harmonies
                    if (System.Math.Abs(harmonicintervals[i]) == 1 || System.Math.Abs(harmonicintervals[i]) == 3 || System.Math.Abs(harmonicintervals[i]) == 6 || System.Math.Abs(harmonicintervals[i]) == 8 || System.Math.Abs(harmonicintervals[i]) == 10 || System.Math.Abs(harmonicintervals[i]) == 13)
                    {
                        if (i > 0)
                        {
                            //Strong beat
                            if (beat == 0)
                            {
                                //Check if is suspensions
                                if (cmNumericNotes[i - 1] == cmNumericNotes[i])
                                {
                                    if (!(System.Math.Abs(harmonicintervals[i + 1]) == 1 || System.Math.Abs(harmonicintervals[i + 1]) == 3 || System.Math.Abs(harmonicintervals[i + 1]) == 6 || System.Math.Abs(harmonicintervals[i + 1]) == 8 || System.Math.Abs(harmonicintervals[i + 1]) == 10 || System.Math.Abs(harmonicintervals[i + 1]) == 13))
                                    {
                                        suspensions++;
                                    }
                                }

                                else
                                {
                                    score -= 0.01;
                                }
                            }
                        }
                    }

                    //Check if any dissonant harmonies
                    if (System.Math.Abs(harmonicintervals[i]) == 1 || System.Math.Abs(harmonicintervals[i]) == 3 || System.Math.Abs(harmonicintervals[i]) == 6 || System.Math.Abs(harmonicintervals[i]) == 8 || System.Math.Abs(harmonicintervals[i]) == 10 || System.Math.Abs(harmonicintervals[i]) == 13)
                    {
                        if (i < cmIntervals.Count - 2)
                        {
                            //Make sure it resolves
                            if (System.Math.Abs(cmIntervals[i + 1]) != 1)
                            {
                                if (!(System.Math.Abs(harmonicintervals[i + 1]) == 1 || System.Math.Abs(harmonicintervals[i + 1]) == 3 || System.Math.Abs(harmonicintervals[i + 1]) == 6 || System.Math.Abs(harmonicintervals[i + 1]) == 8 || System.Math.Abs(harmonicintervals[i + 1]) == 10 || System.Math.Abs(harmonicintervals[i + 1]) == 13))
                                {
                                    score -= 0.01;
                                }
                            }
                        }
                    }

                    //No unisons besides start and end
                    if (i != 0 || i != cmNotes.Count - 1)
                    {
                        if (harmonicintervals[i] == 0)
                            score -= 0.01;
                    }

                    //No crossovers
                    if (harmonicintervals[0] > 0)
                    {
                        if (harmonicintervals[i] < 0)
                            score -= 0;
                    }

                    if (cmDurations[0] != 1)
                        score -= 0.01;

                    //No crossovers
                    if (harmonicintervals[0] < 0)
                    {
                        if (harmonicintervals[i] > 0)
                            score -= 0;
                    }

                    if (i < cmIntervals.Count)
                    {
                        //Count leaps
                        if (System.Math.Abs(cmIntervals[i]) > 4)
                            leaps++;
                        //Count octave leaps
                        if (System.Math.Abs(cmIntervals[i]) > 6)
                            octaveleaps++;
                        //Count large leaps
                        if (System.Math.Abs(cmIntervals[i]) > 7)
                            largeleaps++;
                        //Count step movements
                        if (System.Math.Abs(cmIntervals[i]) == 1)
                            steps++;
                    }

                    if (i < cmNotes.Count - 3)
                    {
                        //Avoid 3 consecutive same intervals
                        if (harmonicintervals[i] == harmonicintervals[i + 1] && harmonicintervals[i] == harmonicintervals[i + 2])
                            score -= 0.01;
                    }

                    if (i < cmNotes.Count - 2)
                    {
                        //Stop parallel fifths
                        if ((System.Math.Abs(harmonicintervals[i]) == 4) && (System.Math.Abs(harmonicintervals[i + 1]) == 4))
                            score -= 0.01;
                        //Stop parallel fifths
                        if ((System.Math.Abs(cmIntervals[i]) == 4) && (System.Math.Abs(cmIntervals[i + 1]) == 4))
                                score -= 0.01;
                        //Stop parallel octaves
                        if ((System.Math.Abs(harmonicintervals[i]) == 7) && (System.Math.Abs(harmonicintervals[i + 1]) == 7))
                            score -= 0.01;
                        //Stop parallel octaves
                        if ((System.Math.Abs(cmIntervals[i]) == 7) && (System.Math.Abs(cmIntervals[i + 1]) == 7))
                            score -= 0.01;

                        //Punish consecutive parallel thirds
                        if ((System.Math.Abs(harmonicintervals[i]) == 2 && System.Math.Abs(harmonicintervals[i + 1]) == 2))
                            thirds++;

                        //Punish consecutive parallel sisxths
                        if ((System.Math.Abs(harmonicintervals[i]) == 5 && System.Math.Abs(harmonicintervals[i + 1]) == 5))
                            sixths++;

                        //Stop parallel 4ths unless bass is triad
                        //if ((System.Math.Abs(harmonicintervals[i]) == 3) && (System.Math.Abs(cfIntervals[i + 1]) == 3))
                        //if(harmonicintervals[i] != 2)
                        //score -= 0.05;
                    }

                    //Ensure doesn't start with quick notes
                    if (cmNotes[0].Length != 1)
                    {
                        if (cmNotes[0].Length != 0.5)
                        {
                            badstart = true;
                        }
                    }


                    /// Avoid making successive same-direction leaps in the same voice unless they outline a triad. If they can't be avoided they should at least total less than an octave.
                    // As in first species) Leaps greater than a fifth should be compensated by stepwise movement in the opposite direction.
                    //Work out if any leaps jump and dont change direction unless outlines triad
                    if (cmIntervals[i] > 5 && i < cmIntervals.Count - 3)
                    {
                        if (chromosome.Genes[i].ObjectValue == "Null" || chromosome.Genes[i + 1].ObjectValue == "Null" || chromosome.Genes[i + 2].ObjectValue == "Null")
                            break;


                        if ((cmIntervals[i + 1]) > 0)
                        {
                            if (cmIntervals[i + 1] != 3)
                            {
                                if (cmIntervals[i + 2] != 3)
                                    score -= 0.01;
                            }
                        }
                    }

                    //Work out if any leaps jump and dont change direction unless outlines triad
                    if (cmIntervals[i] > -5 && i < cmIntervals.Count - 3)
                    {
                        if (chromosome.Genes[i].ObjectValue == "Null" || chromosome.Genes[i + 1].ObjectValue == "Null" || chromosome.Genes[i + 2].ObjectValue == "Null")
                            break;

                        if ((cmIntervals[i + 1]) > 0)
                        {
                            if (cmIntervals[i + 1] != -3)
                            {
                                if (cmIntervals[i + 2] != -3)
                                    score -= 0.01;
                            }
                        }
                    }

                    //Do not outline tritone in melody
                    if (i < cmIntervals.Count - 4)
                    {
                        if (cmIntervals[i] == 1)
                            if (cmIntervals[i + 1] == 1)
                                if (cmIntervals[i + 2] == 1)
                                    if (cmIntervals[i + 3] == 1)
                                        score -= 0.01;
                    }

                    //Ensure the melody is singable with range stay between 10 notes
                    List<int> orderedcmNotes = cmNumericNotes.OrderByDescending(c => c).ToList();
                    if (System.Math.Abs(orderedcmNotes[orderedcmNotes.Count - 1]) - (orderedcmNotes[0]) > 9)
                    {
                        score -= 0.01;
                    }

                    //Ensure not all same note
                    List<double> orderedcmRhythms = cmDurations.OrderByDescending(c => c).ToList();
                    if (System.Math.Abs(orderedcmRhythms[orderedcmNotes.Count - 1]) == (orderedcmRhythms[0]))
                    {
                        score -= 0.01;
                    }

                    var distinctpitches = orderedcmNotes.GroupBy(a => a);

                    foreach (var pitch in distinctpitches)
                    {
                        if (pitch.Count() > ((cmNotes.Count - 1) / 4))
                            score -= 0.1;
                    }

                    //Avoid same consecutive melodic intervals on same pitches
                    if (i < cmIntervals.Count - 3)
                        if (cmIntervals[i] == cmIntervals[i + 1] && cmIntervals[i] == cmIntervals[i + 2])
                            score -= 0.1;
                }

            }

            else
            {
                score = 0;
            }

            double totallength = 0;

            foreach (var length in cmDurations)
            {
                totallength += length;
            }

            var longnotes = (wholenotes + halfnotes);
            var shortnotes = (quarternotes + eigthnotes);

            if (shortnotes < 2)
                score -= 0.05;
            if (suspensions != 0)
                score -= 0;
            if (totallength != 6)
                score -= 1;
            if (badleaps)
                score -= 0.05;
            if (badstart)
                score -= 0.05;
            if (octaveleaps > 2)
                score -= 0.05;
            if (steps < cmNotes.Count / 3)
                score -= 0.05;

            if (score < 0)
                score = 0;
            if (score == 1)
            {
                Console.WriteLine("Suspensions: " + suspensions + "\n Bad Start: " + badstart + "\n Bad Leaps" + badleaps);
            }

            //Console.WriteLine(runningrhythmtotal);

            return score;
        }

        public static bool Terminate(Population population, int currentGeneration, long currentEvaluation)
        {
            return (population.GetTop(1)[0].Fitness >= 1 || currentGeneration > 1000);
        }

        private static IEnumerable<Note> CreateWholeNotes()
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

        private static IEnumerable<Note> CreateHalfNotes()
        {
            var notes = new List<Note>
            {
                new Note("C1", false, false, false, 0.5),
                new Note("D1", false, false, false, 0.5),
                new Note("E1", false, false, false, 0.5),
                new Note("F1", false, false, false, 0.5),
                new Note("G1", false, false, false, 0.5),
                new Note("A1", false, false, false, 0.5),
                new Note("B1", false, false, false, 0.5),
                new Note("C2", false, false, false, 0.5),
                new Note("D2", false, false, false, 0.5),
                new Note("E2", false, false, false, 0.5),
                new Note("F2", false, false, false, 0.5),
                new Note("G2", false, false, false, 0.5),
                new Note("A2", false, false, false, 0.5),
                new Note("B2", false, false, false, 0.5),
                new Note("C3", false, false, false, 0.5),
                new Note("D3", false, false, false, 0.5),
                new Note("E3", false, false, false, 0.5),
                new Note("F3", false, false, false, 0.5),
                new Note("G3", false, false, false, 0.5),
                new Note("A3", false, false, false, 0.5),
                new Note("B3", false, false, false, 0.5),
                new Note("C4", false, false, false, 0.5),
                new Note("D4", false, false, false, 0.5),
                new Note("E4", false, false, false, 0.5),
                new Note("F4", false, false, false, 0.5),
                new Note("G4", false, false, false, 0.5),
                new Note("A4", false, false, false, 0.5),
                new Note("B4", false, false, false, 0.5),
            };

            return notes;
        }

        private static IEnumerable<Note> CreateQuarterNotes()
        {
            var notes = new List<Note>
            {
                new Note("C1", false, false, false, 0.25),
                new Note("D1", false, false, false, 0.25),
                new Note("E1", false, false, false, 0.25),
                new Note("F1", false, false, false, 0.25),
                new Note("G1", false, false, false, 0.25),
                new Note("A1", false, false, false, 0.25),
                new Note("B1", false, false, false, 0.25),
                new Note("C2", false, false, false, 0.25),
                new Note("D2", false, false, false, 0.25),
                new Note("E2", false, false, false, 0.25),
                new Note("F2", false, false, false, 0.25),
                new Note("G2", false, false, false, 0.25),
                new Note("A2", false, false, false, 0.25),
                new Note("B2", false, false, false, 0.25),
                new Note("C3", false, false, false, 0.25),
                new Note("D3", false, false, false, 0.25),
                new Note("E3", false, false, false, 0.25),
                new Note("F3", false, false, false, 0.25),
                new Note("G3", false, false, false, 0.25),
                new Note("A3", false, false, false, 0.25),
                new Note("B3", false, false, false, 0.25),
                new Note("C4", false, false, false, 0.25),
                new Note("D4", false, false, false, 0.25),
                new Note("E4", false, false, false, 0.25),
                new Note("F4", false, false, false, 0.25),
                new Note("G4", false, false, false, 0.25),
                new Note("A4", false, false, false, 0.25),
                new Note("B4", false, false, false, 0.25),
            };

            return notes;
        }

        private static IEnumerable<Note> CreateEigthNotes()
        {
            var notes = new List<Note>()
            {
                new Note("D1", false, false, false, 0.125),
                new Note("E1", false, false, false, 0.125),
                new Note("F1", false, false, false, 0.125),
                new Note("G1", false, false, false, 0.125),
                new Note("A1", false, false, false, 0.125),
                new Note("B1", false, false, false, 0.125),
                new Note("C2", false, false, false, 0.125),
                new Note("D2", false, false, false, 0.125),
                new Note("E2", false, false, false, 0.125),
                new Note("F2", false, false, false, 0.125),
                new Note("G2", false, false, false, 0.125),
                new Note("A2", false, false, false, 0.125),
                new Note("B2", false, false, false, 0.125),
                new Note("C3", false, false, false, 0.125),
                new Note("D3", false, false, false, 0.125),
                new Note("E3", false, false, false, 0.125),
                new Note("F3", false, false, false, 0.125),
                new Note("G3", false, false, false, 0.125),
                new Note("A3", false, false, false, 0.125),
                new Note("B3", false, false, false, 0.125),
                new Note("C4", false, false, false, 0.125),
                new Note("D4", false, false, false, 0.125),
                new Note("E4", false, false, false, 0.125),
                new Note("F4", false, false, false, 0.125),
                new Note("G4", false, false, false, 0.125),
                new Note("A4", false, false, false, 0.125),
                new Note("B4", false, false, false, 0.125),
            };

            return notes;
        }
    }
}
