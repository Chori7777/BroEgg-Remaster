using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private PlayerStats player;
    [SerializeField] private Image healthBar;

    void Update()
    {
        if (player == null) return;

        healthBar.fillAmount =
            (float)player.Health / player.MaxHealth;
    }
}