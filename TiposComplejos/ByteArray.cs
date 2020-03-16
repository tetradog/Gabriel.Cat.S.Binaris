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
        }
    

        protected override byte[] IGetBytes(object obj)
        {
            byte[] data = (byte[])obj;
            return Serializar.GetBytes(data.Length).AddArray(data);
        }

        protected override object IGetObject(MemoryStream bytes)
        {
            byte[] data = new byte[Serializar.ToInt(bytes.Read(4))];
            bytes.Read(data, 0, data.Length);
            return data;
        }
        public override string ToString()
        {
            return "TipoDatos=ByteArray";
        }
    }
}
