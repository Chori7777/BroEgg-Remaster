using System;
using System.Collections.Generic;
using UnityEngine;

public class SimpleArrayDictionary<TKey, TValue> : ISimpleDictionary<TKey, TValue>
{
    KeyValuePair<TKey, TValue>[] internalArray;
    int defaultCapacity = 4;
    int count = 0;

    public TValue this[TKey key]
    {
        get
        {
            if (key == null)
            {
                throw new ArgumentNullException("Key is null");
            }

            int index = indexOf(key);

            //si la key no esta, get no puede devolver nada valido: tira KeyNotFoundException
            if (index < 0)
            {
                throw new KeyNotFoundException("Key is not in this Dictionary");
            }

            return internalArray[index].Value;
        }
        set
        {
            if (key == null)
            {
                throw new ArgumentNullException("Key is null");
            }

            int index = indexOf(key);

            if (index >= 0)
            {
                //la key existe: KeyValuePair es un struct y no se puede modificar, creamos un par nuevo y lo pisamos
                internalArray[index] = new KeyValuePair<TKey, TValue>(key, value);
            }
            else
            {
                //la key no existia: la agregamos al final como en Add y TryAdd
                ExecuteAdd(key, value);
            }
        }
    }

    public int Count => count;
    public bool IsEmpty => count == 0;


    public SimpleArrayDictionary()
    {
        internalArray = new KeyValuePair<TKey, TValue>[defaultCapacity];
    }

    public void Add(TKey key, TValue value)
    {
        if (ContainsKey(key))
        {
            throw new ArgumentException("Key is already use in this Dictionary");
        }

        ExecuteAdd(key, value);
    }

    public bool Remove(TKey key)
    {
        if (key == null)
        {
            throw new ArgumentNullException("Key is null");
        }

        int index = indexOf(key);

        //si la key no existe no hay nada que remover
        if (index < 0) return false;

        //como el orden no se garantiza no corremos todo: el ultimo elemento ocupa el lugar vacio
        internalArray[index] = internalArray[count - 1];

        //limpiamos la ultima posicion (queda el par default) y bajamos count
        internalArray[count - 1] = default(KeyValuePair<TKey, TValue>);
        count--;

        return true;
    }

    public void Clear()
    {
        //el array nuevo vuelve a la capacidad inicial (con new ...[count] podia quedar de largo 0 y Resize nunca terminaba)
        internalArray = new KeyValuePair<TKey, TValue>[defaultCapacity];
        count = 0;
    }

    public bool ContainsKey(TKey key)
    {
        if (key == null)
        {
            throw new ArgumentNullException("Key is null");
        }

        return indexOf(key) >= 0;
    }

    public bool TryAdd(TKey key, TValue value)
    {
        //ContainsKey ya valida que la key no sea null
        if (ContainsKey(key))
        {
            return false;
        }

        ExecuteAdd(key, value);
        return true;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        if (key == null)
        {
            throw new ArgumentNullException("Key is null");
        }

        int index = indexOf(key);

        if (index >= 0)
        {
            value = internalArray[index].Value; //guardamos el valor en el parametro out
            return true;
        }

        //out obliga a asignar value en todos los caminos, si no esta le damos su valor default
        value = default(TValue);
        return false;
    }

    public TKey[] Keys()
    {
        TKey[] result = new TKey[count]; //creo un array con la cantidad de espacion ocupados (count) en la lista

        for (int i = 0; i < count; i++)
        {
            result[i] = internalArray[i].Key; //copiamos 1x1
        }
        return result; //devolvemos el array
    }

    public TValue[] Values()
    {
        TValue[] result = new TValue[count]; //creo un array con la cantidad de espacion ocupados (count) en la lista

        for (int i = 0; i < count; i++)
        {
            result[i] = internalArray[i].Value; //copiamos 1x1
        }
        return result; //devolvemos el array
    }

    //-------------------------------------------------------------------

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
        KeyValuePair<TKey, TValue>[] nextArray = new KeyValuePair<TKey, TValue>[currentLength];

        //copiamos lo que hay en el array viejo en el nuevo
        for (int i = 0; i < count; i++)
        {
            nextArray[i] = internalArray[i];
        }

        //reemplazamos el array actual por el nuevo mas grande
        internalArray = nextArray;
    }

    int indexOf(TKey key) //basicamente esto recorre el set y va sumando al i, si encuentra la condicion le asiganamos al objeto un idice
    {
        for (int i = 0; i < count; i++)
        {
            if (internalArray[i].Key.Equals(key)) return i;
        }
        //esto lo toma como null
        return -1;
    }

    void ExecuteAdd(TKey key, TValue value)
    {
        ValidateSize(count);
        internalArray[count] = new KeyValuePair<TKey, TValue>(key, value);
        count++;
    }
}