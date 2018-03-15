using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Gabriel.Cat.S.Seguretat;
namespace Gabriel.Cat.S.Binaris
{
    public class KeyBinario : ElementoIListBinario<ItemBinario>
    {
        public KeyBinario(ElementoBinario elemento, LongitudBinaria unidadCantidadElementos = LongitudBinaria.UInt) : base(elemento, unidadCantidadElementos)
        {
        }

        public KeyBinario(ElementoBinario elemento, byte[] marcaFin) : base(elemento, marcaFin)
        {
        }
    }
    public class ItemBinario : ElementoComplejoBinario
    {
        public ItemBinario()
        {
            Partes.Add(ElementoBinario.ElementosTipoAceptado(Utilitats.Serializar.TiposAceptados.Int));
            Partes.Add(ElementoBinario.ElementosTipoAceptado(Utilitats.Serializar.TiposAceptados.Int));
            Partes.Add(ElementoBinario.ElementosTipoAceptado(Utilitats.Serializar.TiposAceptados.String));
        }
        protected override IList IGetPartsObject(object obj)
        {
            Key.ItemKey item=obj as Key.ItemKey;
            return new object[] { item.MethodData, item.MethodPassword, item.Password };
        }

        protected override object IGetObject(MemoryStream bytes)
        {
            object[] partes = GetPartsObject(bytes);
            return new Key.ItemKey((int)partes[0], (int)partes[1], (string)partes[2]);

        }
    }
}
