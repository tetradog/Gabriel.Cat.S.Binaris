using Gabriel.Cat.S.Extension;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gabriel.Cat.S.Binaris
{
    public abstract class ElementoBinarioNullable : ElementoBinario
    {
        #region implemented abstract members of ElementoBinario
        public override byte[] GetBytes(object obj)
        {
            byte[] bytesObj;
            if (obj == null)
                bytesObj = new byte[] { ElementoBinario.NULL };
            else bytesObj = new byte[] { ElementoBinario.NOTNULL }.AddArray( IGetBytes(obj)); 

            return bytesObj;
        }
        protected abstract byte[] IGetBytes(object obj);

        public override object GetObject(System.IO.MemoryStream bytes)
        {
            object obj;
            byte comprobarNull =(byte) bytes.ReadByte();
            if (comprobarNull == ElementoBinario.NULL)
            {
                obj = null;
            }
            else if(comprobarNull==ElementoBinario.NOTNULL)
            {
                
                obj = IGetObject(bytes);

            }
            else
            {
                throw new TipoException();
            }
            return obj;
        }
        protected abstract object IGetObject(System.IO.MemoryStream bytes);
        #endregion

    }
}