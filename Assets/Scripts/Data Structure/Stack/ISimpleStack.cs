namespace ED262C
{
    public interface ISimpleStack<T>
    {
        public int Count { get; }
        public bool IsEmpty { get; }
        public void Push(T item);
        public T Pop();
        public T Peek();
        public void Clear();
        public T[] ToArray();
    }
}

