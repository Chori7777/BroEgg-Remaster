using System;


namespace ED262C
{
    public class SimpleLinkedList<T> : ISimpleList<T>
    {
        // Referencias al primer y ultimo nodo
        LinkedNode<T> first;
        LinkedNode<T> last;
        
        // Cantidad de elementos guardados en la lista
        int count;

        // myList[3] y va al "indice" 3
        public T this[int index] 
        {
            get => GetNodeByIndex(index).value;
            
            // Cuando hacemos myList[3] = 10, value es 10
            // Guardamos 10 en el nodo (tambien tiene una variable value)
            set => GetNodeByIndex(index).value = value; 
        }

        public int Count => count;

        public void Add(T item)
        {
            // Creamos un nodo que guarde el dato a insertar
            LinkedNode<T> newNode = new LinkedNode<T>(item);

            // Si la lista estaba vacia, el nuevo nodo es el primero Y ultimo
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

        public void AddRange(T[] items)
        {
            throw new System.NotImplementedException();
        }

        public void Clear()
        {
            // Si borramos el primero y ultimo, todo lo demas queda aislado
            // No hay ningun objeto referenciando a esos nodos
            // En ese caso, el Garbage Collector borra todo automaticamente
            first = null;
            last = null;
            count = 0;
        }

        public bool Contains(T item)
        {
            return GetNodeByValue(item) != null;
        }

        public void Insert(int index, T item)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(T item)
        {
            LinkedNode<T> toRemove = GetNodeByValue(item);
            if (toRemove == null) return false;

            // Si solo hay un elemento, vaciamos todo
            // No necesitamos chequear que sea el elemento buscado porque
            // si no es ya devolvimos false antes
            if (count == 1)
            {
                Clear(); // Clear ya borra first, last y deja count en 0
                return true;
            }

            // Si quiero borrar el primero pero no es el unico
            if (toRemove == first)
            {
                // El segundo pasa a ser el nuevo primero
                // Y no tiene mas prev
                first.next.prev = null;
                first = first.next;
            }

            // Si quiero borrar el ultimo y no es el unico
            else if (toRemove == last)
            {
                // El anteultimo pasa a ser el nuevo ultimo
                // Y no tiene mas next
                last.prev.next = null;
                last = last.prev;
            }

            // Si esta en el medio, reconectamos
            else RemoveAndReconnect(toRemove);

            count--;
            return true;
        }

        public void RemoveAt(int index)
        {
            // Validamos que el indice exista
            if (index < 0 || index >= count)
                throw new IndexOutOfRangeException("Index is outside of List bounds");

            // Si solo hay un elemento, vaciamos todo
            // No necesitamos chequear que el indice sea 0, porque ya lo hicimos arriba
            if (count == 1)
            {
                Clear(); // Clear ya borra first, last y deja count en 0
                return;
            }

            // Si quiero borrar el primero pero no es el unico
            if (index == 0)
            {
                // El segundo pasa a ser el nuevo primero
                // Y no tiene mas prev
                first.next.prev = null;
                first = first.next;
            }

            // Si quiero borrar el ultimo y no es el unico
            else if(index == count -1)
            {
                // El anteultimo pasa a ser el nuevo ultimo
                // Y no tiene mas next
                last.prev.next = null;
                last = last.prev;
            }

            // Buscamos el nodo con GetNodeByIndex
            // Reconectamos a sus vecinos entre si con RemoveAndReconnect
            else RemoveAndReconnect(GetNodeByIndex(index));

            count--;
        }

        public void RemoveRange(int index, int count)
        {
            throw new System.NotImplementedException();
        }

        public T[] ToArray()
        {
            throw new System.NotImplementedException();
        }

        // Usamos esta funcion para el indexer, Remove, Insert, etc.
        LinkedNode<T> GetNodeByIndex(int index)
        {
            if (index < 0 || index >= count)
                throw new IndexOutOfRangeException("Index is outside of List bounds");

            //Decidimos si empezar por el primer nodo o el ultimo
            // Para saber si esta mas cerca del principio o final,
            // Hacemos count / 2
            if(index <= count / 2)
            {
                // Empezamos por el primero
                LinkedNode<T> current = first;

                // Vamos pasando por el siguiente del siguiente, etc.
                // Hasta llegar al indice
                for(int i = 0; i < index; i++)
                    current = current.next;

                // Una vez que repetimos el proceso, devolvemos el que
                // estaba en la posicion index
                return current;
            }
            else
            {
                // Empezamos por el utimo
                LinkedNode<T> current = last;

                // Vamos pasando por el anterior del anterior, etc.
                // Hasta llegar al indice
                for (int i = count -1; i > index; i--)
                    current = current.prev;

                // Una vez que repetimos el proceso, devolvemos el que
                // estaba en la posicion index
                return current;
            }
        }

        LinkedNode<T> GetNodeByValue(T value)
        {
            // Como no sabemos donde esta el valor, siempre arrancamos en first
            LinkedNode<T> current = first;
            
            // Solo podemos chequear un nodo si existe
            while(current != null)
            {
                // Si el nodo tiene lo que buscamos, lo devolvemos
                if (current.value.Equals(value)) return current;
                
                // Si llegamos hasta aca, no era el valor
                // Pasamos al siguiente
                current = current.next;
            }

            // Si llegamos hasta aca, vimos toda la lista y no estaba
            return null;
        }

        // Vamos a llamar a esta funcion en Remove, RemoveAt
        void RemoveAndReconnect(LinkedNode<T> toRemove)
        {
            // Antes de remover
            // toRemove.prev <---> toRemove <---> toRemove.next

            // Despues de remover
            // toRemove.prev <------------------> toRemove.next

            // Asignamos las referencias de los nodos vecinos de toRemove
            toRemove.next.prev = toRemove.prev;
            toRemove.prev.next = toRemove.next;

            // Ahora que estan conectados, borramos a toRemove
            toRemove = null;
        }
    }
}

