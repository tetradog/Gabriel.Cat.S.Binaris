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

        public static ElementoComplejoBinarioNullable GetElement<T>() where T : new()
        {
            const UsoPropiedad USONECESARIO = UsoPropiedad.Get | UsoPropiedad.Set;
            const UsoPropiedad USOILISTNECESARIO = UsoPropiedad.Get;
            GetPartsObjectMethod getPartsObj = (obj) => {

                IList<Propiedad> propiedades = obj.GetPropiedades();
                List<object> partes = new List<object>();
                for (int i = 0; i < propiedades.Count; i++)
                {
                    if (propiedades[i].Info.Uso == USONECESARIO && ElementoBinario.IsCompatible(propiedades[i].Info.Tipo) || propiedades[i].Objeto is IList && propiedades[i].Info.Uso == USOILISTNECESARIO && ElementoBinario.IsCompatible(propiedades[i].Objeto))
                        partes.Add(propiedades[i].Objeto);
                }
                return partes;
            };
            GetObjectMethod getObject = (partes) => {

                T obj = new T();
                IList<Propiedad> propiedades = obj.GetPropiedades();
                IList lstAPoner;
                IList lstObj;
                IDictionary dicAPoner;
                IDictionary dicObjs;

                for (int i = 0, j = 0; i < propiedades.Count; i++)
                {
                    if (propiedades[i].Info.Uso == USONECESARIO && ElementoBinario.IsCompatible(partes[j].GetType()))
                        obj.SetProperty(propiedades[i].Info.Nombre, partes[j++]);
                    else if (propiedades[i].Info.Uso == USOILISTNECESARIO && partes[j].GetType().ImplementInterficie(typeof(IList))  && ElementoBinario.IsCompatible(partes[j]))
                    {
                        lstAPoner = partes[j++] as IList;
                        //cojo la lista del objeto y le añado la nueva
                        lstObj = obj.GetProperty(propiedades[i].Info.Nombre) as IList;

                        for (int k = 0; k < lstAPoner.Count; k++)
                            lstObj.Add(lstAPoner[k]);
                    }
                    else if (propiedades[i].Info.Uso == USOILISTNECESARIO && partes[j].GetType().ImplementInterficie(typeof(IDictionary)) && ElementoBinario.IsCompatible(partes[j]))
                    {//por probar
                        dicAPoner = partes[j++] as IDictionary;
                        //cojo la lista del objeto y le añado la nueva
                        dicObjs = obj.GetProperty(propiedades[i].Info.Nombre) as IDictionary;
                       
                        foreach(dynamic pair in dicAPoner)
                            dicObjs.Add(pair.Key,pair.Value);
                    }
                }
                return obj;
            };


           IList< PropiedadTipo> properties = typeof(T).GetPropiedadesTipos();
            List<ElementoBinario> elementos = new List<ElementoBinario>();
            IList list;
            IDictionary dic;
            for (int i = 0; i < properties.Count; i++)
            {
                if (properties[i].Uso == USONECESARIO && ElementoBinario.IsCompatible(properties[i].Tipo))
                    elementos.Add(ElementoBinario.GetElementoBinario(properties[i].Tipo));
                else if (properties[i].Uso == USOILISTNECESARIO && properties[i].Tipo.ImplementInterficie(typeof(IList)))
                {
                    try
                    {

                        list = (IList)Activator.CreateInstance(properties[i].Tipo);
                        if (ElementoBinario.IsCompatible(list.ListOfWhat()))
                        {
                            //si es de un tipo compatible lo añado
                            elementos.Add(ElementoBinario.GetElementoBinario(list));
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new PropiedadNoCompatibleException(properties[i], ex);

                    }

                }
                else if (properties[i].Uso == USOILISTNECESARIO && properties[i].Tipo.ImplementInterficie(typeof(IDictionary)))
                {
                    try
                    {

                        dic = (IDictionary)Activator.CreateInstance(properties[i].Tipo);
                        if (ElementoBinario.IsCompatible(dic.DicOfWhat()))
                        {
                            //si es de un tipo compatible lo añado
                            elementos.Add(ElementoBinario.GetElementoBinario(dic));
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new PropiedadNoCompatibleException(properties[i], ex);

                    }
                }
            }



            return new ElementoComplejoBinarioNullableExt(elementos, getPartsObj, getObject);
        }
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