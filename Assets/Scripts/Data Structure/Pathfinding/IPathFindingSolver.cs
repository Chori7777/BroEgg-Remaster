using ED262C;
using UnityEngine;

public interface IPathFindingSolver<T>
{
    public ISimpleDictionary<GraphNode<T>, Edge<GraphNode<T>>> GetAllShortestPath(GraphNode<T> origin, IGraph<GraphNode<T>> graph);
    public ISimpleList<GraphNode<T>> GetShortestPath(GraphNode<T> origin, GraphNode<T> destination, IGraph<GraphNode<T>> graph);


}
