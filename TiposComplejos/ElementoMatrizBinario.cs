using Gabriel.Cat.S.Extension;
using Gabriel.Cat.S.Utilitats;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace Gabriel.Cat.S.Binaris.TiposComplejos
{
    public class ElementoMatrizBinario<T> : ElementoIListBinario<T[]>
    {
        public ElementoMatrizBinario(ElementoBinario elemento, LongitudBinaria unidadCantidadElementos = LongitudBinaria.UInt) : base(elemento, unidadCantidadElementos)
        {
            Elemento = new ElementoIListBinario<T>(elemento,unidadCantidadElementos);
        }

        public ElementoMatrizBinario(ElementoBinario elemento, byte[] marcaFin) : base(elemento, marcaFin)
        {
            Elemento = new ElementoIListBinario<T>(elemento, marcaFin);
        }
        protected override byte[] JGetBytes(object obj)
        {//falta mirar si funciona...
            T[,] matriz = (T[,])obj;
            List<T[]> columnas = new List<T[]>();
            T[] columna;
           
            List<byte[]> bytesMatriz = new List<byte[]>();
            ElementoBinario elementoSize = ElementoBinario.GetElementoBinario(typeof(int));
            for (int x = 0, xF = matriz.GetLength(DimensionMatriz.Columna), yF = matriz.GetLength(DimensionMatriz.Fila); x < xF; x++)
            {
                columna = new T[yF];
                for (int y = 0; y < yF; y++)
                    columna[y] = matriz[x, y];
                columnas.Add(columna);
            }

            bytesMatriz.Add(elementoSize.GetBytes(columnas.Count));

            for (int i = 0; i < columnas.Count; i++)
                bytesMatriz.Add(base.GetBytes(columnas[i]));

            return new byte[0].AddArray(bytesMatriz.ToArray());
        }

        
        protected override object JGetObject(MemoryStream bytes)
        {//falta mirar si funciona...
            ulong? numItems = null;

            List<IList<T>> lstColumnas ;
            T[,] matriz;

            Llista<byte> compruebaBytes = new Llista<byte>();
            List<byte> bytesElementoMarcaFin = new List<byte>();
            byte[] bufferStreamBytes, bytesObj;
            object objHaPoner;
            int posMarcaFin;
            List<T> objects;

            switch (Longitud)
            {
                case LongitudBinaria.Byte:
                    numItems = Convert.ToUInt64(bytes.ReadByte());
                    break;
                case LongitudBinaria.UShort:
                    numItems = Serializar.ToUShort(bytes.Read(2));
                    break;
                case LongitudBinaria.UInt:
                    numItems = Serializar.ToUInt(bytes.Read(4));
                    break;
            }
            lstColumnas = new List<IList<T>>();
            if (numItems.HasValue)
            {

                for (ulong i = 0, f = numItems.Value; i < f; i++)
                {
                   lstColumnas.Add( (T[])Elemento.GetObject(bytes));
                }
            }
            else
            {
                //usa marca fin
                //pongo el byte en la cola
                //miro si coincide con la marca fin
                //si no coincide cojo el primer byte
                //si coincide dejo de añadir bytes
                bufferStreamBytes = bytes.ToArray();
                objects = new List<T>();
                posMarcaFin = (int)bufferStreamBytes.SearchArray((int)bytes.Position, base.MarcaFin);
                if (posMarcaFin > 0)
                {
                    bytesObj = bufferStreamBytes.SubArray((int)bytes.Position, posMarcaFin);
                    //ahora tengo los bytes tengo que obtener los elementos
                    bytes = new MemoryStream(bytesObj);
                    do
                    {
                        objHaPoner = Elemento.GetObject(bytes);
                        if (objHaPoner != null)
                            lstColumnas.Add((T[])objHaPoner);
                    }
                    while (objHaPoner != null && !bytes.EndOfStream());
                  
                }
                else
                    throw new FormatException("No se ha encontrado la marca de fin");
            }
  
            matriz= new T[lstColumnas.Count, lstColumnas.Count > 0 ? lstColumnas[0].Count : 0];
            if (lstColumnas.Count > 0)
                for (int x = 0, yF = lstColumnas.Count; x < lstColumnas[0].Count; x++)
                    for (int y = 0; y < yF; y++)
                        matriz[x, y] = lstColumnas[x][y];

            return matriz;
        }
    }
}
