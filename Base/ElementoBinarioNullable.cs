using Gabriel.Cat.S.Extension;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gabriel.Cat.S.Binaris
{
    public abstract class ElementoBinarioNullable : ElementoBinario
    {
        #region implemented abstract members of ElementoBinario
        protected override byte[] IGetBytes(object obj)
        {
            byte[] bytesObj;
            byte[] aux;
            if (obj == null)
                bytesObj = new byte[] { ElementoBinario.NULL };
            else
            {
                aux = JGetBytes(obj);
                bytesObj = new byte[] { ElementoBinario.NOTNULL }.AddArray(aux);
            }
            return bytesObj;
        }
        protected abstract byte[] JGetBytes(object obj);

        protected override object IGetObject(System.IO.MemoryStream bytes)
        {
            object obj;
            byte comprobarNull =(byte) bytes.ReadByte();
            if (comprobarNull == ElementoBinario.NULL)
            {
                obj = null;
            }
            else if(comprobarNull==ElementoBinario.NOTNULL)
            {
                
                obj = JGetObject(bytes);

            }
            else
            {
                throw new TipoException();
            }
            return obj;
        }
        protected abstract object JGetObject(System.IO.MemoryStream bytes);
        #endregion

    }
}