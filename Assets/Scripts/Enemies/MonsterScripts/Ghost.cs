using UnityEngine;

public class Ghost : Enemy
{
    protected override void Update()
    {
        Vector2 direction = (player.transform.position - transform.position).normalized;
        rb.linearVelocity = direction * stats.Speed;
        base.Update();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(stats.Damage);
        }
    }
}