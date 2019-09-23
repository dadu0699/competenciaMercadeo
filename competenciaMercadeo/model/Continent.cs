using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace competenciaMercadeo.model
{
    class Continent
    {
        private string name;
        private int population;
        private List<Country> countries;
        private int saturation;

        public Continent(string name, int population, List<Country> countries, int saturation)
        {
            this.name = name;
            this.population = population;
            this.countries = countries;
            this.saturation = saturation;
        }

        public string Name { get => name; set => name = value; }
        public int Population { get => population; set => population = value; }
        public int Saturation { get => saturation; set => saturation = value; }
        internal List<Country> Countries { get => countries; set => countries = value; }
    }
}
