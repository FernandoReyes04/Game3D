using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [Header("Movement Settings")]
    public float runSpeed = 7f;
    public float rotationSpeed = 10f;

    [Header("Jump Settings")]
    public float jumpForce = 8f;
    public float gravity = -20f;

    [Header("Combat Settings")]
    public float attackRange = 2.5f;
    public int attackDamage = 100;
    public float attackCooldown = 0.5f;
    public LayerMask enemyLayer;

    [Header("References")]
    public Animator animator;
    public Transform cameraTransform;
    public Transform attackPoint;

    private CharacterController controller;
    private Vector2 moveInput;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isAttacking;
    private float lastAttackTime;
    private int attackComboCount = 0;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
        
        if (animator == null)
            animator = GetComponent<Animator>();
        
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        if (attackPoint == null)
        {
            GameObject attackPointObj = new GameObject("AttackPoint");
            attackPointObj.transform.SetParent(transform);
            attackPointObj.transform.localPosition = new Vector3(0, 1.5f, 1.5f);
            attackPoint = attackPointObj.transform;
            Debug.Log("AttackPoint created!");
        }
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (isGrounded && value.isPressed && !isAttacking)
        {
            velocity.y = jumpForce;
            
            if (animator != null)
            {
                animator.SetTrigger("Jump");
            }
        }
    }

    public void OnAttack(InputValue value)
    {
        if (value.isPressed && !isAttacking && Time.time >= lastAttackTime + attackCooldown)
        {
            StartAttack();
        }
    }

    void Update()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (!isAttacking && moveInput.magnitude > 0.1f)
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
            animator.SetBool("IsAttacking", isAttacking);
        }
    }

    void StartAttack()
    {
        Debug.Log("*** ATTACK STARTED ***");
        
        isAttacking = true;
        lastAttackTime = Time.time;
        attackComboCount++;

        if (attackComboCount > 3)
            attackComboCount = 1;

        if (animator != null)
        {
            animator.SetTrigger("Attack");
            animator.SetInteger("AttackCombo", attackComboCount);
        }

        Invoke("DealDamage", 1.5f);
        Invoke("FinishAttack", 0.6f);
    }

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

    public void DealDamage()
    {
        Debug.Log("=== DEAL DAMAGE ===");
        
        if (attackPoint == null)
        {
            Debug.LogError("AttackPoint is NULL!");
            return;
        }

        Debug.Log("AttackPoint Position: " + attackPoint.position);
        Debug.Log("Attack Range: " + attackRange);
        Debug.Log("Enemy Layer Mask: " + enemyLayer.value);

        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);
        Debug.Log("Enemies found: " + hitEnemies.Length);

        foreach (Collider enemy in hitEnemies)
        {
            Debug.Log("Found collider: " + enemy.name + " on layer " + LayerMask.LayerToName(enemy.gameObject.layer));
            
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
                Debug.Log("DAMAGE DEALT to: " + enemy.name);
            }
            else
            {
                Debug.LogWarning("No EnemyHealth on: " + enemy.name);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}
