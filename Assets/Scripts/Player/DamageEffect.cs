using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DamageEffect : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Light2D playerLight;
    // dimmedScale es a que proporcion del  tamaño original se achica la luz de golpe, 0.3f en este ejemplo seria 30%
    [SerializeField, Range(0f, 1f)] private float dimmedScale = 0.3f;
    // Que tan rapido recupera su tamaño original
    [SerializeField] private float recoverSpeed = 1.5f;

    // aca se guardan los radios originales de la luz para saber a donde volver cuando se recupere
    private float normalOuter;
    private float normalInner;
    private float intensity;

    private void Start()
    {
        // aca se leen los radios actuales, externo e interno y los guarda, solo se hace una vez
        normalOuter = playerLight.pointLightOuterRadius;
        normalInner = playerLight.pointLightInnerRadius;
    }

    private void OnEnable() => playerHealth.OnDamaged += HandleDamage;
    private void OnDisable() => playerHealth.OnDamaged -= HandleDamage;

    private void HandleDamage()
    {
        //la intensidad esta al maximo, no tenia pensado cambiarlo a menos q ustedes lo quiera :p
        intensity = 1f;
    }

    private void Update()
    {
      
        if (intensity <= 0f) return;
        // despues de recibir el daño, la intensidad va bajando hasta 0, y la luz vuelve a su tamaño original
        intensity = Mathf.MoveTowards(intensity, 0f, recoverSpeed * Time.deltaTime);

        float scale = Mathf.Lerp(1f, dimmedScale, intensity);
        playerLight.pointLightOuterRadius = normalOuter * scale;
        playerLight.pointLightInnerRadius = normalInner * scale;
    }
}