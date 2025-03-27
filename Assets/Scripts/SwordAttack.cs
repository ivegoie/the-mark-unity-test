using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    private Collider swordCollider;
    private PlayerController playerController;

    void Awake()
    {
        swordCollider = GetComponent<Collider>();
        swordCollider.enabled = false;
        playerController = FindAnyObjectByType<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyHealthSystem enemyHealth = other.GetComponent<EnemyHealthSystem>();
            if (enemyHealth != null)
            {
                float damage = playerController.GetAttackDamage();
                enemyHealth.TakeDamage(damage);
                Debug.Log($"Sword hit {other.name}, dealing {damage} damage!");
            }
        }
    }

    public void EnableCollider()
    {
        swordCollider.enabled = true;
    }

    public void DisableCollider()
    {
        swordCollider.enabled = false;
    }
}
