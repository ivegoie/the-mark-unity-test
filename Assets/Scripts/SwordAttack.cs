using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    [SerializeField] private float damage = 20f;
    private Collider swordCollider;

    void Awake()
    {
        swordCollider = GetComponent<Collider>();
        swordCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyHealthSystem enemyHealth = other.GetComponent<EnemyHealthSystem>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
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
