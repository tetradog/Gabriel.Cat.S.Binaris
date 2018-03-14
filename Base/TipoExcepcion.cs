using System;
using System.Collections.Generic;
using System.Text;

namespace Gabriel.Cat.S.Binaris
{
    public class TipoException : Exception
    {
        public TipoException() : base()
        {
        }

        public TipoException(string message) : base(message)
        {
        }
    }
}
