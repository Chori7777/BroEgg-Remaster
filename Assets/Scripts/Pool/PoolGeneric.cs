using System.Collections.Generic;
using UnityEngine;

public class PoolGeneric<T> : MonoBehaviour where T : Component
{
    [SerializeField] private T prefab; // Podés arrastrar directamente tu script/componente desde el Inspector
    [SerializeField] private int initialSize = 20;

    private Queue<T> pool = new Queue<T>();

    private void Start()
    {
        for (int i = 0; i < initialSize; i++)
        {
            T newObject = InstantiateNew();
            pool.Enqueue(newObject);
        }
    }

    private T InstantiateNew() //funcion que se encarga devfolver un T instanciado y desactivado
    {
        T newObject = Instantiate(prefab);
        newObject.gameObject.SetActive(false);
        return newObject;
    }

    public T Get()
    {
        T instance;

        if (pool.Count > 0)
        {
            instance = pool.Dequeue();
        }
        else
        {
            // Si nos quedamos sin objetos en la Queue, instanciamos uno nuevo bajo demanda
            instance = InstantiateNew();
        }

        instance.gameObject.SetActive(true);
        return instance;
    }

    public void Recycle(T instance)
    {
        instance.gameObject.SetActive(false);
        pool.Enqueue(instance);
    }
}