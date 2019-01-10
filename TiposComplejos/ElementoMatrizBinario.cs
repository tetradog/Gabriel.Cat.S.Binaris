using Gabriel.Cat.S.Extension;
using Gabriel.Cat.S.Utilitats;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace Gabriel.Cat.S.Binaris
{
    public class ElementoMatrizBinario<T> : ElementoBinarioNullable
    {

        public ElementoBinario Serializador { get; set; }
        public ElementoMatrizBinario(ElementoBinario elementoMatriz)
        {
            Serializador = elementoMatriz;
        }
        public byte[] GetBytes(T[,] matriz)
        {
            return JGetBytes(matriz);
        }
        protected override byte[] JGetBytes(object obj)
        {
            T[,] matriz = (T[,])obj;
            List<byte[]> datosMatriz=new List<byte[]>();
            datosMatriz.Add(Serializar.GetBytes(matriz.GetLength(DimensionMatriz.X)));
            datosMatriz.Add(Serializar.GetBytes(matriz.GetLength(DimensionMatriz.Y)));
            for(int y=0,xF= matriz.GetLength(DimensionMatriz.X),yF= matriz.GetLength(DimensionMatriz.Y);y<yF;y++)
            {
                for (int x = 0; x < xF; x++)
                    datosMatriz.Add(Serializador.GetBytes(matriz[x, y]));
            }
            return new byte[0].AddArray(datosMatriz.ToArray());
        }

        
        protected override object JGetObject(MemoryStream bytes)
        {

            T[,] matriz = new T[Serializar.ToInt(bytes.Read(sizeof(int))), Serializar.ToInt(bytes.Read(sizeof(int)))];
            for (int y = 0, xF = matriz.GetLength(DimensionMatriz.X), yF = matriz.GetLength(DimensionMatriz.Y); y < yF; y++)
                for (int x = 0; x < xF; x++)
                    matriz[x, y] =(T) Serializador.GetObject(bytes);
            return matriz;
        }
    }
}
