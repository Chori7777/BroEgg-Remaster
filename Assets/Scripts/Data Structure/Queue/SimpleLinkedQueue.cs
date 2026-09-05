using System;

namespace ED262C
{
    public class SimpleLinkedQueue<T> : ISimpleQueue<T>
    {
        // Referencias al primer y ultimo nodo
        LinkedNode<T> first = null;
        LinkedNode<T> last = null;

        // Cantidad de elementos guardados en la lista
        int count = 0;

        public int Count => count;

        public bool IsEmpty => count == 0;

        // Igual que en List y Stack
        public void Clear()
        {
            // Si borramos el primero y ultimo, todo lo demas queda aislado
            // No hay ningun objeto referenciando a esos nodos
            // En ese caso, el Garbage Collector borra todo automaticamente
            first = null;
            last = null;
            count = 0;
        }

        public T Dequeue()
        {
            if (IsEmpty) throw new InvalidOperationException("Cannot Dequeue from empty Queue");
            
            T result = first.value;
            first = first.next;

            // Solo si queda al menos un nodo, le sacamos el prev al primero
            if (first != null) first.prev = null;
            count--;
            return result;
        }

        // Igual que Add de List
        public void Enqueue(T item)
        {
            // Creamos un nodo que guarde el dato a insertar
            LinkedNode<T> newNode = new LinkedNode<T>(item);

            // Si la cola estaba vacia, el nuevo nodo es el primero Y ultimo
            if (count == 0) first = newNode;

            // Si no, conectamos al nuevo nodo con el que era ultimo
            else
            {
                newNode.prev = last;
                last.next = newNode;
            }

            // El ultimo ahora es el nuevo y subimos count
            last = newNode;
            count++;
        }

        // Igual que Stack, pero con el primero
        public T Peek()
        {
            if (IsEmpty) throw new InvalidOperationException("Cannot Peek from empty Queue");
            return first.value;
        }

        public T[] ToArray()
        {
            // Este es el array que devolvemos
            T[] result = new T[count];

            // Empezamos desde el primer nodo, pasamos uno por uno
            LinkedNode<T> current = first;
            for (int i = 0; i < count; i++)
            {
                // Guardamos lo que esta en este nodo, en el indice actual
                result[i] = current.value;

                // Pasamos al siguiente
                current = current.next;
            }

            return result;
        }
    }
}