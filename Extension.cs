using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabriel.Cat.S.Binaris
{
    public static class Extension
    {

        public static KeyValuePair<object, object>[] ToArray(IDictionary dic)
        {
            IEnumerator keys = dic.Keys.GetEnumerator();
            IEnumerator values = dic.Values.GetEnumerator();
            KeyValuePair<object, object>[] valuePairs = new KeyValuePair<object, object>[dic.Count];
            for (int i = 0; i < valuePairs.Length; i++)
            {
                valuePairs[i] = new KeyValuePair<object, object>(keys.Current, values.Current);
                keys.MoveNext();
                values.MoveNext();
            }
            return valuePairs;
        }

    }
}
