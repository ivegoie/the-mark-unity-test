using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] LevelWindow levelWindow;
    [SerializeField] PlayerController playerController;
    [SerializeField] SkillTree skillTree;

    void Start()
    {
        LevelSystem levelSystem = new LevelSystem();
        HealthSystem healthSystem = new HealthSystem(100, 2);

        levelWindow.SetLevelSystem(levelSystem);
        levelWindow.SetHealthSystem(healthSystem);

        playerController.SetLevelSystem(levelSystem);
        playerController.SetHealthSystem(healthSystem);

        PlayerSkills playerSkills = playerController.GetPlayerSkills();
        skillTree.SetPlayerSkills(playerSkills);

        playerSkills.OnSkillUnlocked += (sender, e) => skillTree.UpdateSkillTreeUI();

        skillTree.UpdateSkillTreeUI();
    }
}
