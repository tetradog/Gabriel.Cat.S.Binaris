using Gabriel.Cat.S.Utilitats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Gabriel.Cat.S.Binaris
{
    public class StringBinario : ElementoIListBinario<char>
    {
        public StringBinario() : base(ElementoBinario.ElementosTipoAceptado(Serializar.TiposAceptados.Char), LongitudBinaria.UInt)
        {
        }

        public StringBinario(byte[] marcaFin) : base(ElementoBinario.ElementosTipoAceptado(Serializar.TiposAceptados.Char), marcaFin)
        {
        }

        public override object GetObject(MemoryStream bytes)
        {
            return new string((char[])base.GetObject(bytes));
        }

        public override byte[] GetBytes(object obj)
        {
            string str = obj as string;
            if (str == null)
                throw new ArgumentException(String.Format("Se tiene que serializar {0}", "".GetType().FullName));
            return base.GetBytes(str.ToCharArray());
        }
        public override string ToString()
        {
            return "TipoDatos=String";
        }
    }
}