using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] LevelWindow levelWindow;
    [SerializeField] PlayerController playerController;

    [Header("Object Pooling")]
    [SerializeField] List<GameObject> pooledObjects;
    [SerializeField] GameObject objectToPool;
    [SerializeField] int amountToPool;

    void Awake()
    {
        LevelSystem levelSystem = new LevelSystem();
        HealthSystem healthSystem = new HealthSystem(100, 2);

        levelWindow.SetLevelSystem(levelSystem);
        levelWindow.SetHealthSystem(healthSystem);

        playerController.SetLevelSystem(levelSystem);
        playerController.SetHealthSystem(healthSystem);
    }

    void Start()
    {
        pooledObjects = new List<GameObject>();
        GameObject temp;
        for (int i = 0; i < amountToPool; i++)
        {
            temp = Instantiate(objectToPool);
            temp.SetActive(false);
            pooledObjects.Add(temp);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        GameObject enemy = GetPooledObject();
        if (enemy != null)
        {
            Vector3 randomSpawnPosition = new Vector3(Random.Range(260, 290), 0f, Random.Range(360, 390));

            enemy.transform.position = randomSpawnPosition;
            enemy.SetActive(true);
        }
        else
        {
            Debug.Log("No available enemies in pool!");
        }
    }


    public GameObject GetPooledObject()
    {
        foreach (GameObject obj in pooledObjects)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }
        return null;
    }

}
