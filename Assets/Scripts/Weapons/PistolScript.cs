using UnityEngine;

public class PistolScript : MonoBehaviour, IWeapon
{
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float defaultBulletSpeed; 

    public void shoot()
    {
        //La logica para pensar como disparar en nuestro juego es la siguiente:
        // Al hacer click en la pantalla, se guarda un punto en el mapa, a partir de ese punto
        // hay que calcular la distancia entre el click y nuestro personaje, para luego disparar en esa direccion
        // despues se hacen los calculos para que la bala se mueva en esa direccion y a la velocidad que queremos


        //Transforma las coordenadas del mouse en la pantalla a Coordenadas de Unity
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Calcula la dirección del disparo desde la posición de la pistola hacia el mouse
        Vector2 direction = mousePosition - transform.position;
        //Se normaliza la direcion 
        direction.Normalize();

        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        rb.linearVelocity = direction * defaultBulletSpeed;
        Debug.Log("la pistola esta disparando");
    }
    public Transform getTransform()
    {
        return transform;
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
