using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabriel.Cat.S.Extension
{
    public static class Extension
    {
        public static object GetObj(this Type type,params object[] partes)
        {
            return type.GetConstructor(partes.Select((p)=>p.GetType()).ToArray()).Invoke(partes);
        }

    }
}
