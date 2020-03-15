using Gabriel.Cat.S.Extension;
using Gabriel.Cat.S.Utilitats;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace Gabriel.Cat.S.Binaris
{
    public class ElementoArrayBinario<T> : ElementoBinarioNullable
    {//mirar de hacer que funciones T[],T[,],T[,,]...si pudiera ser cualquier Rango mejor!!

        public ElementoBinario Serializador { get; set; }
        public ElementoArrayBinario(ElementoBinario elementoMatriz)
        {
            Serializador = elementoMatriz;
        }
        
        protected override byte[] JGetBytes(object obj)
        {
            Array matriz = (Array)obj;
            List<byte[]> datosMatriz=new List<byte[]>();
            int[] dimensiones = matriz.GetDimensiones();
            datosMatriz.Add(Serializar.GetBytes(matriz.Rank));
            for (int i = 0; i < matriz.Rank; i++)
                datosMatriz.Add(Serializar.GetBytes(dimensiones[i]));
            for (int i = 0, f = matriz.Length; i < f; i++)
                datosMatriz.Add(Serializador.GetBytes(matriz.GetValue(dimensiones,i)));
            return new byte[0].AddArray(datosMatriz.ToArray());
        }

        
        protected override object JGetObject(MemoryStream bytes)
        {
            int rank = Serializar.ToInt(bytes.Read(sizeof(int)));
            int[] dimensiones = new int[rank];
            Array matriz = null;
            for(int i=0;i<dimensiones.Length;i++)
                dimensiones[i]= Serializar.ToInt(bytes.Read(sizeof(int)));
            matriz= Array.CreateInstance(typeof(T), dimensiones);
            for (int i = 0, f = matriz.Length; i < f; i++)
                matriz.SetValue(dimensiones,i,Serializador.GetObject(bytes));
            return matriz;
        }
        public override string ToString()
        {
            return "TipoDatos=MatrizBinario";
        }
    }
}
