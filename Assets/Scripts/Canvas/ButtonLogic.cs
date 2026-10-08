using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonLogic : MonoBehaviour
{
    [SerializeField] private ShopManager shopManager;

    private ShopProductData product;

    [Header("Nuevo contenido Genial de la increible tienda de BroEgg")]
    [SerializeField] private Image iconImage;          // 1) Icono del producto
    [SerializeField] private TMP_Text nameText;           // 2) Nombre
    [SerializeField] private TMP_Text statsAndDescText;   // 3) Stats + descripcion 
    [SerializeField] private Button buyButton;          // 4) Boton de comprar
    [SerializeField] private TMP_Text buyButtonLabel;     // 5) Texto del boton 

    private PlayerGold gold;
    private InventaryManager inventory;

    [Header("Colorines del precio para los que checan el oro!!!")]
    [SerializeField] private Color colorComprar = Color.green;
    [SerializeField] private Color colorCaro = Color.red;
    [SerializeField] private Color colorAdquirido = Color.gray;

    private void Awake()
    {
        if (buyButton == null) buyButton = GetComponent<Button>();
        gold = FindFirstObjectByType<PlayerGold>();
        inventory = FindFirstObjectByType<InventaryManager>();
    }
    private void OnEnable()
    {
        if (gold != null) gold.OnGoldChanged += OnOreChange;
        if (inventory != null) inventory.OnInventoryChanged += OnInventaryChange;
    }

    private void OnDisable()
    {
        if (gold != null) gold.OnGoldChanged -= OnOreChange;
        if (inventory != null) inventory.OnInventoryChanged -= OnInventaryChange;
    }

    private void OnOreChange(int nuevoOro)
    {
        UpdateBuyButtonState();
    }

    private void OnInventaryChange()
    {
        UpdateBuyButtonState();
    }

    public void Setup(ShopProductData product, ShopManager manager)
    {
        this.product = product;
        shopManager = manager;

        // El padre es la tarjeta entera si no hay producto, se oculta TODO el slot, por si acaso
        GameObject rootCard = transform.parent.gameObject;
        rootCard.SetActive(product != null);

        if (product == null)
        {
            return;
        }

        if (iconImage != null) iconImage.sprite = product.icon;
        if (nameText != null) nameText.text = product.displayName;

        if (statsAndDescText != null)
        {
            // bueno aca se van sumando cosas al texto: stats primero, descripcion despues.
            string bloqueStats = ObtenerStats(product);
            string bloqueDesc = product.description;

            bool hayStats = !string.IsNullOrEmpty(bloqueStats);
            bool hayDesc = !string.IsNullOrEmpty(bloqueDesc);

            // chequeos para acomodar el texto, si hay estadisticas y descripcion, se pone un salto de linea entre ambos
            // si no hay stats, solo se pone la descripcion, si no hay descripcion, solo se ponen las stats
            if (hayStats && hayDesc)
                statsAndDescText.text = bloqueStats + "\n" + bloqueDesc;
            else if (hayStats)
                statsAndDescText.text = bloqueStats;
            else
                statsAndDescText.text = bloqueDesc;
        }

        UpdateBuyButtonState();
    }

    private string ObtenerStats(ShopProductData data)
    {
        // bueno aca se van sumando cosas al texto, si es un objeto de stats, se suman las stats, si es un arma, se suman las stats del arma, lo del arma esta mas abajo
        string texto = "";
        // si es un objeto de stats, se suman las stats
        if (data is ObjectShopData item)
        {
            // aca se hace el chequeo que me mostraron, si el valor es diferente a 0 se muestra en las estadisticas , si es 0 no se muestra, para que no se vea feo
            if (item.BonusMaxHealth != 0)
                texto += "Vida +" + item.BonusMaxHealth + "\n";
            if (item.BonusArmor != 0)
                texto += "Armadura +" + item.BonusArmor + "\n";
            if (item.BonusDamage != 0)
                texto += "Poder +" + item.BonusDamage + "\n";
            if (item.BonusSpeed != 0)
                texto += "Velocidad +" + item.BonusSpeed + "\n";
            if (item.BonusHealthRegeneration != 0)
                texto += "Regeneracion de vida +" + item.BonusHealthRegeneration + "\n";
            if (item.BonusCritDamage != 0)
                texto += "Poder critico +" + item.BonusCritDamage + "\n";
            if (item.BonusCriticalChance != 0)
                texto += "Probabilidad de critico +" + item.BonusCriticalChance + "\n";
            if (item.BonusDodgeChance != 0)
                texto += "Probabilidad de esquivar +" + item.BonusDodgeChance + "\n";
            if (item.BonusHarvesting != 0)
                texto += "Recoleccion +" + item.BonusHarvesting + "\n";
            if (item.BonusCurse != 0)
                texto += "Maldicion +" + item.BonusCurse + "\n";
        }
        else if (data is WeaponShopData weapon && weapon.weaponPrefab != null)
        {
            // y aca se agarra el weaponData del arma y se muestran las stats del arma, si es null no se muestra nada
            WeaponData weaponData = weapon.weaponPrefab.WeaponData;
            if (weaponData == null) return texto;
            // lo del F2 es para q sean solo 2 decimales we
            texto += "Poder base " + weaponData.BaseDamage + "\n";
            texto += "Cargador  " + weaponData.MagazineSize + "\n";
            texto += "Cadencia " + weaponData.FireRate.ToString("F2") + "s\n";
            texto += "Recarga " + weaponData.ReloadTime.ToString("F2") + "s\n";
            texto += "Tipo: " + weaponData.GetShootingType;
        }
        // y devolvemos el texto ya armado con todas nuestras estadisticas hermosas
        return texto;
    }

    public void BuyProduct()
    {
        if (shopManager == null)
        {
            return;
        }

        bool bought = shopManager.TryBuy(product);

        if (bought)
        {
            UpdateBuyButtonState();
        }
    }

    private void UpdateBuyButtonState()
    {
        if (product == null) return;

      

        // el bool de adquirido es: si el producto es un objeto, y si el inventario no es nulo, chequear si esta el objeto con un id dentro de ese inventario...
        bool adquired;

        if (product is ObjectShopData obj && inventory != null)
        {
            adquired = inventory.Contains(obj.id);
        }
        else if (product is WeaponShopData wpn && wpn.weaponPrefab != null)
        {
            string weaponId = wpn.weaponPrefab.WeaponData.IdWeapon;
            PlayerWeaponManager wm = FindFirstObjectByType<PlayerWeaponManager>();
            adquired = (wm != null) && wm.HasWeapon(weaponId);
        }
        else
        {
            adquired = false;
        }

        bool enoughGold = gold == null || gold.CurrentGold >= product.price;
        bool canBuy = !adquired && enoughGold;

        if (buyButton != null) buyButton.interactable = canBuy;

        if (buyButtonLabel != null)
        {
            if (adquired) buyButtonLabel.color = colorAdquirido;
            else if (canBuy) buyButtonLabel.color = colorComprar;
            else buyButtonLabel.color = colorCaro;
                     //chequea si adquired es true o si es falso
            buyButtonLabel.text = adquired ? "Adquirido": $"Comprar \n<size=80%>{product.price} g</size>";
        }
    }
}