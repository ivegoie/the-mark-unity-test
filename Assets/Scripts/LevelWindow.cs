using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelWindow : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI experienceText;
    [SerializeField] Image experienceBarImage;
    [SerializeField] Image healthBarImage;
    [SerializeField] TextMeshProUGUI healthText;

    LevelSystem levelSystem;
    HealthSystem healthSystem;

    void Awake()
    {
        levelText = transform.Find("Experience Bar/Level/Level Int")?.GetComponent<TextMeshProUGUI>();
        experienceBarImage = transform.Find("Experience Bar/Bar")?.GetComponent<Image>();
        experienceText = transform.Find("Experience Bar/Experience Text")?.GetComponent<TextMeshProUGUI>();
        healthBarImage = transform.Find("Health Bar/Bar")?.GetComponent<Image>();
        healthText = transform.Find("Health Bar/Health Text")?.GetComponent<TextMeshProUGUI>();


        if (levelText == null) Debug.LogError("LevelWindow: levelText is NULL. Check UI hierarchy!");
        if (experienceBarImage == null) Debug.LogError("LevelWindow: experienceBarImage is NULL. Check UI hierarchy!");
        if (experienceText == null) Debug.LogError("LevelWindow: experienceText is NULL. Check UI hierarchy!");
        if (healthBarImage == null) Debug.LogError("LevelWindow: healthBarImage is NULL. Check UI hierarchy!");
        if (healthText == null) Debug.LogError("LevelWindow: healthText is NULL. Check UI hierarchy!");


        if (transform.Find("Buttons/add5") != null)
        {
            transform.Find("Buttons/add5").GetComponent<Button>().onClick.AddListener(() => levelSystem?.AddExperience(5));
            transform.Find("Buttons/add50").GetComponent<Button>().onClick.AddListener(() => levelSystem?.AddExperience(50));
            transform.Find("Buttons/add500").GetComponent<Button>().onClick.AddListener(() => levelSystem?.AddExperience(500));
        }
        else
        {
            Debug.LogError("LevelWindow: Buttons not found in UI! Check your hierarchy.");
        }
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

    public void SetHealthSystem(HealthSystem healthSystem)
    {
        this.healthSystem = healthSystem;
        UpdateHealthUI();

        healthSystem.OnHealthChanged += HealthSystem_OnHealthChanged;
    }

    private void HealthSystem_OnHealthChanged(object sender, System.EventArgs e)
    {
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        healthBarImage.fillAmount = healthSystem.GetHealthNormalized();
        healthText.text = healthSystem.GetCurrentHealth() + " / " + healthSystem.GetMaxHealth();
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
