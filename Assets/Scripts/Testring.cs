using UnityEngine;

public class Testring : MonoBehaviour
{
    [SerializeField] LevelWindow levelWindow;
    [SerializeField] PlayerController playerController;
    void Awake()
    {
        LevelSystem levelSystem = new LevelSystem();
        levelWindow.SetLevelSystem(levelSystem);
        playerController.SetLevelSystem(levelSystem);
    }
}
