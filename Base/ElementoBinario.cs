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
    public abstract class ElementoBinario
    {
        static readonly ByteArrayBinario byteArrayBinario = new ByteArrayBinario();
        static readonly LlistaOrdenada<string, ElementoBinario> DicTipos;
        static readonly LlistaOrdenada<string, string> DicTiposGenericos;
        public const byte NULL = 0x0;
        public const byte NOTNULL = 0x1;
        static ElementoBinario()
        {
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
        /// Se usan las propiedades con Get y Set y las List,IDictionary,TwoKeysList con solo Get además se ignorará las propiedades con el atributo nameof(IgnoreSerialitzer)
        /// </summary>
        /// <typeparam name="T">Tipo a generar el serializador</typeparam>
        /// <param name="MetodoNew">Si el tipo no tiene un New() se debe de dar una forma de generar el tipo</param>
        /// <returns></returns>
        public static ElementoBinario GetSerializador<T>(GetEmtpyNewObject<T> MetodoNew=null)
        
        {
            IList<PropiedadTipo> propiedades;
            IList<ElementoBinario> partes;
            GetPartsObjectMethod getParts;
            GetObjectMethod getObject;
          
            if (MetodoNew == null)
                MetodoNew = (GetEmtpyNewObject<T>)(() => { return (T)typeof(T).GetObj(); });
           
            propiedades =GetPropiedadesCompatibles(typeof(T).GetPropiedadesTipos());
       
            partes = GetPartes(propiedades);
            getParts=(obj)=>GetPartes(obj,propiedades);
            getObject = (iPartes) => {
                 T obj= MetodoNew();
                 SetPartes(obj, propiedades, iPartes);

                return obj;
            
            };

            return new ElementoComplejoBinarioNullableExt(partes,getParts,getObject);
        }

       

        private static void SetPartes<T>(T obj, IList<PropiedadTipo> propiedades, object[] partes)
        {
           for(int i =0; i < propiedades.Count; i++)
            {
                obj.SetProperty(propiedades[i].Nombre, partes[i]);
            }
        }

        private static IList GetPartes(object obj, IList<PropiedadTipo> propiedades)
        {
            object[] partes = new object[propiedades.Count];
            for (int i = 0; i < propiedades.Count; i++)
            {
                partes[i]=obj.GetProperty(propiedades[i].Nombre);
            }
            return partes;
        }

        private static IList<ElementoBinario> GetPartes(IList<PropiedadTipo> propiedades)
        {
            return propiedades.Select((p) => GetElementoBinario(p.Tipo)).ToList();
        }

        private static IList<PropiedadTipo> GetPropiedadesCompatibles(IList<PropiedadTipo> list)
        {
            return list.Filtra((p) =>AccesibilidadOk(p)&&EsCompatible(p.Tipo));
        }



        private static ElementoBinario GetElementoBinario(Type tipo)
        {
            ElementoBinario elemento;

            if(Gabriel.Cat.S.Utilitats.Serializar.AsseblyQualifiedName.Contains(tipo.AssemblyQualifiedName))
            {
                elemento = ElementoBinario.ElementoTipoAceptado(Serializar.AssemblyToEnumTipoAceptado(tipo.AssemblyQualifiedName));
            }else if (tipo.ImplementInterficie(typeof(IElementoBinarioComplejo)))
            {
                elemento = ((IElementoBinarioComplejo)tipo.GetObj()).Serialitzer;
            }
            else
            {

                //es un tipo mio
                elemento = null;
            }
            return elemento;
        }
        private static bool EsCompatible(Type tipo)
        {
            bool compatible=true;

            //KeyValuePair,TwoKeys
            //TiposBasicos
            //TiposMios
            //Array Tipo
            //Las listas genericas y diccionarios mirar sus tipos
            return compatible;
        }

        private static bool AccesibilidadOk(PropiedadTipo p)
        {
            bool correcto= !p.Atributos.Contains(new IgnoreSerialitzer());
            if (correcto)
            {
                //miro get
                correcto = p.Uso.HasFlag(UsoPropiedad.Get);
                if (correcto&&(p.Tipo.IsArray||!(p.Tipo.ImplementInterficie(typeof(IList)) || p.Tipo.ImplementInterficie(typeof(IDictionary)))))
                {//si no es una lista o un diccionario miro si tiene el set
                  correcto= p.Uso.HasFlag(UsoPropiedad.Set);
                }
              
            }

            return correcto;
        }
    }
}