using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] LevelWindow levelWindow;
    [SerializeField] PlayerController playerController;

    void Awake()
    {
        LevelSystem levelSystem = new LevelSystem();
        HealthSystem healthSystem = new HealthSystem(100, 2);

        levelWindow.SetLevelSystem(levelSystem);
        levelWindow.SetHealthSystem(healthSystem);

        playerController.SetLevelSystem(levelSystem);
        playerController.SetHealthSystem(healthSystem);
    }
}
