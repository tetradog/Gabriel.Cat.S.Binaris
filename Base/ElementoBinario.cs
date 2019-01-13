using Gabriel.Cat.S.Extension;
using Gabriel.Cat.S.Utilitats;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using Gabriel.Cat.S.Seguretat;

namespace Gabriel.Cat.S.Binaris
{
    public abstract class ElementoBinario
    {
        static readonly ByteArrayBinario byteArrayBinario = new ByteArrayBinario();
        public const byte NULL = 0x0;
        public const byte NOTNULL = 0x1;
        public Key Key { get; set; }
        public byte[] GetBytes()
        {
            return GetBytes(this);
        }
        public byte[] GetBytes(object obj)
        {
            byte[] bytes = IGetBytes(obj);
            if (Key != null)
            {
                bytes = byteArrayBinario.GetBytes(Key.Encrypt(bytes));
            }
            return bytes;

        }

        protected abstract byte[] IGetBytes(object obj);

        public object GetObject(byte[] bytes)
        {
            return GetObject(new MemoryStream(bytes));
        }

        public object GetObject(MemoryStream bytes)
        {
            byte[] bytesObj;

            if (Key != null)
            {
                bytesObj = (byte[])byteArrayBinario.GetObject(bytes);
                bytes = new MemoryStream(Key.Decrypt(bytesObj));
            }
            return IGetObject(bytes);
        }

        protected abstract object IGetObject(MemoryStream bytes);

        public static ElementoBinario ElementosTipoAceptado(Serializar.TiposAceptados tipo)
        {
            ElementoBinario elemento;
            switch (tipo)
            {
                case Serializar.TiposAceptados.String:
                    elemento = new StringBinario();
                    break;
                case Serializar.TiposAceptados.Bitmap:
                    elemento = new BitmapBinario();
                    break;
                default:
                    elemento = new ElementoBinarioTamañoFijo(tipo);
                    break;
            }
            return elemento;
        }
        public static bool IsCompatible(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException();

            IList lst = obj as IList;
            bool compatible;
            Type tipoObj = obj.GetType();
            if (tipoObj.Name.Contains("["))
            {
                compatible = tipoObj.GetArrayRank() < 3;//mirar como sacar el tipo...
                if (compatible)
                {
                    compatible = IsCompatible(tipoObj.GetArrayType());//mirar el resultado  
                }
            }
            else if (lst != null)
                compatible = IsCompatible(lst.ListOfWhat());

            else compatible = IsCompatible(tipoObj);

            return compatible;

        }
        public static bool IsCompatible(Type tipo)
        {
            bool compatible = tipo.ImplementInterficie(typeof(IElementoBinarioComplejo));
            Type[] tiposAux;
            Type tipoKeyValuePair = typeof(KeyValuePair<,>);
            Type tipoTwoKeys = typeof(TwoKeys<,>);
            if (!compatible)
            {
                try
                {
                    Serializar.AssemblyToEnumTipoAceptado(tipo.AssemblyQualifiedName);
                    compatible = true;
                }
                catch
                {
                    switch (tipo.AssemblyQualifiedName)
                    {
                        //poner tipos

                        default:
                            compatible = false;
                            break;
                    }
                    if (!compatible)
                    {
                        if (tipo.IsArray && tipo.GetArrayRank() < 3)
                        {
                            compatible = IsCompatible(tipo.GetArrayType());
                        }
                        //KeyValuePair<TKey,TValue> ->TKey && TValue are compatible type
                        else if (tipoKeyValuePair.Equals(tipo.GetGenericTypeDefinition()) || tipoTwoKeys.Equals(tipo.GetGenericTypeDefinition()))
                        {
                            tiposAux = tipo.GetGenericArguments();
                            compatible = IsCompatible(tiposAux[0]) && IsCompatible(tiposAux[1]);
                        }
                        else if (typeof(Enum).IsAssignableFrom(tipo))
                        {
                            //mirar si funciona
                            compatible = true;
                        }

                    }
                }
            }
            return compatible;
        }

