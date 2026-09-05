namespace ED262C
{
    // Reciclamos ISimpleStack
    // Cambiamos Push -> Enqueue, Pop -> Dequeue
    public interface ISimpleQueue<T>
    {
        public int Count { get; }
        public bool IsEmpty { get; }
        public void Enqueue(T item);
        public T Dequeue();
        public T Peek();
        public void Clear();
        public T[] ToArray();
    }
}