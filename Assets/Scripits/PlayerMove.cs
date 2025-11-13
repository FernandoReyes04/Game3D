using UnityEngine;
using UnityEngine.InputSystem; // Necesario para el nuevo Input System

public class PlayerMove : MonoBehaviour
{
    [Header("Movement Settings")]
    public float runSpeed = 7f;
    public float rotationSpeed = 10f;

    [Header("Jump Settings")]
    public float jumpForce = 8f;
    public float gravity = -20f;

    // ** NUEVO: Referencia al gestor de armas **
    [Header("Weapon System")]
    public WeaponManager weaponManager;

    [Header("References")]
    public Animator animator;
    public Transform cameraTransform;
    public Transform attackPoint;

    private CharacterController controller;
    private Vector2 moveInput;
    private Vector3 velocity;
    private bool isGrounded;
    // ** Las variables de combate han sido eliminadas **

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        // ** Buscamos o requerimos el WeaponManager **
        if (weaponManager == null)
        {
            weaponManager = GetComponent<WeaponManager>();
            if (weaponManager == null)
            {
                Debug.LogError("PlayerMove requiere un WeaponManager. Por favor, añádelo al GameObject.");
            }
        }
    }

    // Método llamado por el Input System para moverse
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    // Método llamado por el Input System para saltar
    public void OnJump(InputValue value)
    {
        // Nota: Si quieres que el ataque bloquee el salto, el WeaponManager
        // debe tener una propiedad pública (ej: IsAttacking) para que la uses aquí.
        if (isGrounded && value.isPressed)
        {
            velocity.y = jumpForce;

            if (animator != null)
            {
                animator.SetTrigger("Jump");
            }
        }
    }

    // Método llamado por el Input System para atacar
    public void OnAttack(InputValue value)
    {
        if (value.isPressed && weaponManager != null)
        {
            // ** SOLO DELEGAMOS LA ORDEN DE ATAQUE AL GESTOR **
            weaponManager.Attack();
        }
    }

    // ** NUEVO: Método para cambiar de arma (ejemplo para el Input System) **
    public void OnEquipWeapon1(InputValue value)
    {
        if (value.isPressed && weaponManager != null)
        {
            weaponManager.EquipWeapon(0); // Índice 0: La espada/arma inicial
            Debug.Log(" EQUIPANDO: Arma 1 (Índice 0)");
        }
    }

    // Método que se conecta a la tecla '2'
    public void OnEquipWeapon2(InputValue value)
    {
        if (value.isPressed && weaponManager != null)
        {
            weaponManager.EquipWeapon(1); // Índice 1: El hacha
            Debug.Log(" EQUIPANDO: Arma 2 (Índice 1)");
        }
    }


    void Update()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Ya no comprobamos isAttacking aquí. Si el personaje se detiene al atacar,
        // la animación de ataque tiene que ser más prioritaria que el movimiento.
        if (moveInput.magnitude > 0.1f)
        {
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();

            Vector3 moveDirection = (forward * moveInput.y + right * moveInput.x).normalized;

            if (moveDirection.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            controller.Move(moveDirection * runSpeed * Time.deltaTime);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (animator != null)
        {
            animator.SetFloat("VelX", moveInput.x);
            animator.SetFloat("VelY", moveInput.y);
            // ** animator.SetBool("IsAttacking", isAttacking); <--- ELIMINADO **

            // Si quieres que el Animator sepa si ataca, la variable 'IsAttacking' 
            // debe ser enviada por el script MeleeWeapon.
        }
    }

    // ** Todos los métodos de combate (StartAttack, DealDamage, FinishAttack, ResetCombo, OnDrawGizmosSelected) **
    // ** HAN SIDO ELIMINADOS DE AQUÍ. ¡Ahora viven en MeleeWeapon.cs! **
}