        /// <summary>
        /// Obtiene el Serializador del tipo indicado como parametro
        /// </summary>
        /// <param name="tipo">tipo con constructor sin parametros si es IElementoBinarioComplejo</param>
        /// <returns>devuelve null si no es compatible</returns>
        public static ElementoBinario GetElementoBinario(Type tipo)
        {
            ElementoBinario elementoBinario;
            Type tipoKeyValuePair = typeof(KeyValuePair<,>);
            Type tipoTwoKeys = typeof(TwoKeys<,>);
            if (IsCompatible(tipo))
            {
                if (tipo.Name.Contains("["))
                {
                    switch (tipo.GetArrayRank())
                    {
                        case 1:
                            elementoBinario = (ElementoBinario)Activator.CreateInstance(typeof(ElementoIListBinario<>).MakeGenericType(tipo.GetArrayType()));
                            break;
                        case 2:
                            elementoBinario = (ElementoBinario)Activator.CreateInstance(typeof(ElementoMatrizBinario<>).MakeGenericType(tipo.GetArrayType()),GetElementoBinario(tipo.GetArrayType() ));
                            break;
                        default: throw new ArgumentOutOfRangeException("el tipo sobrepasa el rango");
                    }

                }
                //con string da problemas en estas comparaciones...
                else if (tipo.IsGenericType&&tipoKeyValuePair.Equals(tipo.GetGenericTypeDefinition()))//mirar si compara KeyValuePair con KeyValuePair
                {
                    elementoBinario = (ElementoBinario)GetITwoPartsSerielitzer(tipo, typeof(KeyValuePairBinario<,>));
                }
                else if (tipo.IsGenericType && tipoTwoKeys.Equals(tipo.GetGenericTypeDefinition()))
                {
                    elementoBinario = (ElementoBinario)GetITwoPartsSerielitzer(tipo, typeof(TwoKeysBinary<,>));
                }

                else { 
                
                    elementoBinario = IGetElementoBinario(tipo);
                }

            }
            else elementoBinario = null;

            return elementoBinario;
        }
        private static ITwoPartsElement GetITwoPartsSerielitzer(Type tipo, Type tipoITwopartsGeneric)
        {
            Type[] tiposAux = tipo.GetType().GetGenericArguments();
            ITwoPartsElement elementoKeyValuePair = (ITwoPartsElement)Activator.CreateInstance(tipoITwopartsGeneric.MakeGenericType(tipo), new Object[] { Activator.CreateInstance(tiposAux[0]), Activator.CreateInstance(tiposAux[1]) });
            //obtengo tipoKey
            ElementoBinario elementoBinarioAux = IGetElementoBinario(tiposAux[0]);

            elementoKeyValuePair.Part1 = elementoBinarioAux;

            //obtengo tipoValue

            elementoBinarioAux = IGetElementoBinario(tiposAux[1]);

            elementoKeyValuePair.Part2 = elementoBinarioAux;

            return elementoKeyValuePair;
        }
        private static ElementoBinario IGetElementoBinario(Type tipo)
        {
            Type tipoKeyValuePair = typeof(KeyValuePair<,>);
            Type tipoTwoKeys = typeof(TwoKeys<,>);
            ElementoBinario elementoBinario;
            if (tipo.IsGenericType&&(tipoKeyValuePair.Equals(tipo.GetGenericTypeDefinition()) || tipoTwoKeys.Equals(tipo.GetGenericTypeDefinition())))
            {
                elementoBinario = GetElementoBinario(tipo);
            }
            else
            {
                if (tipo.ImplementInterficie(typeof(IElementoBinarioComplejo)))
                {
                    try
                    {

                        elementoBinario = ((IElementoBinarioComplejo)Activator.CreateInstance(tipo)).Serialitzer;
                    }
                    catch
                    {
                        throw new ArgumentException(String.Format("El tipo tiene que tener un constructor publico sin parametros y la propiedad de {0} tener valor.", typeof(IElementoBinarioComplejo).Name));

                    }
                }
                else if (typeof(Enum).IsAssignableFrom(tipo))//mirar si va así
                {
                    elementoBinario = (ElementoBinario)Activator.CreateInstance(typeof(EnumBinario<>).MakeGenericType(tipo));
                }
                else
                {

                    elementoBinario = ElementosTipoAceptado(Serializar.AssemblyToEnumTipoAceptado(tipo.AssemblyQualifiedName));
                }
            }
            return elementoBinario;
        }
        /// <summary>
        /// Devuelve el serializador del objeto pasado como parametro
        /// </summary>
        /// <param name="obj">se tendrá en cuenta si implementa IElementoBinarioComplejo.</param>
        /// <returns>si no es compatible es null</returns>
        public static ElementoBinario GetElementoBinario(object obj)
        {
            return GetElementoBinario(obj.GetType());
        }
        public static ElementoBinario GetSerializador<T>() where T : new()
        {//añadir posibilidad de evitar la serializacion de un elemento usando atributos
            const UsoPropiedad USONECESARIO = UsoPropiedad.Get | UsoPropiedad.Set;
            const UsoPropiedad USOILISTNECESARIO = UsoPropiedad.Get;
            GetPartsObjectMethod getPartsObj = (obj) =>
            {

                IList<Propiedad> propiedades = obj.GetPropiedades();
                List<object> partes = new List<object>();
                for (int i = 0; i < propiedades.Count; i++)
                {
                    if (propiedades[i].Info.Uso == USONECESARIO && ElementoBinario.IsCompatible(propiedades[i].Info.Tipo) || propiedades[i].Objeto is IList && propiedades[i].Info.Uso == USOILISTNECESARIO && ElementoBinario.IsCompatible(propiedades[i].Objeto))
                        partes.Add(propiedades[i].Objeto);
                }
                return partes;
            };
            GetObjectMethod getObject = (partes) =>
            {

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
                    else if (propiedades[i].Info.Uso == USOILISTNECESARIO && partes[j].GetType().ImplementInterficie(typeof(IList)) && ElementoBinario.IsCompatible(partes[j]))
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

                        foreach (dynamic pair in dicAPoner)
                            dicObjs.Add(pair.Key, pair.Value);
                    }
                }
                return obj;
            };


            IList<PropiedadTipo> properties = typeof(T).GetPropiedadesTipos();
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
}