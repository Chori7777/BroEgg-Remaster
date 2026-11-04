using ED262C;
using UnityEngine;

public interface IGraph<T>
{
    public T[] vertices { get; }
    public bool AddVertex(T vertex); //esto es el nodo
    public bool RemoveVertex(T vertex);
    public bool AddEdge(T from, T to, int weight); //esto es lo que conecta a los nodos
    public bool RemoveEdge(T from, T to);
    public ISimpleList<Edge<T>> GetNeighbours(T vertex);
    public bool ContainsVertex(T vertex);
    public bool ContainsEdge(T from, T to);
    public int GetWeight(T from, T to);
}
