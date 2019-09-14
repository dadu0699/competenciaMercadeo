using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace competenciaMercadeo.model
{
    class Token
    {
        public enum Type
        {
            RESERVADA_GRAFICA,
            RESERVADA_NOMBRE,
            RESERVADA_CONTINENTE,
            RESERVADA_PAIS,
            RESERVADA_POBLACION,
            RESERVADA_SATURACION,
            RESERVADA_BANDERA,
            SIMBOLO_LLAVE_IZQ,
            SIMBOLO_LLAVE_DCHO,
            SIMBOLO_DOS_PUNTOS,
            SIMBOLO_PUNTO_Y_COMA,
            SIMBOLO_PORCENTAJE,
            NUMERO,
            CADENA
        }

        private int idToken;
        private int row;
        private int column;
        private Type typeToken;
        private string value;

        public int IdToken { get => idToken; set => idToken = value; }
        public int Row { get => row; set => row = value; }
        public int Column { get => column; set => column = value; }
        public string Value { get => value; set => this.value = value; }
        public string TypeToken
        {
            get
            {
                switch (typeToken)
                {
                    case Type.RESERVADA_GRAFICA:
                        return "Reservada Grafica";
                    case Type.RESERVADA_NOMBRE:
                        return "Reservada Nombre";
                    case Type.RESERVADA_CONTINENTE:
                        return "Reservada Continente";
                    case Type.RESERVADA_PAIS:
                        return "Reservada Pais";
                    case Type.RESERVADA_POBLACION:
                        return "Reservada Poblacion";
                    case Type.RESERVADA_SATURACION:
                        return "Reservada Saturacion";
                    case Type.RESERVADA_BANDERA:
                        return "Reservada Bandera";
                    case Type.SIMBOLO_LLAVE_IZQ:
                        return "Simbolo Llave Izquierda";
                    case Type.SIMBOLO_LLAVE_DCHO:
                        return "Simbolo Llave Derecha";
                    case Type.SIMBOLO_DOS_PUNTOS:
                        return "Simbolo Dos Puntos";
                    case Type.SIMBOLO_PUNTO_Y_COMA:
                        return "Simbolo Punto y Coma";
                    case Type.SIMBOLO_PORCENTAJE:
                        return "Simbolo Porcentaje";
                    case Type.NUMERO:
                        return "Numero";
                    case Type.CADENA:
                        return "Cadena";
                    default:
                        return "Desconocido";
                }
            }
        }

        public Token(int idToken, int row, int column, Type typeToken, string value)
        {
            this.IdToken = idToken;
            this.Row = row;
            this.Column = column;
            this.typeToken = typeToken;
            this.Value = value;
        }
    }
}
