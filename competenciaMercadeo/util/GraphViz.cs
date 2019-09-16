using competenciaMercadeo.model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace competenciaMercadeo.util
{
    class GraphViz
    {
        private string route;
        private StringBuilder graph;

        public GraphViz()
        {
            route = Directory.GetCurrentDirectory();
        }

        private void generateDotPNG(string rdot, string rpng)
        {
            File.WriteAllText(rdot, graph.ToString());
            string dotCommand = "dot.exe -Tpng " + rdot + " -o " + rpng;
            Console.WriteLine(dotCommand);

            var command = string.Format(dotCommand);
            var startProcess = new ProcessStartInfo("cmd", "/C" + command);
            var process = new Process();

            process.StartInfo = startProcess;
            process.Start();
            process.WaitForExit();
        }

        private void generateDotPDF(string rdot, string rpdf)
        {
            File.WriteAllText(rdot, graph.ToString());
            string dotCommand = "dot.exe -Tpdf " + rdot + " -o " + rpdf;
            // Console.WriteLine(dotCommand);

            var command = string.Format(dotCommand);
            var startProcess = new ProcessStartInfo("cmd", "/C" + command);
            var process = new Process();

            process.StartInfo = startProcess;
            process.Start();
            process.WaitForExit();
        }

        public void generateGraphViz(Graph graph)
        {
            this.graph = new StringBuilder();
            string rdot = route + "\\" + graph.Name.Replace(" ", "") + ".dot";
            string rpng = route + "\\" + graph.Name.Replace(" ", "") + ".png";
            string rpdf = route + "\\" + graph.Name.Replace(" ", "") + ".pdf";

            this.graph.Append("digraph G {");
            this.graph.Append("\nstart [shape=Mdiamond label=\"" + graph.Name + "\"];");
            foreach (Continent continent in graph.Continents)
            {
                this.graph.Append("\n\n\tstart -> " + continent.Name.Replace(" ", "") + ";");
                this.graph.Append("\n\t" + continent.Name.Replace(" ", "") + " [shape=record label=\"{" + continent.Name + "|"
                    + continent.Saturation + "}\"style=filled fillcolor=" + getColor(continent.Saturation) + "];");

                Console.WriteLine(continent.Name + "----" + continent.Saturation);
                foreach (Country country in continent.Countries)
                {
                    this.graph.Append("\n\t" + continent.Name.Replace(" ", "") + " -> " + country.Name.Replace(" ", "") + ";");
                    this.graph.Append("\n\t" + country.Name.Replace(" ", "") + " [shape=record label=\"{" + country.Name + "|"
                        + country.Saturation + "}\"style=filled fillcolor=" + getColor(country.Saturation) + "];");
                    Console.WriteLine("\t" + country.Name + "----" + country.Saturation);
                }
            }
            this.graph.Append("\n}");

            generateDotPNG(rdot, rpng);
            generateDotPDF(rdot, rpdf);
        }

        private string getColor(int saturation)
        {
            string color = "";
            if (saturation >= 0 && saturation <= 15)
            {
                color = "white";
            }
            else if (saturation >= 16 && saturation <= 30)
            {
                color = "blue";
            }
            else if (saturation >= 31 && saturation <= 45)
            {
                color = "green";
            }
            else if (saturation >= 46 && saturation <= 60)
            {
                color = "yellow";
            }
            else if (saturation >= 61 && saturation <= 75)
            {
                color = "orange";
            }
            else if (saturation >= 76 && saturation <= 100)
            {
                color = "red";
            }
            return color;
        }

        private void deleteFile(string pathFile)
        {
            try
            {  
                if (File.Exists(pathFile))
                {  
                    File.Delete(pathFile);
                    Console.WriteLine("File deleted");
                }
                else Console.WriteLine("File not found");
            }
            catch (IOException ioExp)
            {
                Console.WriteLine(ioExp.Message);
            }
        }
    }
}
