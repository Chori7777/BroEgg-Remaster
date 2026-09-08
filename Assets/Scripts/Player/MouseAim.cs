using UnityEngine;

public class MouseAim : MonoBehaviour
{

   
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

        transform.position = mouseWorldPos;

    }

    
}
