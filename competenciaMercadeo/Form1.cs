using competenciaMercadeo.controller;
using competenciaMercadeo.model;
using competenciaMercadeo.util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace competenciaMercadeo
{
    public partial class Form1 : Form
    {
        private int countTab;
        private Graph graph;

        public Form1()
        {
            InitializeComponent();
            countTab = 1;
            graph = null;
        }

        private void AnalyzeButton_Click(object sender, EventArgs e)
        {
            LexicalAnalyzer lexicalAnalyzer = new LexicalAnalyzer();
            HTMLReport htmlReport = new HTMLReport();

            RichTextBox richTextBox = tabControl1.SelectedTab.Controls.Cast<RichTextBox>().FirstOrDefault(x => x is RichTextBox);
            string content = richTextBox.Text;
            lexicalAnalyzer.scanner(content);

            if (graphVizBox.Image != null)
            {
                graphVizBox.Image.Dispose();

            }
            graphVizBox.Image = null;
            flagBox.Image = null;
            countryLabel.Text = null;
            populationlabel.Text = null;

            intelliSense();
            if (!lexicalAnalyzer.ListError.Any())
            {
                if (lexicalAnalyzer.ListToken.Any())
                {
                    htmlReport.generateReport("listadoTokens.html", lexicalAnalyzer.ListToken);
                    if (File.Exists(Directory.GetCurrentDirectory() + "\\listadoTokens.html"))
                    {
                        Process.Start(Directory.GetCurrentDirectory() + "\\listadoTokens.html");
                    }

                    if (!verifySaturation(lexicalAnalyzer.ListToken))
                    {

                        createGraph(lexicalAnalyzer.ListToken);
                        if (File.Exists(Directory.GetCurrentDirectory() + "\\" + graph.Name.Replace(" ", "") + ".png"))
                        {
                            Image image = Image.FromFile(Directory.GetCurrentDirectory() + "\\" + graph.Name.Replace(" ", "") + ".png");
                            graphVizBox.Image = image;
                            // graphVizBox.Image = new Bitmap(Image.FromFile(Directory.GetCurrentDirectory() + "\\" + graph.Name.Replace(" ", "") + ".png"));
                        }

                        betterOption();
                    }
                    else
                    {
                        MessageBox.Show("EL archivo posee saturaciones mayores al 100%", "Error");
                    }
                }
            }
            else
            {
                htmlReport.generateReport("listadoTokens.html", lexicalAnalyzer.ListToken);
                htmlReport.generateReport("listadoErrores.html", lexicalAnalyzer.ListError);

                MessageBox.Show("El archivo de entrada posee errores", "Error");

                if (File.Exists(Directory.GetCurrentDirectory() + "\\listadoTokens.html")
                    && File.Exists(Directory.GetCurrentDirectory() + "\\listadoErrores.html"))
                {
                    Process.Start(Directory.GetCurrentDirectory() + "\\listadoTokens.html");
                    Process.Start(Directory.GetCurrentDirectory() + "\\listadoErrores.html");
                }
            }
        }

        private void createGraph(List<Token> ListToken)
        {
            GraphViz graphViz = new GraphViz();
            graph = null;

            for (int i = 0; i < ListToken.Count; i++)
            {
                if (ListToken[i].TypeToken.Equals("Reservada Grafica"))
                {
                    string graphName = removeQuotes(ListToken[i + 5].Value);

                    // Continents
                    List<Continent> continents = new List<Continent>();
                    for (int j = i + 1; j < ListToken.Count; j++)
                    {
                        if (ListToken[j].TypeToken.Equals("Reservada Continente"))
                        {
                            string continentName = removeQuotes(ListToken[j + 5].Value);
                            int continentSaturation = 0;
                            int continentPopulation = 0;

                            // Countries
                            List<Country> countries = new List<Country>();
                            for (int k = j + 1; k < ListToken.Count; k++)
                            {
                                if (ListToken[k].TypeToken.Equals("Reservada Pais"))
                                {
                                    string countryName = "";
                                    int countryPopulation = 0;
                                    int countrySaturation = 0;
                                    string countryFlag = "";

                                    for (int l = k + 1; l < ListToken.Count; l++)
                                    {
                                        if (ListToken[l].TypeToken.Equals("Reservada Poblacion"))
                                        {
                                            countryPopulation = int.Parse(ListToken[l + 2].Value);
                                        }
                                        else if (ListToken[l].TypeToken.Equals("Reservada Saturacion"))
                                        {
                                            countrySaturation = int.Parse(ListToken[l + 2].Value);
                                        }
                                        else if (ListToken[l].TypeToken.Equals("Reservada Bandera"))
                                        {
                                            countryFlag = removeQuotes(ListToken[l + 2].Value);
                                        }
                                        else if (ListToken[l].TypeToken.Equals("Reservada Nombre"))
                                        {
                                            countryName = removeQuotes(ListToken[l + 2].Value);
                                        }
                                        else if (ListToken[l].TypeToken.Equals("Reservada Pais") || ListToken[l].TypeToken.Equals("Reservada Continente"))
                                        {
                                            break;
                                        }
                                    }

                                    countries.Add(new Country(countryName, countryPopulation, countrySaturation, countryFlag));
                                }
                                else if (ListToken[k].TypeToken.Equals("Reservada Continente"))
                                {
                                    break;
                                }
                            }

                            foreach (Country item in countries)
                            {
                                continentSaturation += item.Saturation;
                                continentPopulation += item.Population;
                            }
                            continentSaturation = continentSaturation / countries.Count;
                            continents.Add(new Continent(continentName, continentPopulation, countries, continentSaturation));
                        }
                    }

                    graph = new Graph(graphName, continents);
                    graphViz.generateGraphViz(graph);
                }
            }
        }

        private void betterOption()
        {
            List<Country> countries = new List<Country>();
            List<Continent> continents = new List<Continent>();
            Country countryLabels = null;

            foreach (Continent continent in graph.Continents)
            {
                continent.Countries.OrderBy(x => x.Saturation).ToList();
                foreach (Country country in continent.Countries)
                {
                    countries.Add(country);
                }
            }

            var orderedCountries = countries.OrderBy(x => x.Saturation).ToList();
            var orderedContinents = graph.Continents.OrderBy(x => x.Saturation).ToList();
            for (int i = 0; i < orderedCountries.Count; i++)
            {
                if (orderedCountries[i].Saturation < orderedCountries[i + 1].Saturation)
                {
                    countryLabels = orderedCountries[i];
                    break;
                }
                else if (i < orderedCountries.Count && orderedCountries[i].Saturation == orderedCountries[i + 1].Saturation)
                {
                    for (int j = 0; j < orderedContinents.Count; j++)
                    {
                        if (orderedContinents[j].Countries[0].Name == orderedCountries[0].Name)
                        {
                            countryLabels = orderedCountries[i];
                            break;
                        }
                    }
                }
            }

            countryLabel.Text = "País seleccionado: " + countryLabels.Name;
            populationlabel.Text = "Población: " + countryLabels.Population;
            if (File.Exists(countryLabels.Flag))
            {
                Image image = Image.FromFile(countryLabels.Flag);
                flagBox.Image = image;
            }
        }

        private bool verifySaturation(List<Token> ListToken)
        {
            for (int i = 0; i < ListToken.Count; i++)
            {
                if (ListToken[i].TypeToken.Equals("Simbolo Porcentaje"))
                {
                    if (int.Parse(ListToken[i - 1].Value) > 100)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private string removeQuotes(string chain)
        {
            return chain.Replace("\"", "");
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = @"C:\",
                RestoreDirectory = true,
                FileName = "",
                DefaultExt = "ORG",
                Filter = "Archivos ORG (*.ORG)|*.ORG"
            };

            RichTextBox richTextBox = tabControl1.SelectedTab.Controls.Cast<RichTextBox>().FirstOrDefault(x => x is RichTextBox);
            string line = "";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                richTextBox.Clear();
                StreamReader streamReader = new StreamReader(openFileDialog.FileName);
                while (line != null)
                {
                    line = streamReader.ReadLine();
                    if (line != null)
                    {
                        richTextBox.AppendText(line);
                        richTextBox.AppendText(Environment.NewLine);
                    }
                }
                streamReader.Close();

                tabControl1.SelectedTab.Text = openFileDialog.FileName;
            }
        }

        private void saveAs()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = @"C:\",
                RestoreDirectory = true,
                FileName = "",
                DefaultExt = "ORG",
                Filter = "Archivos ORG (*.ORG)|*.ORG"
            };

            RichTextBox richTextBox = tabControl1.SelectedTab.Controls.Cast<RichTextBox>().FirstOrDefault(x => x is RichTextBox);

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                Stream fileStream = saveFileDialog.OpenFile();
                StreamWriter streamWriter = new StreamWriter(fileStream);
                streamWriter.Write(richTextBox.Text);
                streamWriter.Close();
                fileStream.Close();

                tabControl1.SelectedTab.Text = saveFileDialog.FileName;
                Console.WriteLine("Archivo " + saveFileDialog.FileName + " guardado con exito");
            }
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists(tabControl1.SelectedTab.Text))
            {
                RichTextBox richTextBox = tabControl1.SelectedTab.Controls.Cast<RichTextBox>().FirstOrDefault(x => x is RichTextBox);

                StreamWriter streamWriter = new StreamWriter(tabControl1.SelectedTab.Text);
                streamWriter.Write(richTextBox.Text);
                streamWriter.Close();
            }
            else
            {
                saveAs();
            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveAs();
        }

        private void NewTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            countTab++;
            TabPage tabPage = new TabPage("Tab" + countTab);

            RichTextBox richTextBox = new RichTextBox
            {
                BorderStyle = BorderStyle.None,
                Font = new Font("Microsoft Sans Serif", 12),
                Dock = DockStyle.Fill,
                Multiline = true,
                AcceptsTab = true
            };

            tabPage.Controls.Add(richTextBox);
            tabControl1.TabPages.Add(tabPage);
        }

        private void TabControl1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                for (int ix = 0; ix < tabControl1.TabCount; ++ix)
                {
                    if (tabControl1.GetTabRect(ix).Contains(e.Location))
                    {
                        tabControl1.TabPages.RemoveAt(ix);
                        break;
                    }
                }
            }
        }

        private void CloseToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("201801266  Didier Alfredo Domínguez Urías", "Datos");
        }

        private void GeneratePDFButton_Click(object sender, EventArgs e)
        {
            if (File.Exists(Directory.GetCurrentDirectory() + "\\" + graph.Name.Replace(" ", "") + ".pdf"))
            {
                Process.Start(Directory.GetCurrentDirectory() + "\\" + graph.Name.Replace(" ", "") + ".pdf");
            }
        }

        private void RichTextBox_TextChanged(object sender, EventArgs e)
        {
        }

        private void intelliSense()
        {
            LexicalAnalyzer lexicalAnalyzer = new LexicalAnalyzer();
            RichTextBox richTextBox = tabControl1.SelectedTab.Controls.Cast<RichTextBox>().FirstOrDefault(x => x is RichTextBox);
            string content = richTextBox.Text;
            lexicalAnalyzer.scanner(content);

            foreach (Token item in lexicalAnalyzer.ListToken)
            {
                if (item.TypeToken.Equals("Reservada Grafica") || item.TypeToken.Equals("Reservada Nombre")
                    || item.TypeToken.Equals("Reservada Continente") || item.TypeToken.Equals("Reservada Pais")
                    || item.TypeToken.Equals("Reservada Poblacion") || item.TypeToken.Equals("Reservada Saturacion")
                    || item.TypeToken.Equals("Reservada Bandera"))
                {
                    wordColor(item.Value, Color.FromArgb(41, 83, 131), richTextBox);
                }
            }


            foreach (Token item in lexicalAnalyzer.ListToken)
            {
                if (item.TypeToken.Equals("Numero"))
                {
                    wordColor(item.Value, Color.FromArgb(30, 232, 190), richTextBox);
                }
            }

            foreach (Token item in lexicalAnalyzer.ListToken)
            {
                if (item.TypeToken.Equals("Simbolo Llave Izquierda") || item.TypeToken.Equals("Simbolo Llave Derecha"))
                {
                    wordColor(item.Value, Color.FromArgb(227, 103, 149), richTextBox);
                }
            }

            foreach (Token item in lexicalAnalyzer.ListToken)
            {
                if (item.TypeToken.Equals("Simbolo Punto y Coma"))
                {
                    wordColor(item.Value, Color.FromArgb(207, 113, 65), richTextBox);
                }
            }

            foreach (Token item in lexicalAnalyzer.ListToken)
            {
                if (item.TypeToken.Equals("Cadena"))
                {
                    wordColor(item.Value, Color.FromArgb(217, 171, 103), richTextBox);
                }
            }
        }

        private void wordColor(string word, Color color, RichTextBox richTextBox)
        {
            int index = -1;
            int selectStart = richTextBox.SelectionStart;

            while ((index = richTextBox.Text.IndexOf(word, (index + 1))) != -1)
            {
                richTextBox.Select((index), word.Length);
                richTextBox.SelectionColor = color;
                richTextBox.Select(selectStart, 0);
                richTextBox.SelectionColor = Color.FromArgb(50, 50, 50);
            }
        }
    }
}
