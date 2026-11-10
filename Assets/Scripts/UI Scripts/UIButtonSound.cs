using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonSound : MonoBehaviour
{
    [SerializeField] private SoundID sound = SoundID.ButtonClick;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
           if(AudioManager.Instance != null) AudioManager.Instance.PlaySFX(sound);
        });
    }
}