using Gabriel.Cat.S.Utilitats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Gabriel.Cat.S.Binaris
{
    public class StringBinario : ElementoIListBinario<char>
    {
        public StringBinario(LongitudBinaria longitud= LongitudBinaria.UInt) : base(ElementoBinario.ElementosTipoAceptado(Serializar.TiposAceptados.Char), longitud)
        {
        }

        public StringBinario(byte[] marcaFin) : base(ElementoBinario.ElementosTipoAceptado(Serializar.TiposAceptados.Char), marcaFin)
        {
        }

        protected override object IGetObject(MemoryStream bytes)
        {
            return new string((char[])base.GetObject(bytes));
        }

        protected override byte[] IGetBytes(object obj)
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