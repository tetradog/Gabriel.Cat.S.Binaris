using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Gabriel.Cat.S.Binaris
{
    /// <summary>
    /// permite serializar y deserializar partes de un elemento
    /// </summary>
    public abstract class ElementoComplejoBinario : ElementoComplejoBinarioNullable
    {
        public ElementoComplejoBinario(IList<ElementoBinario> partes = null) : base(partes)
        { }
        public override byte[] GetBytes(object obj)
        {
            return IGetBytes(obj);
        }
        public override object GetObject(MemoryStream bytes)
        {
            return IGetObject(bytes);
        }

    }
}
