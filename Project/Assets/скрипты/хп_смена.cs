using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider; // Ссылка на компонент Slider вашей полосы здоровья
    public Image fillImage; // Ссылка на компонент Image, который будет отображать спрайты в зависимости от уровня здоровья

    public Sprite fullHealthSprite; // Спрайт для 100% здоровья
    public Sprite seventyFiveHealthSprite; // Спрайт для 75% здоровья
    public Sprite fiftyHealthSprite; // Спрайт для 50% здоровья
    public Sprite twentyFiveHealthSprite; // Спрайт для 25% здоровья
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
        if (healthPercentage >= 75f)
        {
            fillImage.sprite = fullHealthSprite;
        }
        else if (healthPercentage >= 50f)
        {
            fillImage.sprite = seventyFiveHealthSprite;
        }
        else if (healthPercentage >= 25f)
        {
            fillImage.sprite = fiftyHealthSprite;
        }
        else if (healthPercentage > 0f)
        {
            fillImage.sprite = twentyFiveHealthSprite;
        }
        else
        {
            fillImage.sprite = zeroHealthSprite;
        }
    }
}
