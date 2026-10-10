using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private GameObject panel;
    [SerializeField] private Button retryButton;
    [SerializeField] private TMP_Text infoText;

    private void Awake()
    {
        panel.SetActive(false);
        retryButton.onClick.AddListener(OnRetryClicked);
    }

    private void OnEnable() { playerHealth.OnPlayerDied += Show; }
    private void OnDisable() { playerHealth.OnPlayerDied -= Show; }

    private void Show()
    {
        bool hasCheckpoint = CheckpointManager.Instance != null && CheckpointManager.Instance.HasCheckpoint;
        retryButton.interactable = hasCheckpoint;

        if (infoText != null)
            infoText.text = "Moriste en la ronda " + LevelManager.Instance.CurrentRound;

        Cursor.visible = true;
        panel.SetActive(true);
        Time.timeScale = 0f;
    }

    private void OnRetryClicked()
    {
        if (CheckpointManager.Instance.RetryFromCheckpoint())
        {
            panel.SetActive(false);
            Time.timeScale = 1f;
        }
    }
}