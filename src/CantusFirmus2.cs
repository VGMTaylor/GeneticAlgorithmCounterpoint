using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountermelodyGenerator
{
    class CantusFirmus2
    {
        public static List<Note[]> CFList = new List<Note[]>();

        //Open list of cantus firmus'
        public Note[] GenerateCantusFirmus()
        {
            //Open file that holds CF 
            const string f = "C:\\Users\\Jack Taylor\\Documents\\CFList.txt";
            List<string> lines = new List<string>();
            List<Note> notes = new List<Note>();

            using(StreamReader r = new StreamReader(f))
            {
                string line;
                while((line = r.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }

            //For each string split into composite notes
            foreach(string s in lines)
            {
                char[] c = s.ToCharArray();
                //Console.WriteLine(s);

                for(int i = 0; i <= c.Length; i++)
                {
                    if(i % 2 != 0 || i == c.Length)
                    {
                        if(notes.Count <= 10)
                        {
                            char c1, c2;

                            c2 = c[i];
                            c1 = c[i - 1];
                            string s2 = c1.ToString() + c2.ToString();
                            Note note = new Note(s2);
                            notes.Add(new Note(note.NoteName));
                        }

                        else
                        {
                            Note[] cf = notes.ToArray();
                            CFList.Add(cf);
                            notes.Clear();
                        }
                    }
                }
            }

            Note[] selectedCF = CFList[0];
            return selectedCF;
        }
    }
}
