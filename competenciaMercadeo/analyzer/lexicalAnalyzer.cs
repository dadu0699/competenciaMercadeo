using competenciaMercadeo.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace competenciaMercadeo.controller
{
    class LexicalAnalyzer
    {
        private string auxiliary;
        private int state;
        private int idToken;
        private int idError;
        private int row;
        private int column;

        internal List<Token> ListToken { get; set; }
        internal List<Error> ListError { get; set; }

        public LexicalAnalyzer()
        {
            auxiliary = "";
            state = 0;
            idToken = 0;
            idError = 0;
            row = 1;
            column = 1;

            ListToken = new List<Token>();
            ListError = new List<Error>();
        }

        public void scanner(string entry)
        {
            char character;
            entry += "#";

            for (int i = 0; i < entry.Length; i++)
            {
                character = entry.ElementAt(i);
                switch (state)
                {
                    case 0:
                        // Reserved Word
                        if (char.IsLetter(character))
                        {
                            state = 1;
                            auxiliary += character;
                        }
                        // Digit
                        else if (char.IsDigit(character))
                        {
                            state = 2;
                            auxiliary += character;
                        }
                        // Chain
                        else if (character.Equals('"'))
                        {
                            state = 3;
                            auxiliary += character;
                        }
                        // Blanks and line breaks
                        else if (char.IsWhiteSpace(character))
                        {
                            state = 0;
                            auxiliary = "";
                            // Change row and restart columns in line breaks
                            if (character.CompareTo('\n') == 0)
                            {
                                column = 1;
                                row++;
                            }
                        }
                        // Symbol
                        else if (!addSymbol(character))
                        {
                            if (character.Equals('#') && i == (entry.Length - 1))
                            {
                                Console.WriteLine("Lexical analysis completed");
                            }
                            else
                            {
                                Console.WriteLine("Lexical Error: Not Found '" + character + "' in defined patterns");
                                addError(character.ToString());
                                state = 0;
                            }
                        }
                        break;
                    case 1:
                        if (char.IsLetter(character))
                        {
                            state = 1;
                            auxiliary += character;
                        }
                        else
                        {
                            addWordReserved();
                            i--;
                        }
                        break;
                    case 2:
                        if (char.IsDigit(character))
                        {
                            state = 2;
                            auxiliary += character;
                        }
                        else
                        {
                            addToken(Token.Type.NUMERO);
                            i--;
                        }
                        break;
                    case 3:
                        if (!character.Equals('"'))
                        {
                            state = 3;
                            auxiliary += character;
                        }
                        else
                        {
                            auxiliary += character;
                            addToken(Token.Type.CADENA);
                        }
                        break;
                }
                column++;
            }
        }

        public bool addSymbol(char character)
        {
            if (character.Equals('{'))
            {
                auxiliary += character;
                addToken(Token.Type.SIMBOLO_LLAVE_IZQ);
                return true;
            }
            else if (character.Equals('}'))
            {
                auxiliary += character;
                addToken(Token.Type.SIMBOLO_LLAVE_DCHO);
                return true;
            }
            else if (character.Equals(':'))
            {
                auxiliary += character;
                addToken(Token.Type.SIMBOLO_DOS_PUNTOS);
                return true;
            }
            else if (character.Equals(';'))
            {
                auxiliary += character;
                addToken(Token.Type.SIMBOLO_PUNTO_Y_COMA);
                return true;
            }
            else if (character.Equals('%'))
            {
                auxiliary += character;
                addToken(Token.Type.SIMBOLO_PORCENTAJE);
                return true;
            }
            return false;
        }

        public void addWordReserved()
        {
            if (auxiliary.Equals("Grafica"))
            {
                addToken(Token.Type.RESERVADA_GRAFICA);
            }
            else if (auxiliary.Equals("Nombre"))
            {
                addToken(Token.Type.RESERVADA_NOMBRE);
            }
            else if (auxiliary.Equals("Continente"))
            {
                addToken(Token.Type.RESERVADA_CONTINENTE);
            }
            else if (auxiliary.Equals("Pais"))
            {
                addToken(Token.Type.RESERVADA_PAIS);
            }
            else if (auxiliary.Equals("Poblacion"))
            {
                addToken(Token.Type.RESERVADA_POBLACION);
            }
            else if (auxiliary.Equals("Saturacion"))
            {
                addToken(Token.Type.RESERVADA_SATURACION);
            }
            else if (auxiliary.Equals("Bandera"))
            {
                addToken(Token.Type.RESERVADA_BANDERA);
            }
            else
            {
                Console.WriteLine("Lexical Error: Not Found '" + auxiliary + "' in defined patterns");
                addError(auxiliary);
                auxiliary = "";
                state = 0;
            }
        }

        public void addToken(Token.Type type)
        {
            idToken++;
            ListToken.Add(new Token(idToken, row, column - auxiliary.Length, type, auxiliary));
            auxiliary = "";
            state = 0;
        }

        public void addError(string chain)
        {
            idError++;
            ListError.Add(new Error(idError, row, column, chain, "Patrón desconocido"));
        }
    }
}
