using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider; // Ссылка на компонент Slider вашей полосы здоровья
    public Image fillImage; // Ссылка на компонент Image, который будет отображать спрайты в зависимости от уровня здоровья

    public Sprite fullHealthSprite; // Спрайт для 100% здоровья
    public Sprite eightyHealthSprite; // Спрайт для 80% здоровья
    public Sprite sixtyHealthSprite; // Спрайт для 60% здоровья
    public Sprite fortyHealthSprite; // Спрайт для 40% здоровья
    public Sprite twentyHealthSprite; // Спрайт для 20% здоровья
    public Sprite zeroHealthSprite; // Спрайт для 0% здоровья

    public void SetHealth(float health, float maxHealth)
    {
        slider.value = health / maxHealth; // Устанавливаем значение Slider в проценты от текущего здоровья
        UpdateHealthSprite(health, maxHealth); // Обновляем спрайт в зависимости от текущего здоровья
    }

    private void UpdateHealthSprite(float health, float maxHealth)
    {
        float healthPercentage = health / maxHealth * 100f;

        // Выбираем спрайт в зависимости от процента здоровья
        if (healthPercentage >= 80f)
        {
            fillImage.sprite = fullHealthSprite;
        }
        else if (healthPercentage >= 60f)
        {
            fillImage.sprite = eightyHealthSprite;
        }
        else if (healthPercentage >= 40f)
        {
            fillImage.sprite = sixtyHealthSprite;
        }
        else if (healthPercentage >= 20f)
        {
            fillImage.sprite = fortyHealthSprite;
        }
        else if (healthPercentage > 0f)
        {
            fillImage.sprite = twentyHealthSprite;
        }
        else
        {
            fillImage.sprite = zeroHealthSprite;
        }
    }
}
