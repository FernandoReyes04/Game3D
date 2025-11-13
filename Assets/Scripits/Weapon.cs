using UnityEngine;

// La palabra 'abstract' significa que es una plantilla y no se usa directamente.
public abstract class Weapon : MonoBehaviour
{
    [Header("Weapon Stats")]
    public float FireRate = 0.5f; // Será tu attackCooldown
    public int Damage = 100;      // Será tu attackDamage
    public float Range = 2.5f;    // Será tu attackRange
    public Transform FirePoint;   // Será tu attackPoint

    protected float nextFireTime;

    // Todas las armas deben tener un método Attack()
    public abstract void Attack();

    // Revisa si ya pasó el cooldown
    public bool CanAttack()
    {
        // Si el tiempo actual es mayor al tiempo que podemos volver a disparar...
        return Time.time >= nextFireTime;
    }
}