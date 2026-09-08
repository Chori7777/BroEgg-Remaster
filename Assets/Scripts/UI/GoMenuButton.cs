using UnityEngine;
using UnityEngine.UI;
public class GoMenuButton : MonoBehaviour
{
    private void Awake()
    {
        // Le asignamos el listener al botón automáticamente por código
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(OnGoMenuClicked);
    }

    private void OnGoMenuClicked()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ClearToRoot();
        }
        else
        {
            Debug.LogWarning("[BackButton] No se encontró el UIManager.Instance en la escena papu.");
        }
    }
}
