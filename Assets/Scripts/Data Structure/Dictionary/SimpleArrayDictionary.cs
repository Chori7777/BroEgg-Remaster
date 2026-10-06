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
            if (key == null) throw new ArgumentNullException(nameof(key));

            int index = indexOf(key);
            if (index < 0) throw new KeyNotFoundException($"La clave '{key}' no existe en el diccionario.");

            return internalArray[index].Value;
        }
        set
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            int index = indexOf(key);
            if (index >= 0)
            {
                // Si la clave existe, actualizamos su valor
                internalArray[index] = new KeyValuePair<TKey, TValue>(key, value);
            }
            else
            {
                // Si no existe, lo agregamos como nuevo
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
        if (key == null) throw new ArgumentNullException(nameof(key));

        int index = indexOf(key);

        // Si no existe la clave, no hay nada que eliminar papu
        if (index < 0) return false;

        //si es el ultio es una boludez
        if (index == count - 1) internalArray[index] = default;

        //si no es el ultimo
        if (index != count - 1) internalArray[index] = internalArray[count - 1];


        //Limpiamos la última referencia para evitar fugas de memoria (Garbage Collector)
        internalArray[count - 1] = default;
        count--;

        return true;
    }

    public void Clear()
    {
        internalArray = new KeyValuePair<TKey, TValue>[count];
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
        if (key == null) return false;

        // Si ya contiene la clave, devolvemos false sin tirar excepción
        if (ContainsKey(key)) return false;

        ExecuteAdd(key, value);
        return true;
    }

    // 3. IMPLEMENTACIÓN DE TRYGETVALUE
    public bool TryGetValue(TKey key, out TValue value)
    {
        if (key == null)
        {
            value = default;
            return false;
        }

        int index = indexOf(key);

        if (index >= 0)
        {
            value = internalArray[index].Value;
            return true;
        }

        // Si no se encuentra la clave, devolvemos el valor por defecto de TValue
        value = default;
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
        internalArray[count] = new KeyValuePair<TKey, TValue>(key, value); //agrego al diccionario un nuevo par de valores en la ultima pos del array
        count++;
    }
}
