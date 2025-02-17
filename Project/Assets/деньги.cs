using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MoneyManager : MonoBehaviour
{
    public int money = 5000; // Начальная сумма
    public Text moneyText;   // Ссылка на UI-элемент для отображения денег
    public int passiveIncome = 10; // Пассивный доход
    private float incomeInterval = 1.0f; // Интервал в секундах
    private float nextIncomeTime = 0f;

    void Start()
    {
        UpdateMoneyText(); // Обновляем текст при старте игры
        nextIncomeTime = Time.time + incomeInterval;
    }

    void Update()
    {
        if (Time.time >= nextIncomeTime)
        {
            AddMoney(passiveIncome);
            nextIncomeTime += incomeInterval;
        }
    }

    public void AddMoney(int amount)
    {
        money += amount; // Добавляем деньги
        UpdateMoneyText();
    }

    public void SubtractMoney(int amount)
    {
        if (money >= amount)
        {
            money -= amount; // Вычитаем деньги, если хватает
            UpdateMoneyText();
        }
        // Если денег не хватает, просто ничего не делаем
    }

    private void UpdateMoneyText()
    {
        if (moneyText != null)
        {
            moneyText.text = "$" + money.ToString(); // Обновляем текстовое отображение
        }
        // Убрали Debug.LogError, чтобы не спамило ошибками в консоли
    }

    public bool IsRTSMode()
    {
        return SceneManager.GetActiveScene().name == "RTS";
    }
}
