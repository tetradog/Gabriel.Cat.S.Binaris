﻿using System;
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
        protected override byte[] IGetBytes(object obj)
        {
            return IGetBytes(obj);
        }
        protected override object IGetObject(MemoryStream bytes)
        {
            return IGetObject(bytes);
        }

    }
}
