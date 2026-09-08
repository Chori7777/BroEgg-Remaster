using UnityEngine;

public class RifleScript : MonoBehaviour,IWeapon
{
    [SerializeField] GameObject bulletPrefab;

    [SerializeField] private BulletPool bulletPool;

    public void shoot()
    {
        PlayerStats stats = GetComponentInParent<PlayerStats>();
        Collider2D playerCollider = stats.GetComponent<Collider2D>();

        int damage = DamageCalculator.CalculateDamage(stats, DamageCalculator.DamageType.Normal);

        // 1. Pedimos la bala al pool
        Bullet bullet = bulletPool.Get();

        // 2. Le asignamos la referencia del pool a la bala
        bullet.pool = bulletPool;

        // 3. Posicionamos la bala en la pistola
        bullet.transform.position = transform.position;
        bullet.transform.rotation = transform.rotation;

        // 4. Calculamos la dirección del mouse desde la pistola UNA sola vez
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // sin esto, dependiendo que tan cerca esta el mouse de la pistola, cambia la velocidad de la bala
        mousePosition.z = transform.position.z;

        Vector2 direction = (mousePosition - transform.position).normalized;

        // 5. Mandamo la direccion a la bala noma Y EL DAÑO, TAMBIEN EL DAÑO, para que le diga a su linear velocity para donde ir y saber que daño hacer
        bullet.Setup(direction,damage,playerCollider);
        Debug.Log("el rifle está disparando");

       

     

       
      
    

      
    }
    public Transform getTransform()
    {
        return transform;
    }
}
