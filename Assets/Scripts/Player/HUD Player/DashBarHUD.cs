using UnityEngine;
using UnityEngine.UI;

public class DashBarHUD : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Image dashBarImage;
    [SerializeField] private float maxDashCooldown = 2f; // El mismo 2 de Walk.cs

    private void Update()
    {
        if (playerMovement == null || dashBarImage == null) return;

        dashBarImage.fillAmount = Mathf.Clamp01(playerMovement.DashCooldown / maxDashCooldown);
    }
}

