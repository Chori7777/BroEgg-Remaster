using UnityEngine;
public enum ImpactType
{
    Blood,
    WallSpark,
    Dust,
    SuicideExplosion
}

public class VFXManager : MonoBehaviour
{
    private static VFXManager instance;
    public static VFXManager Instance => instance;

    [Header("Prefabs de Partículas")]
    [SerializeField] private ParticleSystem bloodPrefab;
    [SerializeField] private ParticleSystem wallSparkPrefab;
    [SerializeField] private ParticleSystem dustPrefab;
    [SerializeField] private GameObject suicideExplosion;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void PlayImpact(ImpactType type, Vector2 position, Vector2 normal)
    {
        ParticleSystem particle = GetParticles(type);
        if (particle == null) return;

        // Rota las partículas para que salgan en la dirección de la normal del impacto
        Quaternion rotation = Quaternion.LookRotation(Vector3.forward, normal);
        ParticleSystem particles = Instantiate(particle, position, rotation);

        // Limpieza automática por si olvidaste poner Stop Action = Destroy en el prefab
        Destroy(particles.gameObject, particles.main.duration + particles.main.startLifetime.constantMax);
    }

    public void PlayPrefab(ImpactType type, Vector2 position, Vector2 normal, float duration)
    {
        {
            GameObject particle = GetPrefab(type);
            if (particle == null) return;

            // Rota las partículas para que salgan en la dirección de la normal del impacto
            Quaternion rotation = Quaternion.LookRotation(Vector3.forward, normal);

            GameObject particles = Instantiate(particle, position, rotation);

            Debug.Log("PArticulas de explosion suicide");

            // Limpieza automática por si olvidaste poner Stop Action = Destroy en el prefab
            Destroy(particles.gameObject, duration);
        }
    }

    private ParticleSystem GetParticles(ImpactType type)
    {
        switch (type)
        {
            case ImpactType.Blood:
                return bloodPrefab;
            case ImpactType.WallSpark:
                return wallSparkPrefab;
            case ImpactType.Dust:
                return dustPrefab;
            default:
                return null;
        }
    }

    private GameObject GetPrefab(ImpactType type)
    {
        switch (type)
        {
            case ImpactType.SuicideExplosion:
                return suicideExplosion;
            default:
                return null;
        }
    }
}
