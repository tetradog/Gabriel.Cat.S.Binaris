using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Gabriel.Cat.S.Seguretat;
using Gabriel.Cat.S.Utilitats;

namespace Gabriel.Cat.S.Binaris
{
    public class KeyBinarioConId : ElementoComplejoBinario
    {
        public KeyBinarioConId() : base()
        {
            Partes.Add(new ByteArrayBinario());
            Partes.Add(new KeyBinario());
        }

        protected override IList IGetPartsObject(object obj)
        {
            Key key = obj as Key;
            return new object[] { key.Id.GetId(), key };
        }

        protected override object JGetObject(MemoryStream bytes)
        {
            object[] partes = GetPartsObject(bytes);
            Key key = (Key)partes[1];
            return new Key(key.ItemsKey,true,new IdUnico((byte[])partes[0]));
        }
        public override string ToString()
        {
            return "TipoDatos=KeyBinarioConId";
        }
        public override ElementoBinario Clon()
        {
            return new KeyBinarioConId();
        }
    }
    public class KeyBinario : ElementoIListBinario<Key.ItemKey>
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

            return base.JGetBytes(key.ItemsKey);
        }
        protected override object IGetObject(MemoryStream bytes)
        {
            return new Key((Key.ItemKey[])base.JGetObject(bytes));
        }
        public override string ToString()
        {
            return "TipoDatos=KeyBinario";
        }
        public override ElementoBinario Clon()
        {
            byte[] m = MarcaFin;
            LongitudBinaria l = Longitud;

            return new KeyBinario() { Longitud = l, MarcaFin = m };
        }
    }
    public class ItemBinario : ElementoComplejoBinario
    {
        public ItemBinario()
        {
            Partes.Add(ElementoBinario.ElementoTipoAceptado(Utilitats.Serializar.TiposAceptados.Int));
            Partes.Add(ElementoBinario.ElementoTipoAceptado(Utilitats.Serializar.TiposAceptados.Int));
            Partes.Add(ElementoBinario.ElementoTipoAceptado(Utilitats.Serializar.TiposAceptados.String));
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
        public override string ToString()
        {
            return "TipoDatos=ItemKeyBinario";
        }
        public override ElementoBinario Clon()
        {
            return new ItemBinario();
        }
    }
}
