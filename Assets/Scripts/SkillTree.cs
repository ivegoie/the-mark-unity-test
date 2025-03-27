using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SkillTree : MonoBehaviour
{
    PlayerSkills playerSkills;
    Dictionary<Button, PlayerSkills.SkillType> skillButtons = new Dictionary<Button, PlayerSkills.SkillType>();

    void Awake()
    {
        AssignSkillButton("Skill_1", PlayerSkills.SkillType.Spinwild);
        AssignSkillButton("Skill_2", PlayerSkills.SkillType.Dash);
        AssignSkillButton("Skill_3", PlayerSkills.SkillType.Buff);
        AssignSkillButton("Skill_4", PlayerSkills.SkillType.HealthRegeneration);
        AssignSkillButton("Skill_5", PlayerSkills.SkillType.FasterAttackSpeed);
    }

    void AssignSkillButton(string skillButtonName, PlayerSkills.SkillType skillType)
    {
        Button skillButton = transform.Find(skillButtonName)?.GetComponent<Button>();

        if (skillButton != null)
        {
            skillButtons.Add(skillButton, skillType);
            skillButton.onClick.AddListener(() => UnlockSkill(skillType));
        }
        else
        {
            Debug.LogWarning($"Skill button '{skillButtonName}' not found!");
        }
    }

    void UnlockSkill(PlayerSkills.SkillType skillType)
    {
        if (playerSkills != null)
        {
            playerSkills.UnlockSkill(skillType);
            UpdateSkillTreeUI();
        }
    }

    public void SetPlayerSkills(PlayerSkills playerSkills)
    {
        this.playerSkills = playerSkills;
    }

    public void UpdateSkillTreeUI()
    {
        foreach (var entry in skillButtons)
        {
            Button button = entry.Key;
            PlayerSkills.SkillType skillType = entry.Value;

            bool isUnlocked = playerSkills.IsSkillUnlocked(skillType);
            button.interactable = !isUnlocked;
        }
    }
}
