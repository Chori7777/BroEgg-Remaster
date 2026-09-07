using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectsInventory : MonoBehaviour
{
    public List<Object> objectList = new List<Object>();
    public Dictionary<string, Object> objectDictionary = new Dictionary<string, Object>();

    // Se aplica toda la lista de objetos en el diccionario
    void Start()
    {
        for (int i = 0; i < objectList.Count; i++)
        {
            objectDictionary.Add(objectList[i].IdObject, objectList[i]);

        }
    }
    

    public Object ChooseObject()
    {
        if(objectDictionary.Count == 0)
        {  return null; }

        int randomIndex = Random.Range(0, objectDictionary.Count);
        return objectDictionary.Values.ElementAt(randomIndex);
    //esta linea agarra todos los valores del diccionario en el "Values" y con elementAt en randomindex agarra un objeto random de los valores q reconocio antes en Values
    }

    void Update()
    {

    }
}
