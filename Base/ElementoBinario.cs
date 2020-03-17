using Gabriel.Cat.S.Extension;
using Gabriel.Cat.S.Utilitats;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using Gabriel.Cat.S.Seguretat;
using System.Linq;

namespace Gabriel.Cat.S.Binaris
{
    public delegate T GetEmtpyNewObject<T>();
    public delegate object GetEmtpyNewObject();
    public abstract class ElementoBinario:IClonable<ElementoBinario>
    {
        public static LlistaOrdenada<string, ElementoBinario> SerializadoresTiposNoSoportados { get; private set; }
        public static LlistaOrdenada<string,ElementoBinario> SerializadoresElementosAutosHechos { get; private set; }
        public static bool GuardarSerializadoresAutosHechos { get; set; }
        static SortedList<string, string> DicTiposBasicos = Serializar.AsseblyQualifiedName.ToSortedList();
        static readonly ByteArrayBinario byteArrayBinario = new ByteArrayBinario();
        static readonly LlistaOrdenada<string, ElementoBinario> DicTipos;
        static readonly LlistaOrdenada<string, string> DicTiposGenericos;
        public const byte NULL = 0x0;
        public const byte NOTNULL = 0x1;
        static ElementoBinario()
        {
            GuardarSerializadoresAutosHechos = true;
            SerializadoresElementosAutosHechos = new LlistaOrdenada<string, ElementoBinario>();
            SerializadoresTiposNoSoportados = new LlistaOrdenada<string, ElementoBinario>();
            DicTiposGenericos = new LlistaOrdenada<string, string>();//tipo obj generico,tipo serializador generico
            DicTipos = new LlistaOrdenada<string, ElementoBinario>();
           //tipos normales
            DicTipos.Add(typeof(CrazyKey).AssemblyQualifiedName, new CrazyKeyBinario());
            DicTipos.Add(typeof(IdUnico).AssemblyQualifiedName, new IdUnicoBinario());
            DicTipos.Add(typeof(Key).AssemblyQualifiedName, new KeyBinario());
            DicTipos.Add(typeof(byte[]).AssemblyQualifiedName, new ByteArrayBinario());
            //tipos genericos
            DicTiposGenericos.Add(typeof(KeyValuePair<,>).AssemblyQualifiedName, typeof(KeyValuePairBinario<,>).AssemblyQualifiedName);
            DicTiposGenericos.Add(typeof(TwoKeys<,>).AssemblyQualifiedName, typeof(TwoKeysBinario<,>).AssemblyQualifiedName);
            DicTiposGenericos.Add(typeof(Array).AssemblyQualifiedName, typeof(ElementoArrayBinario<>).AssemblyQualifiedName);
            //IDictionary<TKey,TValue>
            DicTiposGenericos.Add(typeof(IDictionary<,>).AssemblyQualifiedName, typeof(DictionaryBinary<,,>).AssemblyQualifiedName);
            //IList<T>
            DicTiposGenericos.Add(typeof(IList<>).AssemblyQualifiedName, typeof(ElementoIListBinario<,>).AssemblyQualifiedName);

        }
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

        public static ElementoBinario ElementoTipoAceptado(Serializar.TiposAceptados tipo)
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
        /// <summary>
        /// Se usan las propiedades con Get y Set y las List,IDictionary,TwoKeysList(deben estar inicializadas desde el contructor) con solo Get además se ignorará las propiedades con el atributo nameof(IgnoreSerialitzer)
        /// </summary>
        /// <typeparam name="T">Tipo a generar el serializador</typeparam>
        /// <param name="MetodoNew">Si el tipo no tiene un New() se debe de dar una forma de generar el tipo</param>
        /// <returns>Serializador del tipo indicado</returns>
         static ElementoBinario GetSerializador<T>(GetEmtpyNewObject<T> MetodoNew=null) 
        {
            return GetElementoComplejoAuto(typeof(T), () => MetodoNew != null ? MetodoNew() : typeof(T).GetObj());
        }

