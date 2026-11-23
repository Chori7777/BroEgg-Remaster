using UnityEngine;

// Se encarga de las armas que NO estan en la mano: las hace flotar alrededor del jugador
// y les manda disparar solas al enemigo mas cercano.
// El disparo manual del arma seleccionada sigue en PlayerWeaponManager, aca no se toca.
public class PassiveWeaponFire : MonoBehaviour
{
    [SerializeField] private InventaryWeapons inventoryManager;

    [Header("Deteccion")]
    [SerializeField] private float detectionRange = 8f;
    [Tooltip("Cada cuantos segundos se vuelve a buscar la lista de enemigos en la escena")]
    [SerializeField] private float refreshInterval = 0.2f;

    [Header("Flotacion")]
    [SerializeField] private float orbitRadius = 1.2f;
    [SerializeField] private float bobAmount = 0.1f;
    [SerializeField] private float bobSpeed = 3f;

    private IWeapon lastSelectedWeapon;
    private Enemy[] enemies = new Enemy[0];
    private float refreshTimer;

    private void Update()
    {
        if (LevelManager.Instance != null && LevelManager.Instance.ControlsBlocked) return;
        if (inventoryManager == null) return;

        int count = inventoryManager.WeaponCount;
        if (count == 0) return;

        int selected = inventoryManager.SelectedIndex;
        IWeapon selectedWeapon = inventoryManager.GetWeapon(selected);

        // Si cambio el arma de la mano, la nueva vuelve a su lugar en la mano
        if (selectedWeapon != lastSelectedWeapon)
        {
            ResetSelectedWeapon(selectedWeapon);
            lastSelectedWeapon = selectedWeapon;
        }

        RefreshEnemies();

        int passiveCount = count - 1;
        int slot = 0;

        for (int i = 0; i < count; i++)
        {
            if (i == selected) continue;

            IWeapon weapon = inventoryManager.GetWeapon(i);
            if (weapon == null) continue;

            Transform weaponTransform = weapon.getTransform();
            if (weaponTransform == null) continue;

            // 1) la ponemos flotando en su lugar del circulo
            Vector2 slotPosition = GetOrbitPosition(slot, passiveCount);
            weaponTransform.position = slotPosition;
            slot++;

            // 2) buscamos el enemigo mas cercano a esta arma
            Enemy target = GetClosestEnemy(slotPosition);

            if (target == null)
            {
                // sin enemigos mira hacia afuera del jugador
                AimWeapon(weaponTransform, slotPosition - (Vector2)transform.position);
                continue;
            }

            // 3) apuntamos al enemigo y, si puede, dispara
            Vector2 targetPosition = target.transform.position;
            AimWeapon(weaponTransform, targetPosition - slotPosition);

            if (weapon.CanShoot)
            {
                weapon.PassiveShoot(targetPosition, GetVelocity(target));
                if(AudioManager.Instance != null) AudioManager.Instance.PlaySFX(SoundID.PassiveShoot);
            }
        }
    }

    // Buscar en toda la escena es caro, asi que solo actualizamos la lista cada refreshInterval segundos
    private void RefreshEnemies()
    {
        refreshTimer -= Time.deltaTime;
        if (refreshTimer > 0f) return;

        refreshTimer = refreshInterval;
        enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
    }

    // Reparte las armas en un circulo alrededor del jugador, con un pequeño movimiento de arriba hacia abajo
    private Vector2 GetOrbitPosition(int slot, int passiveCount)
    {
        float angle = (360f / Mathf.Max(1, passiveCount)) * slot + 90f;
        Vector2 offset = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * orbitRadius;
        float bob = Mathf.Sin(Time.time * bobSpeed + slot) * bobAmount;

        return (Vector2)transform.position + offset + Vector2.up * bob;
    }

    private Enemy GetClosestEnemy(Vector2 origin)
    {
        Enemy closest = null;
        float closestDistance = float.MaxValue;
        float maxDistance = detectionRange * detectionRange;

        for (int i = 0; i < enemies.Length; i++)
        {
            // un enemigo destruido despues de la ultima busqueda queda en null
            if (enemies[i] == null) continue;

            // el rango se mide desde el jugador, no desde el arma
            float distanceToPlayer = ((Vector2)enemies[i].transform.position - (Vector2)transform.position).sqrMagnitude;
            if (distanceToPlayer > maxDistance) continue;

            float distance = ((Vector2)enemies[i].transform.position - origin).sqrMagnitude;
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = enemies[i];
            }
        }
        return closest;
    }

    private Vector2 GetVelocity(Enemy target)
    {
        Rigidbody2D body = target.GetComponent<Rigidbody2D>();
        if (body == null) return Vector2.zero;
        return body.linearVelocity;
    }

    // Rota el arma hacia la direccion y la espeja en Y si mira a la izquierda para que el sprite no quede dado vuelta
    private void AimWeapon(Transform weaponTransform, Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.0001f) return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        weaponTransform.rotation = Quaternion.Euler(0, 0, angle);

        Vector3 scale = weaponTransform.localScale;
        scale.y = Mathf.Abs(scale.y) * (direction.x < 0f ? -1f : 1f);
        weaponTransform.localScale = scale;
    }

    // El arma de la mano vuelve a la posicion y rotacion que tenia cuando se creo (ver PlayerWeaponManager.GiveWeapon)
    private void ResetSelectedWeapon(IWeapon weapon)
    {
        if (weapon == null) return;

        Transform weaponTransform = weapon.getTransform();
        if (weaponTransform == null) return;

        weaponTransform.localPosition = Vector3.zero;
        weaponTransform.localRotation = Quaternion.identity;

        Vector3 scale = weaponTransform.localScale;
        scale.y = Mathf.Abs(scale.y);
        weaponTransform.localScale = scale;
    }
}