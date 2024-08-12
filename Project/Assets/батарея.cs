using UnityEngine;

public class Battery : MonoBehaviour
{
    public float maxEnergy = 100f; // Максимальная емкость батареи
    public float currentEnergy = 0f; // Текущий уровень энергии в батарее

    public void StoreEnergy(float amount)
    {
        currentEnergy += amount;
        if (currentEnergy > maxEnergy)
        {
            currentEnergy = maxEnergy; // Ограничение по максимальной емкости
        }
    }

    public float GetEnergy(float amount)
    {
        float energyProvided = Mathf.Min(amount, currentEnergy);
        currentEnergy -= energyProvided;
        return energyProvided;
    }

    public float GetCurrentEnergy()
    {
        return currentEnergy;
    }
}
