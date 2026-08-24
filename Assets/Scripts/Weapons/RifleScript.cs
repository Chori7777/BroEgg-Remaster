using UnityEngine;

public class RifleScript : MonoBehaviour,IWeapon
{
    public void shoot()
    {
        Debug.Log("Soy el rifle y estoy disparando");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