        /// <summary>
        /// Se usan las propiedades con Get y Set y las List,IDictionary,TwoKeysList(deben estar inicializadas desde el contructor) con solo Get además se ignorará las propiedades con el atributo nameof(IgnoreSerialitzer)
        /// </summary>
        /// <typeparam name="T">Tipo a generar el serializador</typeparam>
        /// <param name="MetodoNew">Si el tipo no tiene un New() se debe de dar una forma de generar el tipo</param>
        /// <returns>Serializador del tipo indicado</returns>
         static ElementoBinario GetElementoComplejoAuto(Type tipo,GetEmtpyNewObject MetodoNew = null)
        {
            IList<PropiedadTipo> propiedades;
            IList<ElementoBinario> serializadorPartes;
            GetPartsObjectMethod getParts;
            GetObjectMethod getObject;

            if (MetodoNew == null)
            {
                MetodoNew = (GetEmtpyNewObject)(() => { return tipo.GetObj(); });
                try
                {
                    MetodoNew();//asi no se tiene que probar el serializador para saber que no funciona, al obtenero ya se veria el problema
                }
                catch (Exception e) { throw e; }
            }
            propiedades = GetPropiedadesCompatibles(tipo.GetPropiedadesTipos());
            if (propiedades.Count == 0)
                throw new Exception($"El tipo {tipo.Name} no es compatible");
            serializadorPartes = GetSerializadorPartes(propiedades);
            getParts = (obj) => GetPartes(obj, propiedades);
            getObject = (iPartes) => {
                object obj = MetodoNew();
                SetPartes(obj, propiedades, iPartes);

                return obj;

            };

            return new ElementoComplejoBinarioNullableExt(serializadorPartes, getParts, getObject);
        }

        private static void SetPartes<T>(T obj, IList<PropiedadTipo> propiedades, object[] partes)
        {
            dynamic dic;
            IList lst,lst2;
         
            for (int i =0; i < propiedades.Count; i++)
            {
                if (propiedades[i].Uso.HasFlag(UsoPropiedad.Set))
                {
                    obj.SetProperty(propiedades[i].Nombre, partes[i]);
                }
                else if (propiedades[i].Tipo.ImplementInterficie(typeof(IDictionary<,>)))
                {
                    dic = obj.GetProperty(propiedades[i].Nombre);
                   foreach(dynamic pair in (dynamic)partes[i])
                   {
                         dic.Add(pair.Key,pair.Value);
                   }
                }
                else if (propiedades[i].Tipo.ImplementInterficie(typeof(IList<>)))
                {
                    lst = (IList)obj.GetProperty(propiedades[i].Nombre);
                    lst2 = (IList)partes[i];
                    for (int j = 0; j < lst2.Count; j++)
                        lst.Add(lst2[j]);
                }
            
            }
        }

        private static IList GetPartes(object obj, IList<PropiedadTipo> propiedades)
        {
            IDictionary dic;
            IList lst;
            object aux;
            object[] partes = new object[propiedades.Count];
   
            for (int i = 0; i < propiedades.Count; i++)
            {
                aux=obj.GetProperty(propiedades[i].Nombre);
                if (!propiedades[i].Uso.HasFlag(UsoPropiedad.Set))
                {
                    dic = aux as IDictionary;
                    if (dic != null)
                    {
                        aux = Extension.ToArray(dic);
                    }
                    else
                    {
                        lst = aux as IList;
                        if (lst != null)
                            aux = lst.ToArray();
                    }
                }
                partes[i] = aux;
            }
            return partes;
        }
        public abstract ElementoBinario Clon();
        private static IList<ElementoBinario> GetSerializadorPartes(IList<PropiedadTipo> propiedades)
        {
            return propiedades.Select((p) => GetSerializador(p.Tipo)).ToList();
        }

        private static IList<PropiedadTipo> GetPropiedadesCompatibles(IList<PropiedadTipo> list)
        {
            return list.Filtra((p) =>AccesibilidadOk(p)&&EsCompatible(p.Tipo));
        }


