using System;

namespace ED262C
{
    public class SimpleLinkedStack<T> : ISimpleStack<T>
    {
        LinkedNode<T> last = null;
        int count = 0;

        public int Count => count;
        public bool IsEmpty => count == 0;

        public void Clear()
        {
            last = null;
            count = 0;
        }

        public T Peek()
        {
            if (IsEmpty) throw new InvalidOperationException("Cannot Peek from empty Stack");
            return last.value;
        }

        public T Pop()
        {
            if (IsEmpty) throw new InvalidOperationException("Cannot Pop from empty Stack");
            T result = last.value;
            last = last.prev;
            count--;
            return result;
        }

        public void Push(T item)
        {
            // Creamos un nodo que guarde el dato a insertar
            LinkedNode<T> newNode = new LinkedNode<T>(item);

            // No importa si esta vacio o no para Push
            // Si hay un elemento last, pasa a ser el prev del nuevo
            // Si no, no tiene prev el nuevo
            newNode.prev = last;
            last = newNode;
            count++;
        }

        // Igual que el de SimpleLinkedList, pero empezando desde atras
        public T[] ToArray()
        {
            // Este es el array que devolvemos
            T[] result = new T[count];

            // Empezamos desde el ultimo nodo, pasamos uno por uno
            LinkedNode<T> current = last;
            for (int i = count -1; i >= 0; i--)
            {
                // Guardamos lo que esta en este nodo, en el indice actual
                result[i] = current.value;

                // Pasamos al anterior
                current = current.prev;
            }

            return result;
        }
    }
}