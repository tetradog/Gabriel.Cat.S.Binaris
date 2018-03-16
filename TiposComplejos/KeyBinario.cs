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
        public KeyBinario( LongitudBinaria unidadCantidadElementos = LongitudBinaria.UInt) : base(new ItemBinario(), unidadCantidadElementos)
        {
        }

        public KeyBinario(byte[] marcaFin) : base(new ItemBinario(), marcaFin)
        {
        }
        protected override byte[] IGetBytes(object obj)
        {
            Key key = obj as Key;
            if (key == null)
                throw new ArgumentException("tiene que ser un objeto "+(typeof(Key).FullName),"obj");

            return base.GetBytes(key.ItemsKey);
        }
        protected override object IGetObject(MemoryStream bytes)
        {
            return new Key((Key.ItemKey[])base.GetObject(bytes));
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

        protected override object JGetObject(MemoryStream bytes)
        {
            object[] partes = GetPartsObject(bytes);
            return new Key.ItemKey((int)partes[0], (int)partes[1], (string)partes[2]);

        }
    }
}
