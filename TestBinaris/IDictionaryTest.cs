using Gabriel.Cat.S.Binaris;
using Gabriel.Cat.S.Utilitats;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace TestBinaris
{
    [TestClass]
    public class IDictionaryTest
    {
        [TestMethod]
        public void IDictionaryBasicos()
        {
            const int ELEMENTOS = 10;

            ElementoBinario serializador;
            byte[] data;
            SortedList<long, string> dic2;
            SortedList<long, string> dic = new SortedList<long, string>();
            for (int i = 0; i < ELEMENTOS; i++)
                dic.Add(DateTime.Now.Ticks, DateTime.Now.ToShortTimeString());

            serializador = ElementoBinario.GetElementoBinario(dic.GetType());
            data = serializador.GetBytes(dic);
            dic2 = (SortedList<long, string>)serializador.GetObject(data);
            Assert.IsTrue(Equals(dic, dic2));
        }
        [TestMethod]
        public void IDictionaryMios()
        {
            const int ELEMENTOS = 10;

            ElementoBinario serializador;
            byte[] data;
            SortedList<IdUnico, string> dic2;
            SortedList<IdUnico, string> dic = new SortedList<IdUnico, string>();
            for (int i = 0; i < ELEMENTOS; i++)
                dic.Add(new IdUnico(), DateTime.Now.ToShortTimeString());

            serializador = ElementoBinario.GetElementoBinario(dic.GetType());
            data = serializador.GetBytes(dic);
            dic2 = (SortedList<IdUnico, string>)serializador.GetObject(data);
            Assert.IsTrue(Equals(dic, dic2));
        }
        static bool Equals(IDictionary dic,IDictionary dic2)
        {
            
            IEnumerator keys = dic.Keys.GetEnumerator();
            IEnumerator keys2 = dic2.Keys.GetEnumerator();
            bool equals = dic.Count==dic2.Count;
            for(int i=0;i<dic.Count&& equals; i++)
            {
                keys.MoveNext();
                keys2.MoveNext();
                equals = keys.Current.Equals(keys2.Current)&&dic[keys.Current].Equals(dic2[keys2.Current]);

            }
            return equals;
        }
    }
}
