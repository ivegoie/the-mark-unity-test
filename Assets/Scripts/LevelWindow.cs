using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelWindow : MonoBehaviour
{
    TextMeshProUGUI levelText;
    TextMeshProUGUI experienceText;
    Image experienceBarImage;

    LevelSystem levelSystem;

    void Awake()
    {
        levelText = transform.Find("Experience Bar/Level/Level Int").GetComponent<TextMeshProUGUI>();
        experienceBarImage = transform.Find("Experience Bar/Bar").GetComponent<Image>();
        experienceText = transform.Find("Experience Bar/Experience Text").GetComponent<TextMeshProUGUI>();

        transform.Find("Buttons/add5").GetComponent<Button>().onClick.AddListener(() => levelSystem.AddExperience(5));
        transform.Find("Buttons/add50").GetComponent<Button>().onClick.AddListener(() => levelSystem.AddExperience(50));
        transform.Find("Buttons/add500").GetComponent<Button>().onClick.AddListener(() => levelSystem.AddExperience(500));
    }


    void SetExerienceBarSize(float experienceNormalized)
    {
        experienceBarImage.fillAmount = experienceNormalized;
        experienceText.text = levelSystem.GetCurrentExperience() + " / " + levelSystem.GetExperienceToNextLevel();
    }


    void SetLevelNumber(int levelNumber)
    {
        levelText.text = levelNumber.ToString();
    }


    public void SetLevelSystem(LevelSystem levelSystem)
    {
        this.levelSystem = levelSystem;
        SetLevelNumber(levelSystem.GetLevelNumber());
        SetExerienceBarSize(levelSystem.GetExperienceNormalized());


        levelSystem.OnExperienceChanged += LevelSystem_OnExperienceChanged;
        levelSystem.OnLevelChanged += LevelSystem_OnLevelChanged;
    }

    private void LevelSystem_OnLevelChanged(object sender, EventArgs e)
    {
        SetLevelNumber(levelSystem.GetLevelNumber());
    }

    private void LevelSystem_OnExperienceChanged(object sender, EventArgs e)
    {
        SetExerienceBarSize(levelSystem.GetExperienceNormalized());
    }
}
