using Gabriel.Cat.S.Binaris;
using Gabriel.Cat.S.Extension;
using Gabriel.Cat.S.Utilitats;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestGabrielCatSBinaris
{
    [TestClass]
    public class ArrayTest
    {
        [TestMethod]
        public void ArrayBasicUnaDimension()
        {
            const int ELEMENTOS = 10;
            ElementoBinario serializador;
            byte[] data;
            long[] arrayDeserializada;
            long[] array = new long[ELEMENTOS];

            for (int i = 0; i < ELEMENTOS; i++)
                array[i] = DateTime.Now.Ticks;
            serializador = ElementoBinario.GetElementoBinario(array.GetType());
            data = serializador.GetBytes(array);
            arrayDeserializada = (long[])serializador.GetObject(data);
            Assert.IsTrue(array.AreEquals(arrayDeserializada));
        }
        [TestMethod]
        public void ArrayMioUnaDimension()
        {
            const int ELEMENTOS = 10;
            ElementoBinario serializador;
            byte[] data;
            IdUnico[] arrayDeserializada;
            IdUnico[] array = new IdUnico[ELEMENTOS];

            for (int i = 0; i < ELEMENTOS; i++)
                array[i] = new IdUnico();
            serializador = ElementoBinario.GetElementoBinario(array.GetType());
            data = serializador.GetBytes(array);
            arrayDeserializada = (IdUnico[])serializador.GetObject(data);
            Assert.IsTrue(array.AreEquals(arrayDeserializada));
        }
    }
}
