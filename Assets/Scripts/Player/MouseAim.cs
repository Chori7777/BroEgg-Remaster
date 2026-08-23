using UnityEngine;

public class MouseAim : MonoBehaviour
{

    [SerializeField] private float speedCamera = 0.5f;
    private void Start()
    {
        // Cursor.visible = false;
    }
    void Update()
    {
        Vector3 MouseScreenPos = Input.mousePosition;

        MouseScreenPos.z = Mathf.Abs((Camera.main.transform.position.z));

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(MouseScreenPos);
        mouseWorldPos.z = 0f;

        transform.position = Vector3.Lerp(transform.position, mouseWorldPos, speedCamera * Time.deltaTime);

    }

    
}
