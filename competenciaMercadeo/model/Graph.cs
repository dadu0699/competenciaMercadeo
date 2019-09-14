using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace competenciaMercadeo.model
{
    class Graph
    {
        private string name;
        private List<Continent> continents;

        public Graph(string name, List<Continent> continents)
        {
            this.name = name;
            this.continents = continents;
        }

        public string Name { get => name; set => name = value; }
        internal List<Continent> Continents { get => continents; set => continents = value; }
    }
}