        public static ElementoBinario GetSerializador<T>()
        {
            return GetSerializador(typeof(T));
        }
        public static ElementoBinario GetSerializador(Type tipo)
        {
            ElementoBinario elemento;
            Type generic;
            Type[] parametros;
            Type[] parametrosSerializador;
            Type serialitzerType;

            if(DicTiposBasicos.ContainsKey(tipo.AssemblyQualifiedName))
            {
                elemento = ElementoBinario.ElementoTipoAceptado(Serializar.AssemblyToEnumTipoAceptado(tipo.AssemblyQualifiedName));
            }else if (tipo.ImplementInterficie(typeof(IElementoBinarioComplejo)))
            {
                elemento = ((IElementoBinarioComplejo)tipo.GetObj()).Serialitzer;
            }else if (SerializadoresTiposNoSoportados.ContainsKey(tipo.AssemblyQualifiedName))
            {
                elemento = SerializadoresTiposNoSoportados[tipo.AssemblyQualifiedName];
            }
            else
            {
                if (tipo.IsGenericType)
                {
                    generic = tipo.GetGenericTypeDefinition();
                    parametros = tipo.GetGenericArguments();
                    parametrosSerializador = parametros;
                    if (generic.ImplementInterficie(typeof(IDictionary)))
                    {
                        generic = typeof(IDictionary<,>);
                        parametros = new Type[] { tipo }.AfegirValors(parametros).ToArray();
                      
                    }
                    else if (generic.ImplementInterficie(typeof(IList)))
                    {
                        generic = typeof(IList<>);
                        parametros = new Type[] { tipo }.AfegirValors(parametros).ToArray();
                    }

                    serialitzerType = Type.GetType(DicTiposGenericos[generic.AssemblyQualifiedName]).SetTypes(parametros);
                    elemento = (ElementoBinario) serialitzerType.GetObj(parametrosSerializador.Select((p) => GetSerializador(p)).ToArray());
                }
                else if (tipo.IsArray&&tipo.GetElementType().AssemblyQualifiedName!=typeof(byte).AssemblyQualifiedName)
                {
                    serialitzerType = typeof(ElementoArrayBinario<>).SetTypes(tipo.GetElementType());
                    elemento = (ElementoBinario)serialitzerType.GetObj(GetSerializador(tipo.GetElementType()));
                }
                else
                {
                    if (DicTipos.ContainsKey(tipo.AssemblyQualifiedName))
                    {
                        elemento = DicTipos[tipo.AssemblyQualifiedName];
                    }
                    else if (GuardarSerializadoresAutosHechos&&SerializadoresElementosAutosHechos.ContainsKey(tipo.AssemblyQualifiedName))
                    {
                        elemento = SerializadoresElementosAutosHechos[tipo.AssemblyQualifiedName].Clon();
                    }
                    else
                    {
                        try
                        {
                            elemento = GetElementoComplejoAuto(tipo);//mirar si funciona
                            if(GuardarSerializadoresAutosHechos)
                                  SerializadoresElementosAutosHechos.Add(tipo.AssemblyQualifiedName, elemento.Clon());
                        }
                        catch { elemento = null; }
                    }
                }
            }
            return elemento;
        }
        public static bool EsCompatible(Type tipo)
        {                   //TiposMios //tener en cuenta los tipos que añaden en SerializadoresTiposNoSoportados
            bool compatible=DicTipos.ContainsKey(tipo.AssemblyQualifiedName)||SerializadoresTiposNoSoportados.ContainsKey(tipo.AssemblyQualifiedName);
            if (!compatible)   //TiposBasicos
                compatible = DicTiposBasicos.ContainsKey(tipo.AssemblyQualifiedName);
            if (!compatible)//Array Tipo  //  Las listas genericas mirar sus tipos
                compatible = (tipo.IsArray||  tipo.ImplementInterficie(typeof(IList<>))) && EsCompatible(tipo.GetElementType());
            if (!compatible)//los diccionarios
                compatible =tipo.IsGenericType && tipo.ImplementInterficie(typeof(IDictionary<,>)) && EsCompatible(tipo.GetGenericArguments().First()) && EsCompatible(tipo.GetGenericArguments().Last());
            if (!compatible)//KeyValuePair
                compatible = tipo.IsGenericType && tipo.GetGenericTypeDefinition().AssemblyQualifiedName.Equals(typeof(KeyValuePair<,>).AssemblyQualifiedName) && !tipo.GetGenericArguments().Select((t) => EsCompatible(t)).Any((esCompatible) => !esCompatible);
            if (!compatible) //TwoKeys
                compatible = tipo.IsGenericType && tipo.GetGenericTypeDefinition().AssemblyQualifiedName.Equals(typeof(TwoKeys<,>).AssemblyQualifiedName) && !tipo.GetGenericArguments().Select((t) => EsCompatible(t)).Any((esCompatible) => !esCompatible);
            if (!compatible)
            {
                compatible =GuardarSerializadoresAutosHechos&&SerializadoresElementosAutosHechos.ContainsKey(tipo.AssemblyQualifiedName);
                if (!compatible)
                {
                    //si es un tipo con propiedades compatibles es compatible
                    try
                    {   //mirar si hay recursividad infinita
                        GetElementoComplejoAuto(tipo);
                        compatible = true;
                    }
                    catch { }
                }
            }
            return compatible;
        }

        public static bool AccesibilidadOk(PropiedadTipo p)
        {
            bool correcto= !p.Atributos.Contains(new IgnoreSerialitzer());
            if (correcto)
            {
                //miro get
                correcto = p.Uso.HasFlag(UsoPropiedad.Get);
                if (correcto && p.Tipo.IsArray|| !( p.Tipo.ImplementInterficie(typeof(IList<>)) || p.Tipo.ImplementInterficie(typeof(IDictionary<,>)) ) )
                {//si no es una lista o un diccionario miro si tiene el set porque no es obligatorio
                  correcto= p.Uso.HasFlag(UsoPropiedad.Set);
                }
              
            }

            return correcto;
        }

     
    }
}