using Gabriel.Cat.S.Binaris;
using Gabriel.Cat.S.Extension;
using Gabriel.Cat.S.Utilitats;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestGabrielCatSBinaris
{
    [TestClass]
    public class IListTest
    {
        [TestMethod]
        public void IListBasic()
        {
            const int ELEMENTS = 10;
            byte[] data;
            List<long> lstDeserializada;
            List<long> lst = new List<long>();
            ElementoBinario serializador=ElementoBinario.GetElementoBinario(lst.GetType());
            for (int i = 0; i < ELEMENTS; i++)
                lst.Add(DateTime.Now.Ticks);
            data = serializador.GetBytes(lst);
            lstDeserializada = (List<long>)serializador.GetObject(data);
            Assert.IsTrue(lst.AreEquals(lstDeserializada));

        }
        [TestMethod]
        public void IListMio()
        {
            const int ELEMENTS = 10;
            byte[] data;
            List<IdUnico> lstDeserializada;
            List<IdUnico> lst = new List<IdUnico>();
            ElementoBinario serializador = ElementoBinario.GetElementoBinario(lst.GetType());
            for (int i = 0; i < ELEMENTS; i++)
                lst.Add(new IdUnico());
            data = serializador.GetBytes(lst);
            lstDeserializada =(List<IdUnico>)serializador.GetObject(data);
            Assert.IsTrue(lst.AreEquals(lstDeserializada));

        }
        [TestMethod]
        public void IListMioAnidada()
        {
            const int ELEMENTS = 10;
            byte[] data;
            List<IdUnico> aux;
            List<List<IdUnico>> lstDeserializada;
            List<List<IdUnico>> lst = new List<List<IdUnico>>();
            ElementoBinario serializador = ElementoBinario.GetElementoBinario(lst.GetType());
            for (int i = 0; i < ELEMENTS; i++)
            {
                aux = new List<IdUnico>();
                for (int j = 0; j < ELEMENTS; j++)
                    aux.Add(new IdUnico());
                lst.Add(aux);
            }
            data = serializador.GetBytes(lst);
            lstDeserializada = (List<List<IdUnico>>)serializador.GetObject(data);
            Assert.IsTrue(Equals(lst,lstDeserializada));

        }
        static bool Equals<T>(List<List<T>> lst,List<List<T>> lst2)
        {
            bool equals = lst.Count == lst2.Count;
            for (int i = 0; i < lst.Count && equals; i++)
                equals = lst[i].AreEquals(lst2[i]);
            return equals;
        }
    }
}
