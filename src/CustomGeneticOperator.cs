using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GAF;
using GAF.Operators;

namespace CountermelodyGenerator
{
    public class CustomGeneticOperator
    {
        public static Population newPopulation = new Population();
        public static Population initialPopulation = new Population();
        int probablity = 5;

        public CustomGeneticOperator(Population currentPopulation)
        {
            initialPopulation = currentPopulation;

            var currentNonElites = currentPopulation.GetNonElites();
            var currentElites = currentPopulation.GetElites();

            foreach(var melody in currentElites)
            {
                foreach(var note in melody.Genes)
                {
                    Random r = new Random();
                    var i = r.Next(0, 100);
                    if(i < probablity)
                    {
                        Random r2 = new Random();
                        var i2 = r.Next(1, 2);
                        if(i2 == 1)
                        {
                            ((Note)note.ObjectValue).Increment();
                        }
                        else
                        {
                            ((Note)note.ObjectValue).Increment();
                        }
                    }
                }
            }
        }
    }
}
