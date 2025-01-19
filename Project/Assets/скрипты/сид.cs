using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Security.Cryptography;
using System.Text;

public class MenuController : MonoBehaviour
{
    [SerializeField] private InputField seedInputField; // Поле для ввода сида
    [SerializeField] private Button startButton; // Кнопка "Старт"
    [SerializeField] private Dropdown gameModeDropdown; // Dropdown для выбора режима игры

    void Start()
    {
        startButton.onClick.AddListener(OnStartButtonClick);
    }

    void OnStartButtonClick()
    {
        if (gameModeDropdown == null)
        {
            Debug.LogError("Dropdown is not assigned!");
            return;
        }

        // Получаем выбранный режим игры из Dropdown
        string selectedGameMode = gameModeDropdown.options[gameModeDropdown.value].text;

        if (!string.IsNullOrEmpty(seedInputField.text))
        {
            // Преобразуем текстовый сид в числовой с помощью хеширования SHA256
            int seed = GetSeedFromString(seedInputField.text);

            PlayerPrefs.SetInt("Seed", seed);
            PlayerPrefs.Save();

            // Загружаем сцену в зависимости от выбора
            LoadSceneBasedOnGameMode(selectedGameMode);
        }
        else
        {
            int randomSeed = UnityEngine.Random.Range(0, 1000000);
            PlayerPrefs.SetInt("Seed", randomSeed);
            PlayerPrefs.Save();
            seedInputField.text = randomSeed.ToString();

            // Загружаем сцену в зависимости от выбора
            LoadSceneBasedOnGameMode(selectedGameMode);
        }
    }

    void LoadSceneBasedOnGameMode(string gameMode)
    {
        if (gameMode == "Классика")
        {
            SceneManager.LoadScene("Game"); // Замените "Game" на вашу сцену для "Классика"
        }
        else if (gameMode == "RTS")
        {
            SceneManager.LoadScene("RTS"); // Замените "RTS" на вашу сцену для "RTS"
        }
        else
        {
            Debug.LogError($"Unknown game mode: {gameMode}");
        }
    }

    // Метод для преобразования текстового сида в числовой с помощью SHA256
    int GetSeedFromString(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = sha256.ComputeHash(inputBytes);

            // Преобразуем хеш в положительное целое число
            int seed = BitConverter.ToInt32(hashBytes, 0);
            return Mathf.Abs(seed); // Возьмем абсолютное значение для положительного числа
        }
    }
}
