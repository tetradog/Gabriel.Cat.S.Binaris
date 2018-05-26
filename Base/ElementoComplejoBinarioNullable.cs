using Gabriel.Cat.S.Extension;
using Gabriel.Cat.S.Utilitats;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Gabriel.Cat.S.Binaris
{
    public delegate IList GetPartsObjectMethod(object obj);
    public delegate object GetObjectMethod(object[] partsObj);
    /// <summary>
    /// Permite serializar y deserializar partes de un elemento
    /// </summary>
    public abstract class ElementoComplejoBinarioNullable : ElementoBinarioNullable
    {
        Llista<ElementoBinario> partes;

        public ElementoComplejoBinarioNullable(IList<ElementoBinario> partes = null)
        {
            this.partes = new Llista<ElementoBinario>();
            if (partes != null)
                this.partes.AddRange(partes);
        }

        protected Llista<ElementoBinario> Partes
        {
            get
            {
                return partes;
            }
        }

        #region implemented abstract members of ElementoBinarioNullable

        protected override byte[] JGetBytes(object obj)
        {
            IList partesObj = IGetPartsObject(obj);
            object[] bytesPartes;

            if (partesObj.Count != partes.Count)
                throw new ArgumentException(String.Format("El numero de partes no coincide con las partes del {0}", GetType().FullName), "obj");

            bytesPartes = new object[partes.Count];
            for (int i = 0; i < partes.Count; i++)
                bytesPartes[i] = partes[i].GetBytes(partesObj[i]);
            return new byte[0].AddArray(bytesPartes.Casting<byte[]>());

        }
        protected abstract IList IGetPartsObject(object obj);



        public object[] GetPartsObject(System.IO.MemoryStream ms)
        {
            object[] partesObj = new object[partes.Count];
            for (int i = 0; i < partes.Count; i++)
                partesObj[i] = partes[i].GetObject(ms);

            return partesObj;
        }

        #endregion


    }
    public class ElementoComplejoBinarioNullableExt : ElementoComplejoBinarioNullable
    {
        GetPartsObjectMethod GetPartsObjectDelegate;
        GetObjectMethod GetObjectDelegate;
        public ElementoComplejoBinarioNullableExt(IList<ElementoBinario> partes, GetPartsObjectMethod getPartsObjectDelegate, GetObjectMethod getObjectDelegate) : base(partes)
        {
            if (getObjectDelegate == null || getPartsObjectDelegate == null)
                throw new ArgumentNullException();
            GetPartsObjectDelegate = getPartsObjectDelegate;
            GetObjectDelegate = getObjectDelegate;
        }
        #region implemented abstract members of ElementoBinarioNullable
        protected override object JGetObject(System.IO.MemoryStream bytes)
        {
            return GetObjectDelegate(GetPartsObject(bytes));
        }
        #endregion
        #region implemented abstract members of ElementoComplejoBinarioNullable
        protected override IList IGetPartsObject(object obj)
        {
            return GetPartsObjectDelegate(obj);
        }
        #endregion

    }
    public class PropiedadNoCompatibleException : Exception
    {
        public PropiedadNoCompatibleException(PropiedadTipo propiedad, Exception ex) : base(string.Format("Propiedad '{0}' del tipo '{1}' no es compatible: '{2}'", propiedad.Nombre, propiedad.Tipo, ex.Message))
        { }
    }
}