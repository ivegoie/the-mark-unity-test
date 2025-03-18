using UnityEngine;

public class EnemyHealthSystem : MonoBehaviour
{
    [SerializeField] float maxHealth = 100;
    [SerializeField] int gainExperience = 15;
    float currentHealth;
    Animator animator;
    bool isDead = false;

    void Awake()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        animator.SetTrigger("isDead_trig");
        GetComponent<Enemy>().enabled = false;
        GetComponent<Collider>().enabled = false;

        PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        if (player != null)
        {
            player.GainExperience(gainExperience);
        }


        Invoke(nameof(DisableEnemy), 10f);
    }

    void DisableEnemy()
    {
        gameObject.SetActive(false);
        isDead = false;
        currentHealth = maxHealth;

        GetComponent<Enemy>().enabled = true;
        GetComponent<Collider>().enabled = true;
    }
}
