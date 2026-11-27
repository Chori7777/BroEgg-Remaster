using UnityEngine;
public enum ImpactType
{
    Blood,
    WallSpark,
    Dust
}

public class VFXManager : MonoBehaviour
{
    private static VFXManager instance;
    public static VFXManager Instance => instance;

    [Header("Prefabs de Partículas")]
    [SerializeField] private ParticleSystem bloodPrefab;
    [SerializeField] private ParticleSystem wallSparkPrefab;
    [SerializeField] private ParticleSystem dustPrefab;

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
        ParticleSystem prefab = GetPrefab(type);
        if (prefab == null) return;

        // Rota las partículas para que salgan en la dirección de la normal del impacto
        Quaternion rotation = Quaternion.LookRotation(Vector3.forward, normal);
        ParticleSystem particles = Instantiate(prefab, position, rotation);

        // Limpieza automática por si olvidaste poner Stop Action = Destroy en el prefab
        Destroy(particles.gameObject, particles.main.duration + particles.main.startLifetime.constantMax);
    }

    private ParticleSystem GetPrefab(ImpactType type)
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
}
