using UnityEngine;

public interface ISimpleSet<T>
{
    public int Count { get; }
    public bool IsEmpty {  get; }

    public bool Add(T item);
    public bool Remove(T item);
    public bool Contains(T item);
    public void Clear();
    public T[] ToArray();
    public ISimpleSet<T> UnionWith(ISimpleSet<T> other);
    public ISimpleSet<T> IntersectWith(ISimpleSet<T> other);
    public ISimpleSet<T> DifferenceWith(ISimpleSet<T> other);

}
