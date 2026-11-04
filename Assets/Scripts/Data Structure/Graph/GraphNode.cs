using UnityEngine;

public class GraphNode<T>
{
    public T value; // para pathfinding esta variable puede ser Vector2
                    //aca puede haber una variable booleana para saber si un nodo esta bloqueado 
    public GraphNode(T value) => this.value = value;
}
