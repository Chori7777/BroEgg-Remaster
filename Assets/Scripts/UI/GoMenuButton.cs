using UnityEngine;
using UnityEngine.SceneManagement;
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
            Time.timeScale = 1f;
            SceneManager.LoadScene("Main Menu");
        }
        else
        {
            Debug.LogWarning("[BackButton] No se encontró el UIManager.Instance en la escena papu.");
        }
    }
}
