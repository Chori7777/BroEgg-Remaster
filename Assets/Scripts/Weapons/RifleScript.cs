using UnityEngine;

public class RifleScript : MonoBehaviour,IWeapon
{
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float defaultBulletSpeed;
    public Transform getTransform()
    {
        return transform;
    }
   
    public void shoot()
    {
        Debug.Log("Soy el rifle y estoy disparando");
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
        Debug.Log("el rifle disparo");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is create
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
