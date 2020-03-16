using Gabriel.Cat.S.Binaris;
using Gabriel.Cat.S.Extension;
using Gabriel.Cat.S.Utilitats;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace TestBinaris
{
    [TestClass]
    public class KeyValuePairTest
    {
        [TestMethod]
        public void KeyValuePairTiposBasicos()
        {
            KeyValuePair<long, string> pair = new KeyValuePair<long, string>(125, "Gabriel");
            ElementoBinario serializador = ElementoBinario.GetSerializador(pair.GetType());
            byte[] data = serializador.GetBytes(pair);
            KeyValuePair<long, string> pairDeserializada = (KeyValuePair<long, string>)serializador.GetObject(data);
            Assert.IsTrue(pair.Key.Equals(pairDeserializada.Key) && pair.Value.Equals(pairDeserializada.Value));
        }
        [TestMethod]
        public void KeyValuePairTiposMios()
        {
            KeyValuePair<IdUnico, byte[]> pair = new KeyValuePair<IdUnico, byte[]>(new IdUnico(), Serializar.GetBytes("Gabriel"));
            ElementoBinario serializador = ElementoBinario.GetSerializador(pair.GetType());
            byte[] data = serializador.GetBytes(pair);
            KeyValuePair<IdUnico, byte[]> pairDeserializada = (KeyValuePair<IdUnico, byte[]>)serializador.GetObject(data);
            Assert.IsTrue(pair.Key.GetId().ArrayEqual(pairDeserializada.Key.GetId()) && pair.Value.ArrayEqual(pairDeserializada.Value));
        }
        [TestMethod]
        public void KeyValuePairTiposAnidados()
        {
            KeyValuePair<IdUnico, KeyValuePair<long, string>> pair = new KeyValuePair<IdUnico, KeyValuePair<long, string>>(new IdUnico(), new KeyValuePair<long, string>(125, "Gabriel"));
            ElementoBinario serializador = ElementoBinario.GetSerializador(pair.GetType());
            byte[] data = serializador.GetBytes(pair);
            KeyValuePair<IdUnico, KeyValuePair<long, string>> pairDeserializada = (KeyValuePair<IdUnico, KeyValuePair<long, string>>)serializador.GetObject(data);
            Assert.IsTrue(pair.Key.GetId().ArrayEqual(pairDeserializada.Key.GetId()) && pair.Value.Key.Equals(pairDeserializada.Value.Key) && pair.Value.Value.Equals(pairDeserializada.Value.Value));
        }
    }
}
