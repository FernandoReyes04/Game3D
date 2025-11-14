using UnityEngine;

public class EnemyRanged : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;
    public Transform firePoint;
    public GameObject projectilePrefab;
    private EnemyHealth health; // ⭐ Para detectar muerte

    [Header("Stats")]
    public float detectionRange = 15f;
    public float escapeRange = 5f;
    public float moveSpeed = 4f;
    public float fireRate = 1.5f;

    private float nextFireTime = 0f;
    private bool isDead = false;

    private Rigidbody rb;

    void Start()
    {
        health = GetComponent<EnemyHealth>();
        rb = GetComponent<Rigidbody>();

        if (rb != null)
            rb.freezeRotation = true; // evita volar o girar raro

        if (health != null)
            health.onDeath += OnDeath; // ⭐ detener todo al morir
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // 1) Huir si está muy cerca
        if (distance < escapeRange)
        {
            EscapeFromPlayer();
            return;
        }

        // 2) Atacar si está dentro del rango
        if (distance <= detectionRange)
        {
            LookAtPlayer();
            TryShoot();
        }
    }

    void EscapeFromPlayer()
    {
        Vector3 dir = (transform.position - player.position).normalized;

        // ⭐ Mantenerse en el piso
        dir.y = 0;

        transform.position += dir * moveSpeed * Time.deltaTime;

        LookAwayFromPlayer();
    }

    void LookAtPlayer()
    {
        Vector3 lookDir = player.position - transform.position;
        lookDir.y = 0;
        transform.rotation = Quaternion.LookRotation(lookDir);
    }

    void LookAwayFromPlayer()
    {
        Vector3 lookDir = transform.position - player.position;
        lookDir.y = 0;
        transform.rotation = Quaternion.LookRotation(lookDir);
    }

    void TryShoot()
    {
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
    }

    // ⭐ Cuando se muere
    void OnDeath()
    {
        isDead = true;

        // desactivar collider y animaciones de movimiento
        if (rb != null)
            rb.isKinematic = true;

        enabled = false; // Detiene Update
    }
}
