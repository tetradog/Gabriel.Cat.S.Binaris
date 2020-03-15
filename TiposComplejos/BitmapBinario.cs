using Gabriel.Cat.S.Utilitats;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace Gabriel.Cat.S.Binaris
{
    public class BitmapBinario : ElementoComplejoBinarioNullable
    {
        public BitmapBinario()
        {
            Partes.Add(new ByteArrayBinario());
        }


        #region implemented abstract members of ElementoComplejoBinarioNullable


        protected override IList IGetPartsObject(object obj)
        {
            Bitmap bmp = obj as Bitmap;
            if (bmp == null)
                throw new Exception(string.Format("Se ha intentado leer {0} como System.Drawing.Bitmap", obj.GetType().FullName));
            return new object[] { Serializar.GetBytes(bmp) };
        }


        #endregion
        protected override object JGetObject(MemoryStream bytes)
        {

            byte[] bytesBmp = (byte[])base.GetPartsObject(bytes)[0];

            Bitmap bmp;
            try
            {
                bmp = (Bitmap)Bitmap.FromStream(new MemoryStream(bytesBmp));
            }
            catch
            {

                throw new Exception("El formato no coincide con un Bitmap");
            }
            return bmp;
        }
        public override string ToString()
        {
            return "TipoDatos=Bitmap";
        }


     
    }

}

