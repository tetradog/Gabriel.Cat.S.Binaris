using System;
using System.Collections.Generic;
using System.Text;
using Gabriel.Cat.S.Extension;
namespace Gabriel.Cat.S.Binaris
{
    public class ElementoSinBase<TIdTipo, TIdElemento, TId> : IElementoBinarioComplejo
    {
        const byte NOCOINCIDE = 0x1;
        const byte COINCICE = 0x0;
        public static readonly ElementoBinario Serializador = ElementoBinario.GetSerializador<ElementoSinBase<TIdTipo, TIdElemento, TId>>();

        public bool SinBase { get; set; }
        public TIdTipo IdTipo { get; set; }
        public TIdElemento IdElemento { get; set; }
        public TId Id { get; set; }
        public byte[] BytesSinBase { get; set; }

        ElementoBinario IElementoBinarioComplejo.Serialitzer => Serializador;

        public ElementoSinBase() { }
        public ElementoSinBase(TIdTipo idTipo, TIdElemento idElemento, TId id, IElementoBinarioComplejo elementoBase, IElementoBinarioComplejo elementoCompleto) : this(idTipo, idElemento, id, elementoBase.Serialitzer.GetBytes(elementoBase), elementoCompleto.Serialitzer.GetBytes(elementoCompleto))
        { }
        public ElementoSinBase(TIdTipo idTipo, TIdElemento idElemento, TId id, byte[] bytesBase, byte[] bytesCompletos)
        {
            this.Id = id;
            this.IdTipo = idTipo;
            this.IdElemento = idElemento;
            GetBytesSinBase(bytesBase, bytesCompletos);
        }
        public string Name
        {
            get
            {
                return String.Format("{0}_{1}_{2}", Id, IdTipo, IdElemento);
            }
        }
        public byte[] GetBytesCompletos(byte[] bytesBase)
        {
            if (bytesBase.Length < BytesSinBase.Length - bytesBase.Length)
                throw new BaseIncorrectaException();

            byte[] bytesCompletos = new byte[bytesBase.Length];
            unsafe
            {
                bytesCompletos.UnsafeMethod((ptrBytesCompletos) =>
                {
                    bytesBase.UnsafeMethod((ptrBytesBase) =>
                    {

                        BytesSinBase.UnsafeMethod((ptrBytesSinBase) =>
                        {

                            for (int i = 0; i < bytesCompletos.Length; i++)
                            {
                                if (*ptrBytesSinBase.PtrArray == NOCOINCIDE)
                                {
                                    ptrBytesSinBase.PtrArray++;
                                    *ptrBytesCompletos.PtrArray = *ptrBytesSinBase.PtrArray;
                                }
                                else
                                {
                                    *ptrBytesCompletos.PtrArray = *ptrBytesBase.PtrArray;
                                }

                                ptrBytesCompletos.PtrArray++;
                                ptrBytesBase.PtrArray++;
                                ptrBytesSinBase.PtrArray++;
                            }

                        });

                    });
                });
            }
            return bytesCompletos;
        }

        public void GetBytesSinBase(byte[] bytesBase, byte[] bytesCompletos)
        {//cuando funcione todo lo optimizo
            if (bytesBase.Length != bytesCompletos.Length)
                throw new BaseIncorrectaException();

            int bytesSinCoincidir = 0;
            SinBase = true;
            for (int i = 0; i < bytesBase.Length; i++)
                if (bytesBase[i] != bytesCompletos[i])
                    bytesSinCoincidir++;
            BytesSinBase = new byte[bytesBase.Length + bytesSinCoincidir];
            for (int i = 0, j = 0; i < bytesCompletos.Length; i++)
            {
                if (bytesBase[i] == bytesCompletos[i])
                    BytesSinBase[j] = COINCICE;
                else
                {
                    BytesSinBase[j++] = NOCOINCIDE;
                    BytesSinBase[j] = bytesCompletos[i];
                }
                j++;
            }
        }
        public void SetBytesConBase(byte[] bytesCompletos)
        {
            SinBase = false;
            BytesSinBase = bytesCompletos;
        }
        public override string ToString()
        {
            return Name;
        }
    }
    public class ElementoSinBase : ElementoSinBase<int, int, int>
    {
        public ElementoSinBase() { }
        public ElementoSinBase(int idTipo, int idElemento, int id, IElementoBinarioComplejo elementoBase, IElementoBinarioComplejo elementoCompleto) : base(idTipo, idElemento, id, elementoBase, elementoCompleto)
        {
        }

        public ElementoSinBase(int idTipo, int idElemento, int id, byte[] bytesBase, byte[] bytesCompletos) : base(idTipo, idElemento, id, bytesBase, bytesCompletos)
        {
        }
    }
    public class BaseIncorrectaException : Exception
    { }
    public static class ElementoBinarioSinBaseExtension
    {
        public static T GetCompleto<T, T1, T2, T3>(this ElementoSinBase<T1, T2, T3> elementoSinBase, T tipoCompleto) where T : IElementoBinarioComplejo
        {
            return (T)tipoCompleto.Serialitzer.GetObject(elementoSinBase.GetBytesCompletos(tipoCompleto.Serialitzer.GetBytes(tipoCompleto)));
        }
        public static T GetCompleto<T>(this ElementoSinBase elementoSinBase, T tipoCompleto) where T : IElementoBinarioComplejo
        {
            return ((ElementoSinBase<int, int, int>)elementoSinBase).GetCompleto(tipoCompleto);
        }

    }
}
