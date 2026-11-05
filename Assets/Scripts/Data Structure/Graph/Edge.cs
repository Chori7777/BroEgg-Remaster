using UnityEngine;

public class Edge<T>
{
    public T to;
    public int weight;

    public Edge(T destination, int weight) // to es para donde va y weight el peso que tiene esa arista
    {
        this.to = destination;
        this.weight = weight;
    }

    public override bool Equals(object obj) //esta funcion existe en la clase Object de unity
    {
        if(obj.GetType() != GetType()) return false;

        Edge<T> other = (Edge<T>)obj; //casteo un Object a Edge

        if(other.to == null || !other.to.Equals(to)) return false;
        if (other.weight != weight) return false;

        return true;
    }
}
