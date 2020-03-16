using Gabriel.Cat.S.Binaris;
using Gabriel.Cat.S.Extension;
using Gabriel.Cat.S.Utilitats;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestBinaris
{
    [TestClass]
    public class TwoKeysTest
    {
        [TestMethod]
        public void TwoKeysTiposBasicos()
        {
            TwoKeys<long, string> pair = new TwoKeys<long, string>(125, "Gabriel");
            ElementoBinario serializador = ElementoBinario.GetElementoBinario(pair.GetType());
            byte[] data = serializador.GetBytes(pair);
            TwoKeys<long, string> pairDeserializada = (TwoKeys<long, string>)serializador.GetObject(data);
            Assert.IsTrue(pair.Key1.Equals(pairDeserializada.Key1) && pair.Key2.Equals(pairDeserializada.Key2));
        }
        [TestMethod]
        public void TwoKeysTiposMios()
        {
            TwoKeys<IdUnico, byte[]> pair = new TwoKeys<IdUnico, byte[]>(new IdUnico(), Serializar.GetBytes("Gabriel"));
            ElementoBinario serializador = ElementoBinario.GetElementoBinario(pair.GetType());
            byte[] data = serializador.GetBytes(pair);
            TwoKeys<IdUnico, byte[]> pairDeserializada = (TwoKeys<IdUnico, byte[]>)serializador.GetObject(data);
            Assert.IsTrue(pair.Key1.GetId().ArrayEqual(pairDeserializada.Key1.GetId()) && pair.Key2.ArrayEqual(pairDeserializada.Key2));
        }
        [TestMethod]
        public void KeyValuePairTiposAnidados()
        {
            TwoKeys<IdUnico, TwoKeys<long, string>> pair = new TwoKeys<IdUnico, TwoKeys<long, string>>(new IdUnico(), new TwoKeys<long, string>(125, "Gabriel"));
            ElementoBinario serializador = ElementoBinario.GetElementoBinario(pair.GetType());
            byte[] data = serializador.GetBytes(pair);
            TwoKeys<IdUnico, TwoKeys<long, string>> pairDeserializada = (TwoKeys<IdUnico, TwoKeys<long, string>>)serializador.GetObject(data);
            Assert.IsTrue(pair.Key1.GetId().ArrayEqual(pairDeserializada.Key1.GetId()) && pair.Key2.Key1.Equals(pairDeserializada.Key2.Key1) && pair.Key2.Key2.Equals(pairDeserializada.Key2.Key2));
        }
    }
}
