using Unity.Burst;
using UnityEngine;

public class SimpleArraySet<T> : ISimpleSet<T>
{
    T[] internalArray; //array generico
    int count = 0; //cuenta interna de elementos
    int defaultCapacity = 4;
    public int Count => count;

    public bool IsEmpty => count == 0;

    public SimpleArraySet()
    {
        internalArray = new T[defaultCapacity];
    }
    public SimpleArraySet(ISimpleSet<T> original)
    {
        //convertimos un set a un array
        T[] originalArray = original.ToArray();

        //inicializamos el array interno con el mismo largo del original
        internalArray = new T[originalArray.Length];

        //Copiamos los elementos
        for(int i = 0; i < originalArray.Length; i++)
        {
            internalArray[i] = originalArray[i];
        }

        //hacemos que el count sea el del original
        count = original.Count;
    }

    public bool Add(T item)
    {
        if(Contains(item)) return false;
        
        //lo agrega al final y devuelve true, si es que no lo contiene inicialmente
        ValidateSize(count);
        internalArray[count] = item;
        count++;
        return true;
    }
    public bool Remove(T item)
    {
        //Si no lo contiene, no lo puede remover, devuelve false
        //Si lo contiene, va al indice y lo remueve, devuelve true
        int itemIndex = indexOf(item);
        if(itemIndex < 0) return false;

        //a diferencia de List, Queue o Stack, no se respeta el orden
        //mover con ShiftLeft() tiene una complejidad de O(n) y mantiene un orden
        //mover un solo elemento tiene complejidad O(1)

        //pisamos el indice a remover con el ultimo elemento
        //Salvo que el elemento a remover sea el ultimo
        if (itemIndex != count - 1)
        {
            internalArray[itemIndex] = internalArray[count - 1];    
        }
        //defaulteamos el ultimo indice en cualquier caso
        //si era el ultimo lo borramos
        //si ni era el ultimo, lo borramos para que no quede duplicado
        internalArray[count - 1] = default;
        return true;
    }

    public void Clear()
    {
        internalArray = internalArray = new T[internalArray.Length];
        count = 0;
    }

    public bool Contains(T item)
    {
        return indexOf(item) >= 0;
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

    //Devuelve un Set con todos los elementos de ambos
    public ISimpleSet<T> UnionWith(ISimpleSet<T> other)
    {
        //una copia del Set que recibimos
        ISimpleSet<T> result = new SimpleArraySet<T>(other);

        //al nuevo Set le agregamos los elementos de mi Set
        for(int i = 0; i < count;i++)
        {
            result.Add(internalArray[i]);
        }

        return result;
    }

    //Devuelve un Set con los elementos en comun dentre ambos
    public ISimpleSet<T> IntersectWith(ISimpleSet<T> other)
    {
        //Arrancamos con un Set vacio
        ISimpleSet<T> result = new SimpleArraySet<T>();

        //Recorremos nuestro array (garantiza que esten los elementos de este Set)
        for(int i = 0;i < count;i++)
        {
            //Solo lo agregamos si el otro Set tambien lo tiene
            if(other.Contains(internalArray[i]))
            {
                result.Add(internalArray[i]);
            }
        }

        return result;
    }

    //Devuelve un Set con todos los elementos de este Set que no esten en el otro
    public ISimpleSet<T> DifferenceWith(ISimpleSet<T> other)
    {
        //Arrancamos con un Set vacio
        ISimpleSet<T> result = new SimpleArraySet<T>();

        //Recorremos nuestro array (garantiza que esten los elementos de este Set)
        for (int i = 0; i < count; i++)
        {
            //Solo lo agregamos si el otro Set NO LO TIENE
            if (!other.Contains(internalArray[i]))
            {
                result.Add(internalArray[i]);
            }
        }

        return result;
    }


    void ValidateSize(int nextIndex)
    {
        if (nextIndex >= internalArray.Length)
        {
            Resize(nextIndex + 1);
        }
    }

    void Resize(int targetSize)
    {
        int currentLength = internalArray.Length;

        while (targetSize > currentLength)
        {
            currentLength *= 2;
        }


        //creamos un array del doble del largo del anterior
        T[] nextArray = new T[currentLength];

        //copiamos lo que hay en el array viejo en el nuevo
        for (int i = 0; i < count; i++)
        {
            nextArray[i] = internalArray[i];
        }

        //reemplazamos el array actual por el nuevo mas grande
        internalArray = nextArray;
    }

    int indexOf(T item) //basicamente esto recorre el set y va sumando al i, si encuentra la condicion le asiganamos al objeto un idice
    {
        for(int i = 0; i < count; i++)
        {
            if (internalArray[i].Equals(item)) return i;
        }
        //esto lo toma como null
        return -1;
    }
}
