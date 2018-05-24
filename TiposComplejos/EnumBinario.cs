using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Gabriel.Cat.S.Binaris
{
    public class EnumBinario<T> : ElementoBinario
        where T:Enum
    {
        static readonly ElementoBinario Serializador = ElementoBinario.ElementosTipoAceptado(Utilitats.Serializar.TiposAceptados.ULong);
        protected override byte[] IGetBytes(object obj)
        {
            return Serializador.GetBytes((T)obj);//pongo el casting para filtrar los objetos pasados como parametro que sean del tipo que quiero
        }

        protected override object IGetObject(MemoryStream bytes)
        {
            return (T)Serializador.GetObject(bytes);
        }
    }
}
