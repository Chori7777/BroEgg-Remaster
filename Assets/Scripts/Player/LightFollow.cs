using UnityEngine;

public class LightFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = Vector3.zero;
    [SerializeField] private float followSpeed = 5f;

    [Header("Movimiento")]
    [SerializeField] private float swayAmount = 0.3f;
    [SerializeField] private float swaySpeed = 2f;

    private Vector3 lastTargetPos;

    private void Start()
    {
        lastTargetPos = target.position;
    }

    private void LateUpdate()
    {
        if (Time.deltaTime <= 0f) return;

        Vector3 targetPos = target.position + offset;

        // pom pom pom 
        Vector3 velocity = (target.position - lastTargetPos) / Time.deltaTime;
        float moveFactor = Mathf.Clamp01(velocity.magnitude);
        Vector3 sway = new Vector3(
            Mathf.Sin(Time.time * swaySpeed),
            Mathf.Cos(Time.time * swaySpeed * 0.8f),
            0f) * swayAmount * moveFactor;

        transform.position = Vector3.Lerp(transform.position, targetPos + sway, followSpeed * Time.deltaTime);
        lastTargetPos = target.position;
    }
}