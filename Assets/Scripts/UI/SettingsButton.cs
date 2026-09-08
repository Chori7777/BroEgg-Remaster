using UnityEngine;
using UnityEngine.UI;
public class SettingsButton : MonoBehaviour
{
    private void Awake()
    {
        // Le asignamos el listener al botón automáticamente por código
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(OnSettingsClicked);
    }

    private void OnSettingsClicked()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.OpenPanelByID("Settings");
        }
        else
        {
            Debug.LogWarning("[BackButton] No se encontró el UIManager.Instance en la escena papu.");
        }
    }
}

