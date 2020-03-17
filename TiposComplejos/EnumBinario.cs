using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Gabriel.Cat.S.Binaris
{
    public class EnumBinario<T> : ElementoBinario
        where T:Enum
    {
        static readonly ElementoBinario Serializador = ElementoBinario.ElementoTipoAceptado(Utilitats.Serializar.TiposAceptados.ULong);
        protected override byte[] IGetBytes(object obj)
        {
            return Serializador.GetBytes((T)obj);//pongo el casting para filtrar los objetos pasados como parametro que sean del tipo que quiero
        }

        protected override object IGetObject(MemoryStream bytes)
        {
            return (T)Serializador.GetObject(bytes);
        }
        public override string ToString()
        {
            return $"TipoDatos={typeof(T).Name}Binario";
        }
        public override ElementoBinario Clon()
        {
            return new EnumBinario<T>();
        }
    }
}
