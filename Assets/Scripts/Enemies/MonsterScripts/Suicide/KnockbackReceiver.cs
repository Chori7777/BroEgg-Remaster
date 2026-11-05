using UnityEngine;

// Empuje temporal para cualquier Rigidbody2D.
// El jugador y los enemigos reasignan rb.linearVelocity en cada frame desde sus scripts de movimiento,
// asi que un AddForce se pierde al instante. Este componente fija la velocidad en FixedUpdate,
// despues de los demas scripts (DefaultExecutionOrder alto), mientras dura el knockback.
[DefaultExecutionOrder(1000)]
[RequireComponent(typeof(Rigidbody2D))]
public class KnockbackReceiver : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 velocity;
    private float duration;
    private float timeLeft;

    public static void Apply(Rigidbody2D body, Vector2 velocity, float duration)
    {
        if (body == null || velocity == Vector2.zero || duration <= 0f) return;

        KnockbackReceiver receiver = body.GetComponent<KnockbackReceiver>();
        if (receiver == null)
        {
            receiver = body.gameObject.AddComponent<KnockbackReceiver>();
        }

        receiver.Begin(body, velocity, duration);
    }

    private void Begin(Rigidbody2D body, Vector2 knockbackVelocity, float knockbackDuration)
    {
        rb = body;
        velocity = knockbackVelocity;
        duration = knockbackDuration;
        timeLeft = knockbackDuration;
    }

    private void FixedUpdate()
    {
        if (timeLeft <= 0f || rb == null) return;

        timeLeft -= Time.fixedDeltaTime;

        // El empuje se va apagando hasta llegar a 0
        float strength = Mathf.Clamp01(timeLeft / duration);
        rb.linearVelocity = velocity * strength;
    }
}
