using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Detection Settings")]
    public float detectionRange = 10f;
    public float attackRange = 2.5f;
    public LayerMask playerLayer;

    [Header("Movement Settings")]
    public float walkSpeed = 2f;
    public float runSpeed = 4f;

    [Header("Attack Settings")]
    public float attackCooldown = 2f;
    public int attackDamage = 10;

    [Header("References")]
    public Animator animator;
    public Transform player;

    private NavMeshAgent agent;
    private float lastAttackTime;
    private bool isAttacking;
    private EnemyState currentState = EnemyState.Idle;

    private enum EnemyState
    {
        Idle,
        Patrol,
        Chase,
        Attack
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        if (animator == null)
            animator = GetComponent<Animator>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        if (agent != null)
        {
            agent.speed = walkSpeed;
            agent.stoppingDistance = attackRange;
        }
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case EnemyState.Idle:
                HandleIdleState(distanceToPlayer);
                break;
            case EnemyState.Chase:
                HandleChaseState(distanceToPlayer);
                break;
            case EnemyState.Attack:
                HandleAttackState(distanceToPlayer);
                break;
        }

        UpdateAnimations();
    }

    void HandleIdleState(float distanceToPlayer)
    {
        if (distanceToPlayer <= detectionRange)
        {
            currentState = EnemyState.Chase;
            agent.speed = runSpeed;
        }
    }

    void HandleChaseState(float distanceToPlayer)
    {
        if (distanceToPlayer > detectionRange * 1.5f)
        {
            currentState = EnemyState.Idle;
            agent.ResetPath();
            return;
        }

        if (distanceToPlayer <= attackRange)
        {
            currentState = EnemyState.Attack;
            agent.ResetPath();
            return;
        }

        agent.SetDestination(player.position);
        
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void HandleAttackState(float distanceToPlayer)
    {
        if (distanceToPlayer > attackRange * 1.2f)
        {
            currentState = EnemyState.Chase;
            agent.speed = runSpeed;
            return;
        }

        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);

        if (Time.time >= lastAttackTime + attackCooldown && !isAttacking)
        {
            StartAttack();
        }
    }

    void StartAttack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        Invoke("FinishAttack", 1f);
    }

    void FinishAttack()
    {
        isAttacking = false;
    }

    void UpdateAnimations()
    {
        if (animator == null) return;

        float speed = agent.velocity.magnitude;
        animator.SetFloat("Speed", speed);

        bool isMoving = speed > 0.1f;
        animator.SetBool("IsMoving", isMoving);
    }

    void FootL()
    {
        // Animation Event para pie izquierdo
        // Aquí puedes agregar sonidos de pasos si quieres
    }

    void FootR()
    {
        // Animation Event para pie derecho
        // Aquí puedes agregar sonidos de pasos si quieres
    }

    void Hit()
    {
        // Animation Event para impacto de ataque
        // Aquí puedes agregar lógica de daño si quieres
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
