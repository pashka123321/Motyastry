using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Security.Cryptography;
using System.Text;

public class MenuController : MonoBehaviour
{
    [SerializeField] private InputField seedInputField;
    [SerializeField] private Button startButton;

    void Start()
    {
        startButton.onClick.AddListener(OnStartButtonClick);
    }

    void OnStartButtonClick()
    {
        if (!string.IsNullOrEmpty(seedInputField.text))
        {
            // Преобразуем текстовый сид в числовой с помощью хеширования SHA256
            int seed = GetSeedFromString(seedInputField.text);

            PlayerPrefs.SetInt("Seed", seed);
            PlayerPrefs.Save();
            SceneManager.LoadScene("Game"); // Замените "Game" на имя вашей основной сцены
        }
        else
        {
            int randomSeed = UnityEngine.Random.Range(0, 1000000);
            PlayerPrefs.SetInt("Seed", randomSeed);
            PlayerPrefs.Save();
            seedInputField.text = randomSeed.ToString();
            SceneManager.LoadScene("Game");
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
