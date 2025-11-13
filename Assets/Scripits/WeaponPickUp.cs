using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [Tooltip("El Prefab del arma (el que tiene el script Weapon) que se le dará al jugador.")]
    public Weapon WeaponPrefabToGive;

    // Se llama automáticamente cuando otro Collider (marcado como Is Trigger) entra.
    private void OnTriggerEnter(Collider other)
    {
        // 1. Verificar que quien colisionó sea el jugador (debe tener el Tag "Player")
        if (other.CompareTag("Player"))
        {
            // 2. Intentar obtener el WeaponManager del jugador
            WeaponManager wm = other.GetComponent<WeaponManager>();

            if (wm != null && WeaponPrefabToGive != null)
            {
                // 3. Llamar al método en el WeaponManager para añadir y equipar el arma
                wm.AddWeapon(WeaponPrefabToGive);

                // 4. Destruir el objeto de recogida del mundo
                Destroy(gameObject);
            }
        }
    }
}