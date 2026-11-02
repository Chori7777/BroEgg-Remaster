using UnityEngine;

public class WeaponAim : MonoBehaviour
{
  [SerializeField] private Transform weaponTransform;
  [SerializeField] private float smoothSpeed = 15f;



    private float currentAngle;
    private Camera cam;
    void Start()
    {
        cam = Camera.main;
        currentAngle = weaponTransform.eulerAngles.z; //rotacion del objeto expresada en grados, especificamente z :v
    }

    // Update is called once per frame
    void Update()
    {
        if (LevelManager.Instance != null && LevelManager.Instance.ControlsBlocked) return;
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Mathf.Abs((cam.transform.position.z));

        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(mouseScreenPos);

        Vector3 direction = (mouseWorldPos - weaponTransform.position).normalized;

        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;


        float movement = smoothSpeed * Time.deltaTime;
        currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, movement);

        // Les aseguro q esto es lo mas raro q van a leer q hice, pero bueno, parece q es util, para hacer lo que escribi abajo:
        // Calcular el angulo entre el pivot y el mouse a partir del vector direccion, y rota el arma para que apunte ahi
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;


        weaponTransform.rotation = Quaternion.Euler(0, 0, currentAngle);
        Vector3 scale = weaponTransform.localScale;
        scale.y = Mathf.Abs(scale.y) * (direction.x < 0 ? -1 : 1);
        weaponTransform.localScale = scale;




    }
}
