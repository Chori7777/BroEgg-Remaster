using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public static ScreenShake Instance { get; private set; }

    private float shakeTime;
    private float shakeStrength;
    private Vector3 lastOffset;

    private void Awake() => Instance = this;

    private void LateUpdate()
    {
        // Primero devolvemos la cámara a la posición real que calculó tu CameraFollow
        transform.position -= lastOffset;
        lastOffset = Vector3.zero;

        if (shakeTime > 0f)
        {
            shakeTime -= Time.deltaTime;
            Vector2 offset = Random.insideUnitCircle * shakeStrength;
            lastOffset = (Vector3)offset;
            transform.position += lastOffset;
        }
    }

    public void Shake(float strength, float duration)
    {
        shakeStrength = strength;
        shakeTime = duration;
    }
}

/*public class ScreenShake : MonoBehaviour
{
    public static ScreenShake Instance;

    private float shakeTime;
    private float shakeStrength;
    private Vector3 basePos;

    void Awake() => Instance = this;

    void LateUpdate()
    {
        if (shakeTime > 0)
        {
            shakeTime -= Time.deltaTime;
            Vector2 offset = Random.insideUnitCircle * shakeStrength;
            transform.position = basePos + (Vector3)offset;
        }
        else
        {
            basePos = transform.position;
        }
    }

    public void Shake(float strength, float duration)
    {
        if (shakeTime <= 0) basePos = transform.position;
        shakeStrength = strength;
        shakeTime = duration;
    }
}*/