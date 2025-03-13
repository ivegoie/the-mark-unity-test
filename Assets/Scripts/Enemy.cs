using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    NavMeshAgent navMeshAgent;
    [SerializeField] Transform target;
    [SerializeField] float attackDistance;

    float distance;


    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

    }

    void FixedUpdate()
    {
        distance = Vector3.Distance(navMeshAgent.transform.position, target.position);

        if (distance < attackDistance)
        {
            Debug.Log("ATTACKING");
        }
        else
        {
            navMeshAgent.destination = target.position;
        }
    }
}
