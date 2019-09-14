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
        private List<Country> countries;
        private int saturation;

        public Continent(string name, List<Country> countries, int saturation)
        {
            this.name = name;
            this.countries = countries;
            this.saturation = saturation;
        }

        public string Name { get => name; set => name = value; }
        public int Saturation { get => saturation; set => saturation = value; }
        internal List<Country> Countries { get => countries; set => countries = value; }
    }
}
