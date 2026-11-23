using System.Collections.Generic;
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

    // cono de disparo
    [Header("Cono de disparo")]
    [Tooltip("Angulo TOTAL del cono. Cada arma solo dispara a enemigos dentro de este cono, mirando hacia afuera del jugador")]
    [SerializeField, Range(10f, 360f)] private float coneAngle = 90f;

    [Header("Flotacion")]
    [SerializeField] private float orbitRadius = 1.2f;
    [SerializeField] private float bobAmount = 0.1f;
    [SerializeField] private float bobSpeed = 3f;

    // opacidad de las pasivas
    [Header("Visual")]
    [Tooltip("Opacidad de las armas pasivas (el arma de la mano siempre al 100%)")]
    [SerializeField, Range(0f, 1f)] private float passiveAlpha = 0.5f;
    // cuantas capas por encima de las pasivas se dibuja el arma de la mano
    [Tooltip("Capas (Order in Layer) extra del arma de la mano para que se vea por encima de las pasivas")]
    [SerializeField] private int heldOrderBoost = 1;

    // guarda el Order in Layer original de cada sprite para poder subirlo/bajarlo sin acumular
    private Dictionary<SpriteRenderer, int> baseSortingOrders = new Dictionary<SpriteRenderer, int>();

    private IWeapon lastSelectedWeapon;
    private int lastWeaponCount = -1; // para volver a aplicar la opacidad si se agrega o saca un arma
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

        // opacidad y capa de dibujado se aplican solo cuando cambia el arma de la mano o la cantidad de armas
        if (selectedWeapon != lastSelectedWeapon || count != lastWeaponCount)
        {
            ApplyVisuals(selected, count);
            lastWeaponCount = count;
        }

        // Si cambio el arma de la mano, la nueva vuelve a su lugar en la mano
        if (selectedWeapon != lastSelectedWeapon)
        {
            ResetSelectedWeapon(selectedWeapon);
            lastSelectedWeapon = selectedWeapon;
        }

        // el arma de la mano tambien se espeja para no quedar dada vuelta
        KeepHeldWeaponUpright(selectedWeapon);

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

            // 1) la ponemos flotando en su lugar del circulo (outward = hacia donde "mira" esta arma)
            Vector2 slotPosition = GetOrbitPosition(slot, passiveCount, out Vector2 outward);
            weaponTransform.position = slotPosition;
            slot++;

            // 2) buscamos el enemigo mas cercano a esta arma, SOLO dentro de su cono
            Enemy target = GetClosestEnemy(slotPosition, outward);

            if (target == null)
            {
                // sin enemigos en su cono mira hacia afuera del jugador
                AimWeapon(weaponTransform, slotPosition - (Vector2)transform.position);
                continue;
            }

            // 3) apuntamos al enemigo y, si puede, dispara
            Vector2 targetPosition = target.transform.position;
            AimWeapon(weaponTransform, targetPosition - slotPosition);

            if (weapon.CanShoot)
            {
                weapon.PassiveShoot(targetPosition, GetVelocity(target));
                if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(SoundID.PassiveShoot);
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
    // ademas devuelve outward, la direccion "hacia afuera" de ese lugar del circulo (es el centro de su cono)
    private Vector2 GetOrbitPosition(int slot, int passiveCount, out Vector2 outward)
    {
        float angle = (360f / Mathf.Max(1, passiveCount)) * slot + 90f;
        outward = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        Vector2 offset = outward * orbitRadius;
        float bob = Mathf.Sin(Time.time * bobSpeed + slot) * bobAmount;

        return (Vector2)transform.position + offset + Vector2.up * bob;
    }

    private Enemy GetClosestEnemy(Vector2 origin, Vector2 forward)
    {
        Enemy closest = null;
        float closestDistance = float.MaxValue;
        float maxDistance = detectionRange * detectionRange;
        float halfCone = coneAngle * 0.5f;

        for (int i = 0; i < enemies.Length; i++)
        {
            // un enemigo destruido despues de la ultima busqueda queda en null
            if (enemies[i] == null) continue;

            // el rango se mide desde el jugador, no desde el arma
            float distanceToPlayer = ((Vector2)enemies[i].transform.position - (Vector2)transform.position).sqrMagnitude;
            if (distanceToPlayer > maxDistance) continue;

            // el enemigo tiene que estar dentro del cono de esta arma
            Vector2 toEnemy = (Vector2)enemies[i].transform.position - origin;
            if (Vector2.Angle(forward, toEnemy) > halfCone) continue;

            float distance = toEnemy.sqrMagnitude;
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

    // NUEVO: el arma de la mano al 100% de opacidad y heldOrderBoost capas por encima, las demas con passiveAlpha y su capa original
    private void ApplyVisuals(int selected, int count)
    {
        for (int i = 0; i < count; i++)
        {
            IWeapon weapon = inventoryManager.GetWeapon(i);
            if (weapon == null) continue;

            Transform weaponTransform = weapon.getTransform();
            if (weaponTransform == null) continue;

            bool isHeld = (i == selected);
            float alpha = isHeld ? 1f : passiveAlpha;

            SpriteRenderer[] renderers = weaponTransform.GetComponentsInChildren<SpriteRenderer>();
            for (int r = 0; r < renderers.Length; r++)
            {
                // la primera vez que vemos este sprite guardamos su Order in Layer original
                if (!baseSortingOrders.ContainsKey(renderers[r]))
                    baseSortingOrders[renderers[r]] = renderers[r].sortingOrder;

                renderers[r].sortingOrder = baseSortingOrders[renderers[r]] + (isHeld ? heldOrderBoost : 0);

                Color color = renderers[r].color;
                color.a = alpha;
                renderers[r].color = color;
            }
        }
    }

    // el arma de la mano se espeja en Y cuando apunta a la izquierda, igual que las pasivas, para que el sprite no quede dado vuelta.
    // Se decide con la direccion del PADRE (el pivot que apunta al mouse) y no la del arma, asi el giro de recarga no hace parpadear el espejado.
    // Tambien tiene en cuenta si el padre ya esta espejado (por WeaponAim) para no espejarla dos veces.
    private void KeepHeldWeaponUpright(IWeapon weapon)
    {
        if (weapon == null) return;

        Transform weaponTransform = weapon.getTransform();
        if (weaponTransform == null || weaponTransform.parent == null) return;

        Transform parent = weaponTransform.parent;

        bool facingLeft = parent.right.x < 0f;
        float desiredSign = facingLeft ? -1f : 1f;

        bool parentMirrored = parent.lossyScale.y < 0f;
        float localSign = parentMirrored ? -desiredSign : desiredSign;

        Vector3 scale = weaponTransform.localScale;
        scale.y = Mathf.Abs(scale.y) * localSign;
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