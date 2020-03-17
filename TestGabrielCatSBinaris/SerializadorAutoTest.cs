using Gabriel.Cat.S.Binaris;
using Gabriel.Cat.S.Utilitats;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestGabrielCatSBinaris
{
    [TestClass]
   public class SerializadorAutoTest
    {
        public class ClaseSimple
        {
            public string Texto { get; set; }
            public IdUnico Id { get; set; }
            public override bool Equals(object obj)
            {
                ClaseSimple other = obj as ClaseSimple;
                return other == null ? false : Equals(Texto, other.Texto) && Equals(Id, other.Id);
            }
        }
        public class ClaseCompleja
        {
            public ClaseSimple Clase { get; set; }
            public override bool Equals(object obj)
            {
                ClaseCompleja other = obj as ClaseCompleja;
                return other==null?false:other.Clase.Equals(other.Clase);
            }
        }
        public class ClaseConDiccionario
        {
            public LlistaOrdenada<long, string> Dic { get; set; }
            public override bool Equals(object obj)
            {
                ClaseConDiccionario other = obj as ClaseConDiccionario;
                return other == null ? false : other.Dic.Equals(other.Dic);
            }
        }
        public class ClaseConLista
        {
            public List<string> Lista { get; set; }
            public override bool Equals(object obj)
            {
                ClaseConLista other = obj as ClaseConLista;
                return other == null ? false : other.Lista.Equals(other.Lista);
            }
        }
        public class ClaseConDiccionarioSinSet
        {
            public LlistaOrdenada<long, string> Dic { get;private set; } = new LlistaOrdenada<long, string>();
            public override bool Equals(object obj)
            {
                ClaseConDiccionarioSinSet other = obj as ClaseConDiccionarioSinSet;
                return other == null ? false : other.Dic.Equals(other.Dic);
            }
        }
        public class ClaseConListaSinSet
        {
            public List<string> Lista { get; private set; } = new List<string>();
            public override bool Equals(object obj)
            {
                ClaseConListaSinSet other = obj as ClaseConListaSinSet;
                return other == null ? false : other.Lista.Equals(other.Lista);
            }
        }
        [TestMethod]
        public void UnaClaseSimple()
        {
            Assert.IsTrue(TestClases(() => new ClaseSimple() { Id = new IdUnico(), Texto = "Id" } ));
        }
        [TestMethod]
        public void UnaClaseCompleja()
        {
            Assert.IsTrue(TestClases(() => new ClaseCompleja() { Clase = new ClaseSimple() { Id = new IdUnico(), Texto = "Id2" } }));
        }
        [TestMethod]
        public void UnaClaseConDiccionario()
        {
            const int ELEMENTOS = 10;
            LlistaOrdenada<long, string> dic = new LlistaOrdenada<long, string>();

            for (int i = 0; i < ELEMENTOS; i++)
                dic.Add(DateTime.Now.Ticks, i + "");

            Assert.IsTrue(TestClases(() => new ClaseConDiccionario() { Dic = dic }));
        }
        [TestMethod]
        public void UnaClaseConLista()
        {
            const int ELEMENTOS = 10;
            List<string> lista = new List<string>();

            for (int i = 0; i < ELEMENTOS; i++)
                lista.Add(i + ":" + DateTime.Now.Ticks);

            Assert.IsTrue(TestClases(() => new ClaseConLista() { Lista =lista }));
        }
        [TestMethod]
        public void UnaClaseConDiccionarioSinSet()
        {
            const int ELEMENTOS = 10;
            Assert.IsTrue(TestClases(() => {
                ClaseConDiccionarioSinSet clase = new ClaseConDiccionarioSinSet();
                for (int i = 0; i < ELEMENTOS; i++)
                    clase.Dic.Add(DateTime.Now.Ticks, i + "");
                return clase;
            }));
        }
        [TestMethod]
        public void UnaClaseConListaSinSet()
        {
            const int ELEMENTOS = 10;

            Assert.IsTrue(TestClases(() =>{
                ClaseConListaSinSet clase=new ClaseConListaSinSet();
                for (int i = 0; i < ELEMENTOS; i++)
                    clase.Lista.Add(i + ":" + DateTime.Now.Ticks);
                return clase;
            }));
        }
   
        static bool TestClases<T>(GetElement<T> getElement)
        {
            T clase = getElement();
            ElementoBinario serializador = ElementoBinario.GetSerializador<T>();
            byte[] data = serializador.GetBytes(clase);
            T claseDeserializada =(T) serializador.GetObject(data);
            return Equals(clase, claseDeserializada);
        }
    }
}
