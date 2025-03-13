using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthWindow : MonoBehaviour
{
    Image healthBarImage;
    TextMeshProUGUI healthText;

    private HealthSystem healthSystem;

    void Awake()
    {
        healthBarImage = transform.Find("Health Bar/Bar").GetComponent<Image>();
        healthText = transform.Find("Health Bar/Health Text").GetComponent<TextMeshProUGUI>();
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
        if (healthSystem == null) return;
        healthBarImage.fillAmount = healthSystem.GetHealthNormalized();
        healthText.text = healthSystem.GetCurrentHealth() + " / " + healthSystem.GetMaxHealth();
    }
}
