using UnityEngine;

public class EyeFollow : MonoBehaviour
{
    [SerializeField] private Transform retina;
    [SerializeField] private float maxDistance = 0.3f;
    [SerializeField] private float smoothTime = 0.05f;

    private Camera cam;
    private Vector2 velocity;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        Vector3 mouse = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (Vector2)mouse - (Vector2)transform.position;

        // Limita cuanto se puede alejar la retina del centro del ojo
        Vector2 target = Vector2.ClampMagnitude(dir, maxDistance);

        Vector2 current = retina.localPosition;
        retina.localPosition = Vector2.SmoothDamp(current, target, ref velocity, smoothTime);
    }
}