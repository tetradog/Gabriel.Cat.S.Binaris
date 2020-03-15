using System;
using System.Collections.Generic;
using System.Text;

namespace Gabriel.Cat.S.Binaris
{
    public class ByteArrayBinario : ElementoIListBinario<byte>
    {
        public ByteArrayBinario(LongitudBinaria unidadCantidadElementos = LongitudBinaria.UInt) : base(new ElementoBinarioTamañoFijo(Utilitats.Serializar.TiposAceptados.Byte), unidadCantidadElementos)
        {
        }

        public ByteArrayBinario( byte[] marcaFin) : base(new ElementoBinarioTamañoFijo(Utilitats.Serializar.TiposAceptados.Byte), marcaFin)
        {
        }
        public override string ToString()
        {
            return "TipoDatos=ByteArray";
        }
    }
}
