using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabriel.Cat.S.Extension
{
    public static class Extension
    {
        public static object GetObj(this Type type,params object[] partes)
        {
            return Activator.CreateInstance(type,partes);
        }
        public static Type SetTypes(this Type type,params Type[] types)
        {
           return type.MakeGenericType(types);
        }
        public static KeyValuePair<object,object>[] ToArray(this IDictionary dic)
        {
            IEnumerator keys = dic.Keys.GetEnumerator();
            IEnumerator values =dic.Values.GetEnumerator();
            KeyValuePair<object, object>[] valuePairs = new KeyValuePair<object, object>[dic.Count];
            for(int i = 0; i < valuePairs.Length; i++)
            {
                valuePairs[i] = new KeyValuePair<object, object>(keys.Current,values.Current);
                keys.MoveNext();
                values.MoveNext();
            }
            return valuePairs;
        }
        public static object[] ToArray(this IList lst)
        {
            object[] objs = new object[lst.Count];
            for (int i = 0; i < objs.Length; i++)
                objs[i] = lst[i];
            return objs;
        }
    }
}
