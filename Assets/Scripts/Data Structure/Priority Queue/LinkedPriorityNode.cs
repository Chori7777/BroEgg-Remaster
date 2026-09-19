using UnityEngine;

public class LinkedPriorityNode<T>
{
    public LinkedPriorityNode<T> prev;
    public LinkedPriorityNode<T> next;
    public T value;
    public int priority;
}
