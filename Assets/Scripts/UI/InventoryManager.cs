using System;
using ED262C;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] UnityEngine.UI.Image[] paraLaLinked; 
    [SerializeField] SimpleLinkedList<UnityEngine.UI.Image> invImages = new SimpleLinkedList<UnityEngine.UI.Image>();
    int ItemActual = 0;

    void Start()
    {
        for(int i = 0;  i < paraLaLinked.Length; i++)
        {
            invImages.Add(paraLaLinked[i]);
        }
        UpdateSelection();
    }

    void Update()
    {
        if (invImages.Count == 0) return;

        if (Input.GetKeyDown(KeyCode.Backspace)) Debug.Log("soy el slot " + ItemActual);

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ItemActual--;
            if(ItemActual < 0) ItemActual = invImages.Count - 1;
            UpdateSelection();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ItemActual++;
            if (ItemActual > invImages.Count - 1) ItemActual = 0;
            UpdateSelection();
        }
    }

    void UpdateSelection()
    {
        for(int i = 0; i < invImages.Count; i++)
        {
            if (i == ItemActual)
            {
                invImages[i].color = Color.blue;
            }
            else
            {
                invImages[i].color = Color.white;
            }
        }
    }
}
