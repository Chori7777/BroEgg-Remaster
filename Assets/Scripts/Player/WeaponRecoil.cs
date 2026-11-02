using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    [SerializeField] private Transform visual;      
    [SerializeField] private float recoilDistance = 0.15f;
    [SerializeField] private float returnSpeed = 15f;

    private Vector3 startLocalPos;

    void Start()
    {
        startLocalPos = visual.localPosition;
    }

    void Update()
    {
        // Vuelve suavemente a su posición original ( un papulerp)
        visual.localPosition = Vector3.Lerp(visual.localPosition, startLocalPos, returnSpeed * Time.deltaTime);
    }

    public void Kick()
    {
        // Empuja hacia atrás (el arma apunta hacia su derecha)
        visual.localPosition = startLocalPos + Vector3.left * recoilDistance;
    }
}