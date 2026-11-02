using UnityEngine;

public class ScreenShake : MonoBehaviour
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
}