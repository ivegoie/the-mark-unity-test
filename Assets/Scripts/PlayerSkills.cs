using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkills
{
    public event EventHandler<OnSkillUnlockedEventArgs> OnSkillUnlocked;
    public class OnSkillUnlockedEventArgs : EventArgs
    {
        public SkillType skillType;
    }
    public enum SkillType
    {
        Spinwild,
        Dash,
        Buff,
        HealthRegeneration,
        FasterAttackSpeed,
    }

    List<SkillType> unlockedSkillTypeList;
    int skillPoints;

    public PlayerSkills()
    {
        unlockedSkillTypeList = new List<SkillType>();
    }

    public void AddSkillPoint()
    {
        skillPoints++;
    }

    public int GetSkillPoints()
    {
        return skillPoints;
    }

    public void UnlockSkill(SkillType skillType)
    {
        if (skillPoints <= 0)
        {
            Debug.Log("Not enough skill points to unlock: " + skillType);
            return;
        }

        if (!IsSkillUnlocked(skillType))
        {
            unlockedSkillTypeList.Add(skillType);
            skillPoints--;
            Debug.Log("Unlocked skill: " + skillType);
            OnSkillUnlocked?.Invoke(this, new OnSkillUnlockedEventArgs { skillType = skillType });
        }

    }

    public bool IsSkillUnlocked(SkillType skillType)
    {
        return unlockedSkillTypeList.Contains(skillType);
    }
}
