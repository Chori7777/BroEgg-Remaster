using UnityEngine;
using UnityEngine.UI;
public class BackButton : MonoBehaviour
{
    private void Awake()
    {
        // Le asignamos el listener al botón automáticamente por código
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(OnBackClicked);
    }

    private void OnBackClicked()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.GoBack();
        }
        else
        {
            Debug.LogWarning("[BackButton] No se encontró el UIManager.Instance en la escena papu.");
        }
    }
}
