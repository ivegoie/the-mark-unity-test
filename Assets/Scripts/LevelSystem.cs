using System;

public class LevelSystem
{

    public event EventHandler OnExperienceChanged;
    public event EventHandler OnLevelChanged;

    int level;
    int experience;
    int experienceToNextLevel;


    public LevelSystem()
    {
        level = 1;
        experience = 0;
        experienceToNextLevel = 20;
    }


    public void AddExperience(int amount)
    {
        experience += amount;

        while (experience >= experienceToNextLevel)
        {
            experience -= experienceToNextLevel;
            level++;
            experienceToNextLevel = level * level * 20;

            OnLevelChanged?.Invoke(this, EventArgs.Empty);
        }

        OnExperienceChanged?.Invoke(this, EventArgs.Empty);
    }


    public int GetLevelNumber()
    {
        return level;
    }

    public float GetExperienceNormalized()
    {
        return (float)experience / experienceToNextLevel;
    }

    public int GetCurrentExperience()
    {
        return experience;
    }

    public int GetExperienceToNextLevel()
    {
        return experienceToNextLevel;
    }

}
