using System;
namespace ED262C
{
    // No copien todas las funciones a mano: la interfaz se subraya y pueden autocompletar
    public class SimpleArrayStack<T> : ISimpleStack<T>
    {
        int defaultCapacity = 4;
        int count = 0;
        T[] internalArray;

        public int Count => count;
        public bool IsEmpty => count == 0;

        public SimpleArrayStack() => internalArray = new T[defaultCapacity];

        public void Clear()
        {
            internalArray = new T[internalArray.Length];
            count = 0;
        }

        // La expresion Lambda (=>) seria como "return"
        // Podemos usarla para funciones de una sola linea
        public T Peek()
        {
            if (IsEmpty) throw new InvalidOperationException("Cannot Peek from empty Stack");
            return internalArray[count - 1];
        }
        

        public T Pop()
        {
            if (IsEmpty) throw new InvalidOperationException("Cannot Pop from empty Stack");
            // Guardamos el elemento en una variable temporal
            // Hacemos esto para no perderlo
            T result = internalArray[count - 1];

            // Lo sacamos, y luego devolvemos el dato con la variable temporal
            internalArray[count - 1] = default;
            count--;
            return result;
        }

        // Reciclamos Add de List renombrado a Push
        // Nota: tambien reclicamos ValidateSize y Resize
        public void Push(T item)
        {
            // Primero chequeamos que haya espacio
            ValidateSize(count + 1);

            // La cantidad de elementos ocupados es igual al primer indice  libre 
            internalArray[count] = item;
            count++;
        }

        // Reciclado de SimpleArrayList
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
    }
}
