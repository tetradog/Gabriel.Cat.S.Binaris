using Gabriel.Cat.S.Extension;
using Gabriel.Cat.S.Utilitats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Gabriel.Cat.S.Binaris
{
    public class ByteArrayBinario : ElementoBinario
    {
        LongitudBinaria unidad;
        byte[] marcaFin;
        public ByteArrayBinario(LongitudBinaria unidadCantidadElementos = LongitudBinaria.UInt) 
        {
            this.unidad = unidadCantidadElementos;
        }

        public ByteArrayBinario(byte[] marcaFin)
        {
            this.marcaFin = marcaFin;
            this.unidad = LongitudBinaria.MarcaFin;
        }
    

        protected override byte[] IGetBytes(object obj)
        {
            byte[] data = (byte[])obj;
            return Serializar.GetBytes(data.Length).AddArray(data);
        }

        protected override object IGetObject(MemoryStream bytes)
        {
            long aux;
            long longitud=default;
            
            byte[] data=default;
            switch (unidad)
            {
                case LongitudBinaria.UInt:longitud = Serializar.ToUInt(bytes.Read(sizeof(uint)));break;
                case LongitudBinaria.UShort: longitud = Serializar.ToUShort(bytes.Read(sizeof(ushort))); break;
                case LongitudBinaria.Byte: longitud = bytes.ReadByte(); break;
                case LongitudBinaria.MarcaFin:
                    aux = bytes.Position;
                    data = new byte[bytes.Length - bytes.Position];
                    bytes.Read(data,0,data.Length);
                    longitud = data.SearchArray(marcaFin)-aux;
                    data = data.SubArray((int)longitud);
                    bytes.Position = aux + longitud;
                break;

            }
            if (data == null)
            {
                data = new byte[longitud];
                bytes.Read(data, 0, data.Length);
            }
            return data;
        }
        public override string ToString()
        {
            return "TipoDatos=ByteArray";
        }
    }
}
