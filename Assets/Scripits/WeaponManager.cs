using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    [Header("References")]
    public Animator playerAnimator;
    public Transform WeaponMountPoint; // Punto donde el arma se une al modelo del jugador

    [Header("Current Weapons")]
    // **IMPORTANTE:** Aquí se asignan las PREFABS iniciales en el Inspector.
    public List<Weapon> initialWeaponPrefabs = new List<Weapon>();

    // Lista que contendrá las instancias de armas (las que están en la escena)
    private List<Weapon> ownedWeapons = new List<Weapon>();

    private Weapon currentWeapon;
    private int currentWeaponIndex = -1; // Usamos -1 para indicar que no hay nada equipado al inicio

    //-------------------------------------------------------------------------

    void Start()
    {
        // Asegúrate de que el Animator del jugador esté aquí
        if (playerAnimator == null)
            playerAnimator = GetComponent<Animator>();

        // 1. INSTANCIAR las armas asignadas en el Inspector (si las hay)
        foreach (Weapon prefab in initialWeaponPrefabs)
        {
            // Usamos AddWeapon para gestionar la instanciación y el montaje
            AddWeapon(prefab, false); // No equipamos inmediatamente, solo las añadimos
        }

        // 2. EQUIPAR la primera arma instanciada si la lista no está vacía
        if (ownedWeapons.Count > 0)
        {
            EquipWeapon(0);
        }
    }

    void Update()
    {
        // 3. Lógica para cambiar de arma con las teclas numéricas (1, 2, 3, etc.)
        for (int i = 0; i < ownedWeapons.Count; i++)
        {
            // KeyCode.Alpha1 es la tecla '1', Alpha2 es '2', etc.
            // Si el jugador presiona la tecla que corresponde al índice (i + 1)
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                EquipWeapon(i);
                // Salimos del bucle una vez que se procesa la entrada para evitar chequeos innecesarios
                break;
            }
        }
    }

    //-------------------------------------------------------------------------

    // El método que PlayerMove llamará cuando pulses el botón de ataque
    public void Attack()
    {
        if (currentWeapon != null)
        {
            currentWeapon.Attack();
        }
        else
        {
            Debug.LogWarning("¡Intento de ataque fallido! No hay arma equipada.");
        }
    }

    // Método para añadir un arma al inventario (ej. al recogerla)
    public void AddWeapon(Weapon newWeaponPrefab, bool equipImmediately = true)
    {
        // 1. **DETERMINAR PADRE:** Si el WeaponMountPoint está asignado, úsalo. Si no, usa el propio jugador (transform).
        Transform parent = (WeaponMountPoint != null) ? WeaponMountPoint : transform;

        // 2. INSTANCIAR: Se crea el objeto como hijo del 'parent'
        Weapon newWeaponInstance = Instantiate(newWeaponPrefab, parent);
        newWeaponInstance.gameObject.SetActive(false);

        // 3. Añadir a la lista de instancias
        ownedWeapons.Add(newWeaponInstance);

        // 4. Equipar inmediatamente si es necesario
        if (equipImmediately)
        {
            EquipWeapon(ownedWeapons.Count - 1);
        }
    }

    // Función para cambiar de arma (ej: con las teclas 1/2)
    public void EquipWeapon(int index)
    {
        if (index >= 0 && index < ownedWeapons.Count)
        {
            // Si el arma ya está equipada, no hacemos nada
            if (index == currentWeaponIndex)
            {
                Debug.Log($"El arma {ownedWeapons[index].gameObject.name} ya está equipada.");
                return;
            }

            // ⭐ APAGAR cualquier arma física dentro del mount point (incluye tu arma inicial)
            if (WeaponMountPoint != null)
            {
                foreach (Transform child in WeaponMountPoint)
                {
                    child.gameObject.SetActive(false);
                }
            }

            // 1. Desactivar el arma anterior instanciada
            if (currentWeapon != null)
            {
                currentWeapon.gameObject.SetActive(false);
            }

            // 2. Activar la nueva
            currentWeapon = ownedWeapons[index];
            currentWeapon.gameObject.SetActive(true);
            currentWeaponIndex = index;

            // 3. Asignar el Animator del jugador al arma Melee si lo necesita
            if (currentWeapon is MeleeWeapon melee)
            {
                melee.PlayerAnimator = playerAnimator;
            }

            Debug.Log($"EQUIPANDO ARMA: {currentWeapon.gameObject.name} (Índice {index + 1})");
        }
        else
        {
            Debug.LogError($"Índice de arma {index} fuera de rango. Solo tienes {ownedWeapons.Count} armas.");
        }
    }

}