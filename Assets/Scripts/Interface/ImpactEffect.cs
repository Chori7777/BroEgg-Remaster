using UnityEngine;

public interface ImpactEffect
{
    public void SpawnParticles(ParticleSystem effectPrefab, Vector2 position, Vector2 normal);
}
