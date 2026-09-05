using System;

namespace ED262C
{
    public class SimpleArrayQueue<T> : ISimpleQueue<T>
    {
        int defaultCapacity = 4;
        int count = 0;
        T[] internalArray;

        public SimpleArrayQueue() => internalArray = new T[defaultCapacity];

        public int Count => count;

        public bool IsEmpty => count == 0;

        public void Clear()
        {
            internalArray = new T[internalArray.Length];
            count = 0;
        }

        public T Dequeue()
        {
            if (IsEmpty) throw new InvalidOperationException("Cannot Dequeue from empty Queue");
            // Guardamos el elemento en una variable temporal
            // Hacemos esto para no perderlo
            T result = internalArray[0];

            // Lo sacamos moviendo los demas datos para la izquierda
            ShiftLeft(0, 1);
            count--;
            return result;
        }

        // Igual que Add de List y Pop de Stack
        public void Enqueue(T item)
        {
            // Primero chequeamos que haya espacio
            ValidateSize(count + 1);

            // La cantidad de elementos ocupados es igual al primer indice  libre 
            internalArray[count] = item;
            count++;
        }

        // Igual que Peek de Stack pero con el PRIMER elemento
        public T Peek()
        {
            if (IsEmpty) throw new InvalidOperationException("Cannot Peek from empty Stack");
            return internalArray[0];
        }

        // Igual que List y Stack
        public T[] ToArray()
        {
            // Creo un array con la cantidad de elementos que estan ocupados en la lista
            T[] result = new T[count];

            // Copiamos uno por uno todos los elementos al nuevo array
            for (int i = 0; i < count; i++)
                result[i] = internalArray[i];

            // Devolvemos el array completo
            return result;
        }

        void ValidateSize(int nextIndex)
        {
            if (nextIndex >= internalArray.Length) Resize(nextIndex);
        }

        // Le pasamos cuantos elementos va a tener despues de agregar
        void Resize(int targetAmount)
        {
            // Guaramos el largo del array al principio
            int currentLength = internalArray.Length;

            // Vamos a duplicar ese tamaño mientras sea mas chico que la cantidad que queremos
            while (targetAmount > currentLength)
                currentLength *= 2;

            // Creamos un array del doble de largo que el actual
            T[] nextArray = new T[currentLength];

            // Copiamos todo lo que hay en el array actual al nuevo
            for (int i = 0; i < count; i++)
                nextArray[i] = internalArray[i];

            // Reemplazamos el array actual por el nuevo
            internalArray = nextArray;
        }

        // Corremos todo lo que viene despues de index para la izquierda
        // Asi no quedan espacios en el medio
        void ShiftLeft(int index, int offset)
        {
            // Lo que esta en el casillero actual se pisa con el siguiente
            for (int i = index; i < count - offset; i++)
                internalArray[i] = internalArray[i + offset];

            // Limpiamos todo lo que vaya despues del ultimo elemento
            // para no guardar datos que ya no necesitamos
            for (int i = count - offset; i < count; i++)
                internalArray[i] = default;
        }
    }
}
