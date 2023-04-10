using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    public class ReverseIterate<T> : IEnumerable<T>
    {
        private List<T> list;
        private int index;

        public ReverseIterate()
        {
            this.list = new List<T>();
            index = -1;
        }

        public ReverseIterate(List<T> list)
        {
            this.list = list;
            index = list.Count - 1;
        }

        public bool IsLastAction()
        {
            return (index == -1);
        }

        public T Next()
        {
            if (index >= 0)
            {
                T item = list[index];
                index--;
                return item;
            }
            return default(T);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                yield return list[i];
            }
        }

        public void Clear()
        {
            list.Clear();
            index = -1;
        }
    }
}
