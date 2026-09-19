using UnityEngine;

public interface ISimpleDictionary<TKey, TValue>
{
    public TValue this[TKey key] { get; set; }
    public int Count { get; }
    public bool IsEmpty { get; }
    public void Add(TKey key, TValue value);
    public bool Remove(TKey key);
    public bool ContainsKey(TKey key);
    public bool TryAdd(TKey key, TValue value);
    public bool TryGetValue(TKey key, out TValue value);
    public void Clear();
    public TKey[] Keys();
    public TValue[] Values();

}
