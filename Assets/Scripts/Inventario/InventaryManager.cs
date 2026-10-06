using System;
using System.Collections.Generic;
using UnityEngine;

public class InventaryManager : MonoBehaviour
{
    private readonly Dictionary<string, ObjectShopData> ownedObjects = new Dictionary<string, ObjectShopData>();

    public int Count => ownedObjects.Count;

    public event Action OnInventoryChanged;

    public bool Contains(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return false;
        }

        return ownedObjects.ContainsKey(id);
    }

    public bool TryGet(string id, out ObjectShopData item)
    {
        item = null;

        if (string.IsNullOrWhiteSpace(id))
        {
            return false;
        }

        return ownedObjects.TryGetValue(id, out item);
    }

    public bool TryAdd(ObjectShopData item)
    {
        if (item == null)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(item.id))
        {
            return false;
        }

        // La clave es el ID: no puede haber dos objetos con el mismo ID.
        if (!ownedObjects.TryAdd(item.id, item))
        {
            return false;
        }

        OnInventoryChanged?.Invoke();
        return true;
    }

    public bool TryRemove(string id)
    {
        if (!Contains(id))
        {
            return false;
        }

        if (!ownedObjects.Remove(id))
        {
            return false;
        }

        OnInventoryChanged?.Invoke();
        return true;
    }

    public ObjectShopData[] GetObjects()
    {
        ObjectShopData[] items = new ObjectShopData[ownedObjects.Count];
        ownedObjects.Values.CopyTo(items, 0);

        // Devuelve una copia para que otros scripts no modifiquen el diccionario.
        return items;
    }
}
