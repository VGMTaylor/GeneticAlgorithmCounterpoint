using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using System.Diagnostics;
using System.Threading;
using NAudio.Wave;
using WMPLib;

namespace CountermelodyGenerator
{
    public class MusicPlayer
    {
        static Note[] cantus1;
        static Note[] counter1;

        internal void PlayMelody(Note[] melody)
        {
            
            for (int i = 0; i < melody.Length; i++)
            {
                string s = "H:\\Notes\\" + melody[i].NoteName + ".wav";
                WMPLib.WindowsMediaPlayer wmp = new WMPLib.WindowsMediaPlayer();
                wmp.URL = s;
                try
                {
                    wmp.controls.play();
                }
                catch (COMException e)
                {
                    Console.WriteLine(e);
                }

                // Do something.
                for(int i2 = 0; i2 < 100; i2++)
                {
                    Thread.Sleep(20);
                }

                wmp.controls.stop();
                wmp.close();

                /*SoundPlayer player = new SoundPlayer(s);
                Stopwatch stopwatch = new Stopwatch();
                player.Play();
                stopwatch.Start();

                // Do something.
                for(int i2 = 0; i2 < 1000 ; i2++)
                {
                    Thread.Sleep(1);
                }

                // Stop timing.
                stopwatch.Stop();
                player.Stop();

                Console.WriteLine(melody[i].NoteName + "\tTime elapsed: {0}", stopwatch.Elapsed);*/

            }
        }

        internal void PlayMelodies(Note[] cantus, Note[] counter)
        {
            cantus1 = cantus;
            counter1 = counter;
            Task.Factory.StartNew(PlayCF);
            Task.Factory.StartNew(PlayCP);
        }

        private void PlayCF()
        {
            PlayMelody(cantus1);
        }

        private void PlayCP()
        {
            PlayMelody(counter1);
        }
    }


}