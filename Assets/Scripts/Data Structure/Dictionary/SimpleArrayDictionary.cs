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
        get => throw new System.NotImplementedException(); 
        set => throw new System.NotImplementedException(); 
    }

    public int Count => count;
    public bool IsEmpty => count == 0;
    

    public SimpleArrayDictionary()
    {
        internalArray = new KeyValuePair<TKey, TValue>[defaultCapacity];
    }

    public void Add(TKey key, TValue value)
    {
        if(ContainsKey(key))
        {
            throw new ArgumentException("Key is already use in this Dictionary");
        }

        ExecuteAdd(key, value);
    }

    public bool Remove(TKey key)
    {
        throw new System.NotImplementedException();
    }

    public void Clear()
    {
        internalArray = new KeyValuePair<TKey, TValue>[count];
        count = 0;
    }

    public bool ContainsKey(TKey key)
    {
        if(key == null)
        {
            throw new ArgumentNullException("Key is null");
        }

       return indexOf(key) >= 0;
    }

    public bool TryAdd(TKey key, TValue value)
    {
        throw new System.NotImplementedException();
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        throw new System.NotImplementedException();
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
