using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] float attackRange = 2f;
    [SerializeField] float attackCooldown = 1.5f;
    [SerializeField] int attackDamage = 10;

    private Transform player;
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private bool isAttacking = false;
    private float lastAttackTime = 0f;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > attackRange)
        {
            ChasePlayer();
        }
        else
        {
            AttackPlayer();
        }
    }

    void ChasePlayer()
    {
        if (isAttacking) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > attackRange)
        {
            if (navMeshAgent.isStopped)
            {
                navMeshAgent.isStopped = false;
            }

            navMeshAgent.SetDestination(player.position);

            if (!animator.GetBool("isRunning_b"))
            {
                animator.SetBool("isRunning_b", true);
            }
        }
        else
        {
            navMeshAgent.isStopped = true;

            if (animator.GetBool("isRunning_b"))
            {
                animator.SetBool("isRunning_b", false);
            }
        }
    }


    void AttackPlayer()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;

        isAttacking = true;
        lastAttackTime = Time.time;

        navMeshAgent.isStopped = true;

        animator.SetBool("isRunning_b", false);
        animator.SetTrigger("attack_trig");

        Invoke(nameof(DealDamage), 0.5f);
    }


    void DealDamage()
    {
        if (player != null && Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            PlayerController playerHealth = player.GetComponent<PlayerController>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
        }
        isAttacking = false;
    }
}
