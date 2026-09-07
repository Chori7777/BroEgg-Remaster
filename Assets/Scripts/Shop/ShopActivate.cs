using NUnit.Framework;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UI;


public class ShopActivate : MonoBehaviour
{

    [SerializeField] Image shopPanel;
    private bool shopActive;
    [SerializeField] private float timershop = 2;
    void Start()
    {
        shopPanel.gameObject.SetActive(false);
        shopActive = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O) && shopActive == false)
        {
            shopPanel.gameObject.SetActive(true);
            Debug.Log("abriendo tienda");
            shopActive=true;
        }

        if(shopActive == true)
        {
            timershop -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.O) && timershop <= 0 && shopActive == true)
        {
            shopPanel.gameObject.SetActive(false);
            Debug.Log("cerrando tienda");
            timershop = 2;
            shopActive = false;
        }
    }
}
