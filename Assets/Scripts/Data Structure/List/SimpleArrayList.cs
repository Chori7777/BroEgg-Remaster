using System;

namespace ED262C
{
    public class SimpleArrayList<T> : ISimpleList<T>
    {
        // Internamente, la lista contiene un array para guardar los datos
        T[] internalArray;

        // La lista lleva la cuenta de cuantos elementos hay
        int count = 0;

        // Si no especificamos, arranca el array con 4 elementos
        int defaultCapacity = 4;

        // Propiedad publica que muestra cuantos elementos hay (solo lectura)
        public int Count { get => count; }

        // Indexer: permite usar la lista como si fuera un array
        // Ej: myList[5]
        public T this [int index] 
        { 
            // Esto se llama cuando hacemos Debug.Log(myList[3])
            // Devuelve lo que este guardado en index
            get => internalArray[index];

            // Esto se llama cuando hacemos myList[3] = 10;
            // El dato al que igualemos se guarda en index
            set => internalArray[index] = value; 
        }

        // Constructor: se llama cuando hacemos "new" 
        // Ej: SimpleArrayList myList = new SimpleArrayList()
        // Configura el array con los valores por defecto
        public SimpleArrayList()
        {
            internalArray = new T[defaultCapacity];
        }

        // Add agrega el elemento item al final
        public void Add(T item)
        {
            // Primero chequeamos que haya espacio
            ValidateSize(count + 1);

            // La cantidad de elementos ocupados es igual al primer indice  libre 
            internalArray[count] = item;
            count++;
        }

        // Remove busca a item en la Lista, y elimina la primera instancia de item
        // Ej: List {2, 3, 3, 8} y Remove(3)
        //          {2, X, 3, 8} -> {2, 3, 8}
        public bool Remove(T item)
        {
            // Recorremos todo el array hasta count
            for(int i = 0; i < count; i++)
            {
                // Si el elemento actual es igual a item, removemos
                if(internalArray[i].Equals(item))
                {
                    ShiftLeft(i, 1);
                    count--;
                    return true;
                }
            }

            // Si llegamos hasta aca es porque no estaba
            // Si no, habria retornado antes y cortaba la funcion
            return false;
        }

        // Inserta el elemento item, en la posicion index
        public void Insert(int index, T item)
        {
            // Garantizamos que haya lugar en el array
            ValidateSize(count + 1);

            // Corremos todo lo que venga despues del indice para dejar lugar
            ShiftRight(index);

            // Guardamos el item en ese espacio y subimos la cuenta
            internalArray[index] = item;
            count++;
        }
        public void RemoveAt(int index)
        {
            // Si el indice no existe, rompemos el programa
            if(index < 0 || index >= internalArray.Length) 
                throw new ArgumentOutOfRangeException("Index is outside of bounds");
            
            ShiftLeft(index, 1);
            count--;
        }

        public void AddRange(T[] items)
        {
            // Garantizamos que haya espacio
            // Al ultimo indice 
            ValidateSize(count + items.Length);

            // El bucle recorre todos los elementos del array items
            for(int i = 0; i < items.Length; i++)
            {
                // Guardamos el elemento de items actual en el espacio vacio
                internalArray[count] = items[i];

                // Corremos el espacio vacio para no guardar siempre en el mismo lugar
                count++;
            }
        }

        // Empezando desde index, removemos count elementos
        public void RemoveRange(int index, int count)
        {
            // Si el indice no existe, rompemos el programa
            if(index < 0 || index >= internalArray.Length) 
                throw new ArgumentOutOfRangeException("Index is outside of bounds");
            
            // Corremos todos los elementos que vayan despues de index para la izq
            // Lo movemos una cantidad de espacios = la que queremos remover
            ShiftLeft(index, count);

            // A la cantidad de elementos de esta lista, le restamos la cantidad a remover
            this.count -= count;
        }

        public bool Contains(T item)
        {
            // Recorremos todo el array hasta count
            for(int i = 0; i < count; i++)
            {
                // Si el elemento actual es igual a item, true
                if(internalArray[i].Equals(item)) return true;
            }

            // Si llego hasta aca, no estaba
            return false;
        }

        // Reseteamos el array y el count a 0
        public void Clear()
        {
            count = 0;
            internalArray = new T[defaultCapacity];
        }

        void ValidateSize(int nextIndex)
        {
            if(nextIndex >= internalArray.Length) Resize(nextIndex);
        }

        // Le pasamos cuantos elementos va a tener despues de agregar
        void Resize(int targetAmount)
        {
            // Guaramos el largo del array al principio
            int currentLength = internalArray.Length;

            // Vamos a duplicar ese tamaño mientras sea mas chico que la cantidad que queremos
            while(targetAmount > currentLength)
                currentLength *= 2;

            // Creamos un array del doble de largo que el actual
            T[] nextArray = new T[currentLength];

            // Copiamos todo lo que hay en el array actual al nuevo
            for(int i = 0; i < count; i++)
                nextArray[i] = internalArray[i];

            // Reemplazamos el array actual por el nuevo
            internalArray = nextArray;
        }

        // Corremos todo lo que viene despues de index para la izquierda
        // Asi no quedan espacios en el medio
        void ShiftLeft(int index, int offset)
        {
            // Lo que esta en el casillero actual se pisa con el siguiente
            for(int i = index; i < count; i++)
                internalArray[i] = internalArray[i + offset];

            // Limpiamos todo lo que vaya despues del ultimo elemento
            // para no guardar datos que ya no necesitamos
            for(int i = count - offset; i < count; i++)
                internalArray[i] = default;
        }

        // Corremos todo lo que viene despues de index, uno para adelante
        void ShiftRight(int index)
        {
            // Vamos al reves que shiftLeft
            // Desde despues del ultimo hasta el index
            // i-- porque va de atras para adelante
            for(int i = count + 1; i > index; i--)
            {
                // Lo que esta en el casillero actual se pisa con el anterior
                internalArray[i] = internalArray[i-1];
            }

            // Vaciamos el espacio donde vamos a insertar (no es necesario)
            internalArray[index] = default;
        }

        public T[] ToArray()
        {
            // Creo un array con la cantidad de elementos que estan ocupados en la lista
            T[] result = new T[count];

            // Copiamos uno por uno todos los elementos al nuevo array
            for(int i = 0; i < count; i++)
                result[i] = internalArray[i];

            // Devolvemos el array completo
            return result;
        }
    }
}