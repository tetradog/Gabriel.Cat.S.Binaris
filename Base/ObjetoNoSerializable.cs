using System;
using System.Collections.Generic;
using System.Text;

namespace Gabriel.Cat.S.Binaris
{
    public class ObjectNotSerializable : Exception
    {
        public ObjectNotSerializable(Type typeSerializable) : base("Only can serialitze " + typeSerializable.AssemblyQualifiedName)
        {
        }
    }
}
