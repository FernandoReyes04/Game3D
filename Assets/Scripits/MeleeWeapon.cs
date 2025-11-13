using UnityEngine;
using System.Collections.Generic;

public class MeleeWeapon : Weapon
{
    public LayerMask EnemyLayer;
    public Animator PlayerAnimator; // Se mantiene, pero solo para disparar la animación
    public float AttackDuration = 0.5f; // Duración en segundos que la hitbox estará activa

    private bool isAttacking = false;
    private int attackComboCount = 0;

    [Header("Hitbox")]
    public BoxCollider damageCollider;
    private List<GameObject> hitTargets = new List<GameObject>();

    void Start()
    {
        // Aseguramos que el Collider esté asignado y APAGADO al inicio.
        if (damageCollider == null) damageCollider = GetComponent<BoxCollider>();
        if (damageCollider != null)
        {
            damageCollider.isTrigger = true;
            damageCollider.enabled = false; // ¡APAGADO! Solo se enciende al atacar.
        }
    }

    public override void Attack()
    {
        if (isAttacking) return;

        if (CanAttack())
        {
            nextFireTime = Time.time + FireRate;
            StartAttackSequence();
        }
    }

    void StartAttackSequence()
    {
        isAttacking = true;
        hitTargets.Clear();

        // Lógica de combo (animación)
        attackComboCount++;
        if (attackComboCount > 3) attackComboCount = 1;
        if (PlayerAnimator != null)
        {
            PlayerAnimator.SetTrigger("Attack");
            PlayerAnimator.SetInteger("AttackCombo", attackComboCount);
        }

        // 🔥 ACTIVA Y DESACTIVA LA HITBOX POR TIEMPO 🔥
        if (damageCollider != null)
        {
            damageCollider.enabled = true; // Activa el Collider
            Debug.Log("Hitbox ACTIVADA por temporizador!");
        }

        // 1. Invoca la desactivación del Collider
        Invoke("DeactivateHitbox", AttackDuration);
        // 2. Invoca la finalización de la lógica del ataque (permite otro ataque)
        Invoke("FinishAttack", AttackDuration);
    }

    public void DeactivateHitbox()
    {
        if (damageCollider != null)
        {
            damageCollider.enabled = false; // Desactiva el Collider
            Debug.Log("Hitbox DESACTIVADA por temporizador!");
        }
    }

    // --- DETECCIÓN DE DAÑO (Se mantiene igual) ---

    private void OnTriggerEnter(Collider other)
    {
        // ... [Tu lógica de OnTriggerEnter permanece igual]
        if (((1 << other.gameObject.layer) & EnemyLayer.value) != 0)
        {
            if (hitTargets.Contains(other.gameObject)) return;

            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(Damage);
                hitTargets.Add(other.gameObject);
                Debug.Log($"¡Daño aplicado a {other.gameObject.name}!");
            }
        }
    }

    // --- LÓGICA DE FIN DE ATAQUE (Se mantiene igual) ---
    void FinishAttack()
    {
        isAttacking = false;
        Invoke("ResetCombo", 1f);
    }

    void ResetCombo()
    {
        if (!isAttacking)
        {
            attackComboCount = 0;
        }
    }
}