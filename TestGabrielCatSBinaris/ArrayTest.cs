using Gabriel.Cat.S.Binaris;
using Gabriel.Cat.S.Extension;
using Gabriel.Cat.S.Utilitats;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace TestGabrielCatSBinaris
{
    public delegate T GetElement<T>();
    [TestClass]
    public class ArrayTest
    {
        [TestMethod]
        public void ArrayBasicUnaDimension()
        {
            Assert.IsTrue(TestAny<long>(() => DateTime.Now.Ticks));
        }
        [TestMethod]
        public void ArrayMioUnaDimension()
        {
            Assert.IsTrue(TestAny<IdUnico>(() => new IdUnico()));
        }
        [TestMethod]
        public void ArrayMioAnidadaUnaDimension()
        {
            Assert.IsTrue(TestAny<IdUnico[]>(() =>
            {
                return GetArray<IdUnico>(() => new IdUnico());
            }));
        }
        [TestMethod]
        public void ArrayBasicDosDimensiones()
        {
          Assert.IsTrue(TestDosDimensiones<long>(() => DateTime.Now.Ticks));
        }
        [TestMethod]
        public void ArrayMioUDosDimensiones()
        {
            Assert.IsTrue(TestDosDimensiones<IdUnico>(() => new IdUnico()));
        }
        [TestMethod]
        public void ArrayAnidadoDosDimensiones()
        {
           
            Assert.IsTrue(TestDosDimensiones<IdUnico[]>(() =>
            {
                return GetArray<IdUnico>(() => new IdUnico());
            }));
        }
        static T[] GetArray<T>(GetElement<T> getElement)
        {
            const int ELEMENTOS = 10;
            T[] array = new T[ELEMENTOS];
            for (int i = 0; i < array.Length; i++)
                array[i] = getElement();
            return array;
        }
        static bool TestDosDimensiones<T>(GetElement<T> getElement)
        {
            const int ELEMENTOS = 10;
            return TestAny<T[,]>(() =>
            {
                T[,] array=new T[ELEMENTOS,ELEMENTOS];
                for (int i = 0; i < ELEMENTOS; i++)
                 {
                    for (int j = 0; j < ELEMENTOS; j++)
                        array[i,j] = getElement();
                }
                return array;
            });
        }
        static bool TestAny<T>(GetElement<T> getElement)
        {
            const int ELEMENTOS = 10;
            ElementoBinario serializador;
            byte[] data;
            T[] arrayDeserializada;
            T[] array = new T[ELEMENTOS];
            for (int k = 0; k < ELEMENTOS; k++)
            {
                array[k] = getElement();
            }
            serializador = ElementoBinario.GetElementoBinario(array.GetType());
            data = serializador.GetBytes(array);
            arrayDeserializada = (T[])serializador.GetObject(data);
           return Equals(array,arrayDeserializada);
        }
        static bool Equals(Array array,Array array2)
        {
            bool equals = array.Length == array2.Length;
            IEnumerator a1 = array.GetEnumerator();
            IEnumerator a2 = array2.GetEnumerator();
            while (equals && a1.MoveNext() && a2.MoveNext())
                equals = Equals(a1.Current, a2.Current);
            return equals;
        }
        static bool Equals(object obj1,object obj2)
        {
            bool equals;
            if (obj1 is Array)
            {
                equals = Equals((Array)obj1, (Array)obj2);
            }
            else equals = obj1.Equals(obj2);
            return equals;
        }
  
    }
}
