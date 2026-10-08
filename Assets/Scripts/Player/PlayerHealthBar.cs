using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private PlayerStats player;
    [SerializeField] private Image healthBar;
    [SerializeField] private float smoothSpeed = 8f;

    void Update()
    {
        if (player == null) return;

        float target = (float)player.Health / player.MaxHealth;

        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, target, smoothSpeed * Time.deltaTime);
    }
}