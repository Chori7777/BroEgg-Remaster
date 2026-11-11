using UnityEngine;
using TMPro;

public class HUDController : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerGold playerGold;
    [SerializeField] private PlayerWeaponManager pwm;

    private WeaponController wc;

    [SerializeField] private TMP_Text healthBar;
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text roundText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text weaponCurrentAmmo;

    private void OnEnable()
    {
        if (playerHealth != null) playerHealth.OnHealthChanged += UpdateHealthText;
        if (playerGold != null) playerGold.OnGoldChanged += UpdateGoldText;
    }

    private void OnDisable()
    {
        if (playerHealth != null) playerHealth.OnHealthChanged -= UpdateHealthText;
        if (playerGold != null) playerGold.OnGoldChanged -= UpdateGoldText;
        UnsubscribeCurrentWeapon();
    }

    private void UpdateActiveWeapon()
    {
        if (pwm == null || pwm.inventoryManager == null) return;

        WeaponController newWeapon = pwm.inventoryManager.currentWeapon;

        // Si el arma en la mano cambió, actualizamos las suscripciones
        if (newWeapon != wc)
        {
            UnsubscribeCurrentWeapon();
            wc = newWeapon;

            if (wc != null)
            {
                wc.OnAmmoChanged += UpdateAmmoHUD;

                UpdateAmmoHUD(wc.WeaponData.MagazineSize, wc.WeaponData.MagazineSize);
            }
            else
            {
                if (weaponCurrentAmmo != null) weaponCurrentAmmo.text = "-/-";
            }
        }
    }

    private void UnsubscribeCurrentWeapon()
    {
        if (wc != null)
        {
            wc.OnAmmoChanged -= UpdateAmmoHUD;
        }
    }

    private void UpdateHealthText(int current, int max)
    {
        if (healthBar != null) healthBar.text = current + "/" + max;
    }

    private void UpdateGoldText(int amount)
    {
        if (goldText != null) goldText.text = amount.ToString();
    }

    private void UpdateAmmoHUD(int currentAmmo, int totalAmmo)
    {
        if (weaponCurrentAmmo != null) weaponCurrentAmmo.text = currentAmmo + "/" + totalAmmo;
    }

    void Update()
    {
        if (LevelManager.Instance != null)
        {
            if (roundText != null) roundText.text = "Ronda " + LevelManager.Instance.CurrentRound;
            if (timerText != null) timerText.text = Mathf.CeilToInt(LevelManager.Instance.TimeRemaining).ToString();
        }

        UpdateActiveWeapon();
    }
}