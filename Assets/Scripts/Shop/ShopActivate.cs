using NUnit.Framework;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UI;


public class ShopActivate : MonoBehaviour
{

    [SerializeField] Image shopPanel;
    [SerializeField] Image Objeto1;
    [SerializeField] Image Objeto2;
    [SerializeField] Image Objeto3;
    [SerializeField] Image Objeto4;
    [SerializeField] ObjectsInventory inventory;
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
            Objeto1.sprite = inventory.ChooseObject().SpriteShop;
            Objeto2.sprite = inventory.ChooseObject().SpriteShop;
            Objeto3.sprite = inventory.ChooseObject().SpriteShop;
            Objeto4.sprite = inventory.ChooseObject().SpriteShop;
            shopActive = true;
        }

        if(shopActive == true)
        {
            timershop -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.O) && timershop <= 0 && shopActive == true)
        {
            shopPanel.gameObject.SetActive(false);
            timershop = 2;
            shopActive = false;
        }
    }
}
