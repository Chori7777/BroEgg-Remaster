using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class SpawnerLight : MonoBehaviour
{
    [SerializeField] private float minIntensity = 0.6f;
    [SerializeField] private float maxIntensity = 1.2f;
    [SerializeField] private float speed = 8f;

    private Light2D lightSource;
    private float seed;

    private void Awake()
    {
        lightSource = GetComponent<Light2D>();

        // Cada luz arranca en un punto distinto del ruido, para que no parpadeen todas iguales
        seed = Random.value * 100f;
    }

    private void Update()
    {
        float noise = Mathf.PerlinNoise(seed, Time.time * speed);
        lightSource.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
    }
}