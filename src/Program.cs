using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GAF;
using GAF.Extensions;
using GAF.Operators;
using WMPLib;
using System.Threading;

namespace CountermelodyGenerator
{
    class Program
    {
        public static Note[] counterMelody;
        public static Note[] cantusFirmus;
        public static Note[] cantusFirmus2;
        public static int selectedSpecies;
        public static List<Note> optimalCountermelodies;

        static void Main(string[] args)
        {
            //Initialise and Create Cantus Firmus
            Initialise();

            //Generate species counterpoint
            GenerateCounterpoint();
        }

        public static void GenerateCounterpoint()
        {
            if(selectedSpecies == 1)
            {
                FirstSpecies firstSpeciesCounterpoint = new FirstSpecies();
                firstSpeciesCounterpoint.GenerateMelody(cantusFirmus2);
            }

            else if(selectedSpecies == 2)
            {
                SecondSpecies secondSpeciesCounterpoint = new SecondSpecies();
                secondSpeciesCounterpoint.GenerateMelody(cantusFirmus2);
            }

            else if(selectedSpecies == 3)
            {
                ThirdSpecies thirdSpeciesCounterpoint = new ThirdSpecies();
                thirdSpeciesCounterpoint.GenerateMelody(cantusFirmus);
            }

            else if (selectedSpecies == 4)
            {
                FourthSpecies fourthSpeciesCounterpoint = new FourthSpecies();
                fourthSpeciesCounterpoint.GenerateMelody(cantusFirmus2);
            }


            else if (selectedSpecies == 5)
            {
                FifthSpecies fifhtSpeciesCounterpoint = new FifthSpecies();
                fifhtSpeciesCounterpoint.GenerateMelody(cantusFirmus);
            }

            else
            {
                Console.WriteLine("\nPlease type a number in the range of 1-5");
            }
        }

        public static void Initialise()
        {
            Console.WriteLine("This is the Species Countpoint Generator!\n");
            Console.WriteLine("This program aims to create different levels of species counterpoint from a given cantus firmus using techniques from Fux's Gradus ad Parnassum\n");
            Console.WriteLine("Select which respective species counterpoint using the number keys 1-5:");

            try {
                selectedSpecies = Convert.ToInt32(Console.ReadLine());
            }
            catch (Exception e)
            {
                Console.WriteLine(e + "\nIncorrect selection, type a number 1-5");
                selectedSpecies = 0;
            }

            if(selectedSpecies != 0)
                CreateSpecies(selectedSpecies);
            else
                Initialise();

            Console.WriteLine("\n");
            //MusicPlayer player = new MusicPlayer();
            //player.PlayMelody(cantusFirmus);
            Console.WriteLine("--------------------------------------");

        }

        public static void Play()
        {
            Console.WriteLine("Would you like to hear this generated countermelody? (Y/N)");
            Console.Read();
            WindowsMediaPlayer wmp = new WindowsMediaPlayer();
            wmp.URL = @"C:\Users\Jack Taylor\Desktop\major-scale.mid";
            wmp.controls.play();
        }

        public static void CreateSpecies(int i)
        {
            Console.Clear();
            Console.Write("Species {0} counterpoint:", i);
            Console.WriteLine("\nGenerated Cantus Firmus:");

            //if (i == 1)
            //{
                CantusFirmus cf = new CantusFirmus();
                cantusFirmus = cf.GenerateCantusFirmus();
                Console.Write(" Note: \t\t");
                for(int j = 0; j < cantusFirmus.Count(); j++)
                {
                    Console.Write(cantusFirmus[j].NoteName + ", ");
                    cantusFirmus[j].Length = 1;
                    cantusFirmus[j].Flat = false;
                    cantusFirmus[j].Sharp = false;
                    cantusFirmus[j].Tied = false;
                }

                Console.Write("\n Leng: \t\t");

                for(int j = 0; j < cantusFirmus.Count(); j++)
                {
                    Console.Write("0" + cantusFirmus[j].Length + ", ");
                }

                Console.Write("\n Flat: \t\t");

                for(int j = 0; j < cantusFirmus.Count(); j++)
                {
                    if(cantusFirmus[j].Flat == false)
                        Console.Write("00" + ", ");
                    else
                        Console.Write("01" + ", ");
                }

                Console.Write("\n Shar: \t\t");

                for(int j = 0; j < cantusFirmus.Count(); j++)
                {
                    if(cantusFirmus[j].Sharp == false)
                        Console.Write("00" + ", ");
                    else
                        Console.Write("01" + ", ");
                }

                Console.Write("\n Tied: \t\t");

                for(int j = 0; j < cantusFirmus.Count(); j++)
                {
                    if(cantusFirmus[j].Tied == false)
                        Console.Write("00" + ", ");
                    else
                        Console.Write("01" + ", ");
                }

                Console.Write("\n");

            CantusFirmus2 cf2 = new CantusFirmus2();
            cantusFirmus2 = cf2.GenerateCantusFirmus();
            Console.Write(" Note: \t\t");
            for (int j = 0; j < cantusFirmus2.Count(); j++)
            {
                Console.Write(cantusFirmus2[j].NoteName + ", ");
                cantusFirmus2[j].Length = 1;
                cantusFirmus2[j].Flat = false;
                cantusFirmus2[j].Sharp = false;
                cantusFirmus2[j].Tied = false;
            }

            Console.Write("\n Leng: \t\t");

            for (int j = 0; j < cantusFirmus2.Count(); j++)
            {
                Console.Write("0" + cantusFirmus2[j].Length + ", ");
            }

            Console.Write("\n Flat: \t\t");

            for (int j = 0; j < cantusFirmus2.Count(); j++)
            {
                if (cantusFirmus2[j].Flat == false)
                    Console.Write("00" + ", ");
                else
                    Console.Write("01" + ", ");
            }

            Console.Write("\n Shar: \t\t");

            for (int j = 0; j < cantusFirmus2.Count(); j++)
            {
                if (cantusFirmus2[j].Sharp == false)
                    Console.Write("00" + ", ");
                else
                    Console.Write("01" + ", ");
            }

            Console.Write("\n Tied: \t\t");

            for (int j = 0; j < cantusFirmus2.Count(); j++)
            {
                if (cantusFirmus2[j].Tied == false)
                    Console.Write("00" + ", ");
                else
                    Console.Write("01" + ", ");
            }

            Console.Write("\n");
        }
    }
}
