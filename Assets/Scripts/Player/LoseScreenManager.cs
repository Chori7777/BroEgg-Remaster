using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseScreenManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      Cursor.visible = true;
    }

    // Update is called once per frame
    public void RestartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void CloseGame()
            {
        Application.Quit();
    }
}
