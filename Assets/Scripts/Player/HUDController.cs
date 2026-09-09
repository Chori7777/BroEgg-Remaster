using UnityEngine;
using TMPro;

public class HUDController : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerGold playerGold;

    [SerializeField] private TMP_Text healthBar;
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text roundText;
    [SerializeField] private TMP_Text timerText;
    // Observers en Player health y Player Gold, en On Enable se suscribe el HUDController a los eventos de cambio de salud y oro, y en On Disable se desuscribe para evitar errores cuando el objeto se desactive.
    private void OnEnable()
    {
        playerHealth.OnHealthChanged += UpdateHealthText;
        playerGold.OnGoldChanged += UpdateGoldText;
    }

    private void OnDisable()
    {
        playerHealth.OnHealthChanged -= UpdateHealthText; 
        playerGold.OnGoldChanged -= UpdateGoldText;
    }

    private void UpdateHealthText(int current, int max)
    {
        healthBar.text = (current + "/" + max);
    }

    private void UpdateGoldText(int amount)
    {
               goldText.text = amount.ToString();
    }
    void Update()
    {
        roundText.text = "Ronda" + LevelManager.Instance.CurrentRound;
        // Otra vez cosas raras de Mathf, lo que hace CeilToInt es redondear hacia arriba, asi que si quedan 9.5 segundos, va a mostrar 10, y si quedan 9.1 segundos
        // va a mostrar 10 tambien. Esto es para que el jugador vea un numero entero de tiempo restante y no un decimal!
        timerText.text=Mathf.CeilToInt(LevelManager.Instance.TimeRemaining).ToString();
    }
}
