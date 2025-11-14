using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Stats")]
    public float speed = 20f;
    public float lifeTime = 3f;
    public int damage = 10;

    [Header("Opcional: ¿Destruir al contacto con todo?")]
    public bool destroyOnAnyCollision = true;

    private void Start()
    {
        // Para que no se queden volando infinitamente
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // Mueve el proyectil hacia adelante
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Si toca al jugador → hacer daño
        if (other.CompareTag("Player"))
        {
            //PlayerHealth hp = other.GetComponent<PlayerHealth>();
           // if (hp != null)
            //{
            //    hp.TakeDamage(damage);
            //}
        }

        // Destruir el proyectil sin importar qué tocó
        Destroy(gameObject);
    }

}
