using UnityEngine;
using static WeaponData;

public interface IWeapon
{

    void shoot();

    // Disparo automatico (arma que no esta en la mano). El daño sale reducido a la mitad
    void PassiveShoot(Vector2 targetPosition, Vector2 targetVelocity);

    // true si la cadencia y la recarga permiten disparar en este momento
    bool CanShoot { get; }

    public WeaponController GetWeaponController();
    Transform getTransform();

    public ShootingType GetShootingType();
}