using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hola.Structures
{
    class Bor<T>
    {
        Dictionary<T, Bor<T>> Nodes = new Dictionary<T, Bor<T>>();

        public void Push(T[] array, int index)
        {
            if (index >= array.Length) return;

            Bor<T> node;
            if (!Nodes.TryGetValue(array[index], out node))
            {
                node = new Bor<T>();
                Nodes[array[index]] = node;
            }

            node.Push(array, index + 1);
        }
        public int GetDeep(T[] array, int index)
        {
            if (index >= array.Length) return 0;

            Bor<T> node;
            if (!Nodes.TryGetValue(array[index], out node))
            {
                return 0;
            }

            return node.GetDeep(array, index + 1) + 1;
        }
    }
}
