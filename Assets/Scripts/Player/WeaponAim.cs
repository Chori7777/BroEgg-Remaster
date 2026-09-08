using UnityEngine;

public class WeaponAim : MonoBehaviour
{
  [SerializeField] private Transform weaponTransform;

    private Camera cam;
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mouseScreenPos=Input.mousePosition;
        mouseScreenPos.z = Mathf.Abs((cam.transform.position.z));

        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(mouseScreenPos);

        Vector3 direction = (mouseWorldPos - weaponTransform.position).normalized;
         // Les aseguro q esto es lo mas raro q van a leer q hice, pero bueno, parece q es util, para hacer lo que escribi abajo:
        // Calcular el angulo entre el pivot y el mouse a partir del vector direccion, y rota el arma para que apunte ahi
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        weaponTransform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
