using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hola.Structures
{
    class SuffixTree<T>
    {
        Dictionary<T, SuffixTree<T>> Nodes = new Dictionary<T, SuffixTree<T>>();

        public void Push(T[] array, int index)
        {
            if (index >= array.Length) return;

            SuffixTree<T> node;
            if (!Nodes.TryGetValue(array[index], out node))
            {
                node = new SuffixTree<T>();
                Nodes[array[index]] = node;
            }

            node.Push(array, index + 1);
        }
        public int GetDeep(T[] array, int index)
        {
            if (index >= array.Length) return 0;

            SuffixTree<T> node;
            if (!Nodes.TryGetValue(array[index], out node))
            {
                return 0;
            }

            return node.GetDeep(array, index + 1) + 1;
        }
    }
}
