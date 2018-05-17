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
                //case Serializar.TiposAceptados.Bitmap:
                //    elemento = new BitmapBinario();
                //    break;
                default:
                    elemento = new ElementoBinarioTamañoFijo(tipo);
                    break;
            }
            return elemento;
        }
        public static bool IsCompatible(object tipo)
        {
            IList lst = tipo as IList;
            bool compatible;
            if (lst != null)
                compatible = IsCompatible(lst.ListOfWhat());
            else compatible = IsCompatible(tipo.GetType());

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
                        //KeyValuePair<TKey,TValue> ->TKey && TValue are compatible type
                        if (tipoKeyValuePair.Equals(tipo.GetGenericTypeDefinition()) || tipoTwoKeys.Equals(tipo.GetGenericTypeDefinition()))
                        {
                            tiposAux = tipo.GetGenericArguments();
                            compatible = IsCompatible(tiposAux[0]) && IsCompatible(tiposAux[1]);
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
                if (tipoKeyValuePair.Equals(tipo.GetGenericTypeDefinition()))//mirar si compara KeyValuePair con KeyValuePair
                {
                    elementoBinario =(ElementoBinario)GetITwoPartsSerielitzer(tipo, typeof(KeyValuePairBinario<,>));
                }
                else if (tipoTwoKeys.Equals(tipo.GetGenericTypeDefinition()))
                {
                    elementoBinario = (ElementoBinario)GetITwoPartsSerielitzer(tipo, typeof(TwoKeysBinary<,>));
                }
                else
                {
                    elementoBinario = IGetElementoBinario(tipo);
                }
            }
            else elementoBinario = null;

            return elementoBinario;
        }
        private static ITwoPartsElement GetITwoPartsSerielitzer(Type tipo,Type tipoITwopartsGeneric)
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
            if (tipoKeyValuePair.Equals(tipo.GetGenericTypeDefinition())|| tipoTwoKeys.Equals(tipo.GetGenericTypeDefinition()))
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
            IElementoBinarioComplejo serializador = obj as IElementoBinarioComplejo;
            ElementoBinario elemento = serializador != null ? serializador.Serialitzer : null;
            IList lst;
            Type tipoLst;
            if (elemento == null)
            {
                lst = obj as IList;
                if (lst != null)
                {
                    try
                    {
                        tipoLst = lst.ListOfWhat();

                        elemento = (ElementoBinario)Activator.CreateInstance(typeof(ElementoIListBinario<>).MakeGenericType(tipoLst), new Object[] { ElementoBinario.ElementosTipoAceptado(Serializar.AssemblyToEnumTipoAceptado(tipoLst.AssemblyQualifiedName)), LongitudBinaria.UInt });

                    }
                    catch (Exception ex)
                    {

                    }
                }
                else
                {

                    try
                    {
                        elemento = ElementosTipoAceptado(Serializar.GetType(obj));
                    }
                    catch { }
                }

            }
            return elemento;
        }
    }
}