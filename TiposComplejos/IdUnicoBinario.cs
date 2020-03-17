using Gabriel.Cat.S.Utilitats;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Gabriel.Cat.S.Binaris
{
    public class IdUnicoBinario : ElementoComplejoBinarioNullable
    {
        public IdUnicoBinario() : base(new ElementoBinario[] {new ByteArrayBinario() }){

        }
        protected override IList IGetPartsObject(object obj)
        {
            IdUnico id = obj as IdUnico;
            if (id == null)
                throw new Exception("Se esperaba un IdUnico");
            return new object[]{id.GetId()};
        }

        protected override object JGetObject(MemoryStream bytes)
        {
            return new IdUnico((byte[])GetPartsObject(bytes)[0]);
        }
        public override string ToString()
        {
            return "TipoDatos=IdUnico";
        }
        public override ElementoBinario Clon()
        {
            return new IdUnicoBinario();
        }
    }
}
