using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace competenciaMercadeo.model
{
    class Country
    {
        private string name;
        private int population;
        private int saturation;
        private string flag;

        public Country(string name, int population, int saturation, string flag)
        {
            this.name = name;
            this.population = population;
            this.saturation = saturation;
            this.flag = flag;
        }

        public string Name { get => name; set => name = value; }
        public int Population { get => population; set => population = value; }
        public int Saturation { get => saturation; set => saturation = value; }
        public string Flag { get => flag; set => flag = value; }
    }
}
