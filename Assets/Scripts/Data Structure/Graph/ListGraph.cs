using ED262C;
using UnityEngine;

public class ListGraph<T> : IGraph<T>
{
    SimpleArrayDictionary<T, SimpleArrayList<Edge<T>>> adjacencyList;

    public ListGraph()
    {
        adjacencyList = new SimpleArrayDictionary<T, SimpleArrayList<Edge<T>>>();
    }

    public T[] vertices => adjacencyList.Keys();

    public bool AddEdge(T from, T to, int weight)
    {
        //agregamos vertices, si ya estan siguen de largo por el TryAdd()
        AddVertex(from);
        AddVertex(to);

        //buscamos el node de from a to
        Edge<T> edge = GetEdge(from, to);

        //si no existe el edge, lo creamos
        if (edge == null)
        {
            adjacencyList[from].Add(new Edge<T>(to, weight));
            return true;
        }

        //si el peso no es igual, lo actualizamos
        if (edge.weight != weight)
        {
            edge.weight = weight;
            return true;
        }

        //Si llegamos aca, todo es igual, no cambia
        return false;
    }

    public bool AddVertex(T vertex)
    {
        return adjacencyList.TryAdd(vertex, new SimpleArrayList<Edge<T>>());
    }

    public bool ContainsEdge(T from, T to)
    {
        return GetEdge(from, to) != null;
    }

    public bool ContainsVertex(T vertex)
    {
        return adjacencyList.ContainsKey(vertex); //me fijo si esta la key en el diccionario
    }

    public ISimpleList<Edge<T>> GetNeighbours(T vertex)
    {
        if (vertex == null) return null;
        if(ContainsVertex(vertex))
        {
            return adjacencyList[vertex]; //devuelve la lista de edges que esten conectados a ese vertex
        }
        return null;
    }

    public int GetWeight(T from, T to)
    {
        //buscamos la arista entre from y to
        Edge<T> targetEdge = GetEdge(from, to);

        //Si no existe, devolvemos -1 (valor imposible)
        if (targetEdge == null) return -1;

        //devolvemos el peso del edge devuelto por GetEdge
        return targetEdge.weight;
    }

    public bool RemoveEdge(T from, T to)
    {
        //no aseguramos que exista el vertice
        if(!ContainsVertex(from)) return false;

        //buscamos edge a remover
        Edge<T> edge = GetEdge(from, to);

         //si no existe devolvemos null
        if (edge == null) return false;

        //Si estan ambos, lo removemos y devolvemos true
        adjacencyList[from].Remove(edge);
        return true;
    }

    public bool RemoveVertex(T vertex)
    {
        if (!ContainsVertex(vertex)) return false;

        for (int i = 0; i < vertices.Length; i++)
        {
            if (vertices[i].Equals(vertex)) continue;
            RemoveEdge(vertices[i], vertex);
        }

        return adjacencyList.Remove(vertex);
    }

    Edge<T> GetEdge(T from, T to)
    {
        if (!adjacencyList.ContainsKey(from)) return null;

        //si llegamos hasta aca, estaba en el nodo inicial
        SimpleArrayList<Edge<T>> neighbourEdges = adjacencyList[from]; //el diccionario posee la lista de vecinos de una Key (un nodo especifico)

        //Recorremos la lista de Edges
        for(int i = 0; i < neighbourEdges.Count; i++)
        {
            //si el edge tiene el mismo detino, es el que buscamos
            if(neighbourEdges[i].to.Equals(to)) return neighbourEdges[i];
        }

        return null;
    }
}