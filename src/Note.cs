using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountermelodyGenerator
{
    class Note
    {
        private string s2;

        public Note(string note, bool tied, bool sharp, bool flat, double length)
        {
            NoteName = note;
            Tied = tied;
            Sharp = sharp;
            Flat = flat;
            Length = length;
        }

        public Note(string s2)
        {
            NoteName = s2;
        }

        public Note(string note, bool tied, int length)
        {
            NoteName = note;
        }

        public Note(string note, int length)
        {
            NoteName = note;
        }

        public int GetNumericValue()
        {
            int i = NumericValue(NoteName);
            return i;
        }

        public void Sharpen()
        {
            if (!(this.NoteName.Contains("B") || this.NoteName.Contains("E")))
                this.Sharp = true;
        }

        internal void Increment()
        {
            var a = this.NoteName.ToString();
            var b = a[0];
            var c = a[1];

            int i = 0;
            if(a == "C1")
                a = "D1";
            if(a == "D1")
                a = "E1";
            if(a == "E1")
                a = "F1";
            if(a == "F1")
                a = "G1";
            if(a == "G1")
                a = "A1";
            if(a == "A1")
                a = "B1";
            if(a == "B1")
                a = "C2";
            if(a == "C2")
                a = "D2";
            if(a == "D2")
                a = "E2";
            if(a == "E2")
                a = "F2";
            if(a == "F2")
                a = "G2";
            if(a == "G2")
                a = "A2";
            if(a == "A2")
                a = "B2";
            if(a == "B2")
                a = "C3";
            if(a == "C3")
                a = "D3";
            if(a == "D3")
                a = "E3";
            if(a == "E3")
                a = "F3";
            if(a == "F3")
                a = "G3";
            if(a == "G3")
                a = "A3";
            if(a == "A3")
                a = "B3";
            if(a == "B3")
                a = "C4";
            if(a == "C4")
                a = "D4";
            if(a == "D4")
                a = "E4";
            if(a == "E4")
                a = "F4";
            if(a == "F4")
                a = "G4";
            if(a == "G4")
                a = "A4";
            if(a == "A4")
                a = "B4";
            if(a == "B4")
                a = "C5";
            if(a == "C5")
                a = "C1";

            this.NoteName = a;

        }

        internal void Decrement()
        {
            var a = this.NoteName.ToString();
            var b = a[0];
            var c = a[1];
            this.NoteName = b + c.ToString();
        }

        public void NormaliseFirstSpecies()
        {
            this.Length = 1;
            this.Tied = false;
            this.Sharp = false;
            this.Flat = false;
        }

        public int NumericValue(string NoteName)
        {

            int i = 0;
            if(NoteName == "C1")
                i = 1;
            if(NoteName == "D1")
                i = 2;
            if(NoteName == "E1")
                i = 3;
            if(NoteName == "F1")
                i = 4;
            if(NoteName == "G1")
                i = 5;
            if(NoteName == "A1")
                i = 6;
            if(NoteName == "B1")
                i = 7;
            if(NoteName == "C2")
                i = 8;
            if(NoteName == "D2")
                i = 9;
            if(NoteName == "E2")
                i = 10;
            if(NoteName == "F2")
                i = 11;
            if(NoteName == "G2")
                i = 12;
            if(NoteName == "A2")
                i = 13;
            if(NoteName == "B2")
                i = 14;
            if(NoteName == "C3")
                i = 15;
            if(NoteName == "D3")
                i = 16;
            if(NoteName == "E3")
                i = 17;
            if(NoteName == "F3")
                i = 18;
            if(NoteName == "G3")
                i = 19;
            if(NoteName == "A3")
                i = 20;
            if(NoteName == "B3")
                i = 21;
            if(NoteName == "C4")
                i = 22;
            if(NoteName == "D4")
                i = 23;
            if(NoteName == "E4")
                i = 24;
            if(NoteName == "F4")
                i = 25;
            if(NoteName == "G4")
                i = 26;
            if(NoteName == "A4")
                i = 27;
            if(NoteName == "B4")
                i = 28;
            if(NoteName == "C5")
                i = 29;
            return i;
        }

        public int ToMIDI(string NoteName)
        {

            int i = 0;
            if(NoteName == "C1")
                i = 24;
            if(NoteName == "D1")
                i = 26;
            if(NoteName == "E1")
                i = 28;
            if(NoteName == "F1")
                i = 29;
            if(NoteName == "G1")
                i = 31;
            if(NoteName == "A1")
                i = 33;
            if(NoteName == "B1")
                i = 35;
            if(NoteName == "C2")
                i = 36;
            if(NoteName == "D2")
                i = 38;
            if(NoteName == "E2")
                i = 40;
            if(NoteName == "F2")
                i = 41;
            if(NoteName == "G2")
                i = 43;
            if(NoteName == "A2")
                i = 45;
            if(NoteName == "B2")
                i = 47;
            if(NoteName == "C3")
                i = 48;
            if(NoteName == "D3")
                i = 50;
            if(NoteName == "E3")
                i = 52;
            if(NoteName == "F3")
                i = 53;
            if(NoteName == "G3")
                i = 55;
            if(NoteName == "A3")
                i = 57;
            if(NoteName == "B3")
                i = 59;
            if(NoteName == "C4")
                i = 60;
            if(NoteName == "D4")
                i = 62;
            if(NoteName == "E4")
                i = 64;
            if(NoteName == "F4")
                i = 65;
            if(NoteName == "G4")
                i = 67;
            if(NoteName == "A4")
                i = 69;
            if(NoteName == "B4")
                i = 71;
            if(NoteName == "C5")
                i = 72;
            return i;
        }

        public string NoteName { set; get; }
        public bool Tied { set; get; }
        public bool Sharp { set; get; }
        public bool Flat { set; get; }
        public double Length { set; get; }

    }
}
