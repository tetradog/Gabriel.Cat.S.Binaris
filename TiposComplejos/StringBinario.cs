using Gabriel.Cat.S.Utilitats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Gabriel.Cat.S.Extension;
namespace Gabriel.Cat.S.Binaris
{
    public class StringBinario : ElementoIListBinario<char>
    {
        public StringBinario(LongitudBinaria longitud= LongitudBinaria.UInt) : base(ElementoBinario.ElementoTipoAceptado(Serializar.TiposAceptados.Char), longitud)
        {
        }

        public StringBinario(byte[] marcaFin) : base(ElementoBinario.ElementoTipoAceptado(Serializar.TiposAceptados.Char), marcaFin)
        {
        }

        protected override object JGetObject(MemoryStream bytes)
        {
            return new string((char[])base.JGetObject(bytes));
        }

        protected override byte[] JGetBytes(object obj)
        {
            if (obj == null)
                throw new ArgumentException(String.Format("Se tiene que serializar {0}", typeof(string).FullName));
            IList<char> caracteres;
          
            caracteres = obj.ToString().ToCharArray();

            return base.JGetBytes(caracteres);
        }
        public override string ToString()
        {
            return "TipoDatos=String";
        }
        public override ElementoBinario Clon()
        {
            byte[] m = MarcaFin;
            LongitudBinaria l = Longitud;

            return new StringBinario() {Longitud=l,MarcaFin=m };
        }
    }
}