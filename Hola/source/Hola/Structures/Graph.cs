using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hola.Structures
{
    class Graph<T>
    {
        public Graph()
        {

        }
        public Graph(IEnumerable<T> verticies)
        {
            foreach(var vertex in verticies)
            {
                AddVertex(vertex);
            }
        }

        Dictionary<T, HashSet<T>> VerticiesDict = new Dictionary<T, HashSet<T>>();
        public int Count
        {
            get
            {
                return VerticiesDict.Count;
            }
        }
        public IEnumerable<T> Verticies
        {
            get
            {
                foreach(var pair in VerticiesDict)
                {
                    yield return pair.Key;
                }
            }
        }

        public bool Contains(T vertex)
        {
            return VerticiesDict.ContainsKey(vertex);
        }
        public void AddVertex(T vertex)
        {
            if (Contains(vertex)) return;

            VerticiesDict.Add(vertex, new HashSet<T>());
        }
        public void AddEdge(T vertex1, T vertex2)
        {
            if (!Contains(vertex1)) throw new Exception("vertex1 not exist");
            if (!Contains(vertex2)) throw new Exception("vertex2 not exist");

            VerticiesDict[vertex1].Add(vertex2);
            VerticiesDict[vertex2].Add(vertex1);
        }
        public void RemoveEdge(T vertex1, T vertex2)
        {
            if (!Contains(vertex1)) throw new Exception("vertex1 not exist");
            if (!Contains(vertex2)) throw new Exception("vertex2 not exist");

            VerticiesDict[vertex1].Remove(vertex2);
            VerticiesDict[vertex2].Remove(vertex1);
        }

        public Graph<T>[] GetConnectedComponents()
        {
            var used = new HashSet<T>();

            var r = new List<Graph<T>>();
            var q = new Queue<T>();
            foreach (var vertex in Verticies)
            {
                if (!used.Contains(vertex))
                {
                    var g = new Graph<T>();

                    used.Add(vertex);
                    g.AddVertex(vertex);

                    q.Enqueue(vertex);
                    while (q.Count > 0)
                    {
                        var v = q.Dequeue();

                        foreach(var v2 in VerticiesDict[v])
                        {
                            if (!used.Contains(v2))
                            {
                                used.Add(v2);

                                g.AddVertex(v2);
                                g.AddEdge(v, v2);

                                q.Enqueue(v2);
                            }
                        }
                    }

                    r.Add(g);
                }

            }
            return r.ToArray();
        }
    }
}
