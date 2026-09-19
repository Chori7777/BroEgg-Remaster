using UnityEngine;

public class SimpleArrayPriorityQueue<T> : ISimplePriorityQueue<T>
{
    int count = 0;
    int defaultCapacity = 4;
    T[] internalArray;
    int[] priorities;

    public int Count => count;

    public bool IsEmpty => count == 0;

    //inicializamos los dos arrays
    public SimpleArrayPriorityQueue()
    {
        internalArray = new T[defaultCapacity];
        priorities = new int[defaultCapacity];
    }

    public void Clear()
    {
        internalArray = new T[internalArray.Length];
        priorities = new int[priorities.Length];
        count = 0;
    }

    public T Dequeue()
    {
        if (IsEmpty) throw new System.Exception("Cannot Dequeue from this Queue");
        T result = internalArray[0];
        ShiftLeft(1); //aca falta un offset que seria un segundo parametro
        count--;
        return result;
    }

    public void Enqueue(T item, int priority)
    {
        ValidateSize(count + 1);

        int insertIndex = count;

            
      for(int i = count; i > 0 && priority < priorities[i-1]; i--)
      {
          //corremos lo que esta en el indice anterior al indice actua{
          //movemos para la derecha, dejando un hueco en el indice anterior
          internalArray[i] = internalArray[i - 1];
          priorities[i] = priorities[i - 1];
          insertIndex = i;
      }

        internalArray[insertIndex] = item;
        priorities[insertIndex] = priority;
        count++;
    }

    public T Peek()
    {
        if (IsEmpty) throw new System.Exception("Cannot Peek from this Queue");
        return internalArray[0];
    }

    public int GetHighestPriority()
    {
        if (IsEmpty) throw new System.Exception("Empty Priority Queue");
        return priorities[0];
    }

    public T[] ToArray()
    {
        T[] result = new T[count]; //creo un array con la cantidad de espacion ocupados (count) en la lista

        for (int i = 0; i < count; i++)
        {
            result[i] = internalArray[i]; //copiamos 1x1
        }
        return result; //devolvemos el array
    }

    void ValidateSize(int nextIndex)
    {
        if (nextIndex >= internalArray.Length)
        {
            Resize(nextIndex + 1);
        }
    }

    //-------------------------------------------
    void Resize(int targetSize)
    {
        int currentLength = internalArray.Length;

        while (targetSize > currentLength)
        {
            currentLength *= 2;
        }


        //creamos un array del doble del largo del anterior
        T[] nextArray = new T[currentLength];
        int[] nextPriorities = new int[currentLength];

        //copiamos lo que hay en el array viejo en el nuevo
        for (int i = 0; i < count; i++)
        {
            nextArray[i] = internalArray[i];
            nextPriorities[i] = priorities[i];
        }

        //reemplazamos el array actual por el nuevo mas grande
        internalArray = nextArray;
        priorities = nextPriorities;
    }


    //Corremos todo lo que viene despues de index, uno para adelante
    void ShiftLeft(int index)
    {
        for (int i = index; i < count; i++)
        {
            //lo que esta en el casillero actual se pisa con el siguiente
            internalArray[i] = internalArray[i + 1];
            priorities[i] = priorities[i + 1];
        }
        //vaciamos el ultimo espacio para evitar que guarde algo que no sirve o crashee
        internalArray[count] = default;
        priorities[count] = default;
    }
}
