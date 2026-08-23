namespace ED262C
{
    public interface ISimpleList<T>
    {
        public int Count {get;}
        public T this [int index] { get; set; }
        public void Add(T item);
        public bool Remove(T item);
        public void Insert(int index, T item);
        public void RemoveAt(int index);
        public void AddRange(T[] items);
        public void RemoveRange(int index, int count);
        public bool Contains(T item);
        public void Clear();
        public T[] ToArray();
    }
}
