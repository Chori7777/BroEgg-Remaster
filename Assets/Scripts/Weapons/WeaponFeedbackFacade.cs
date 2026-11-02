using Unity.Cinemachine;
using UnityEngine;

public class WeaponFeedbackFacade : MonoBehaviour
{
    [SerializeField] private SoundID shootSound = SoundID.None;
    [SerializeField] private float shakeStrength = 0.1f;


    private WeaponRecoil recoil;
    private CinemachineImpulseSource impulse;

    private void Awake()
    {
        impulse = GetComponent<CinemachineImpulseSource>();
        if (impulse == null)
        {
            Debug.LogError("No se encontro el CinemachineImpulseSource en el arma.");
        }
        recoil = GetComponent<WeaponRecoil>();
        if (recoil == null)
        {
            Debug.LogError("No se encontro el WeaponRecoil en el arma.");
        }
    }
    
    public void OnShoot()
    {
        if (recoil != null) recoil.Kick();
        if (impulse!= null) impulse.GenerateImpulse(shakeStrength);

        if(shootSound!= SoundID.None&& AudioManager.Instance!= null)
        {
            AudioManager.Instance.PlaySFX(shootSound);
        }    
    }
}
