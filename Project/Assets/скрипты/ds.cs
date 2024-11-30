using UnityEngine;
using Discord; // Убедитесь, что у вас подключена библиотека Discord SDK

public class DiscordController : MonoBehaviour
{
    public static DiscordController instance; // Singleton для сохранения между сценами

    public Discord.Discord discord;

    [Header("Game Settings")]
    public bool isSoloMode = true; // Галочка в инспекторе для выбора соло-игры

    void Awake()
    {
        // Singleton-паттерн: проверка, существует ли объект
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Оставляем объект при переходе между сценами
            InitializeDiscord();
        }
        else
        {
            Destroy(gameObject); // Удаление дубликатов
        }
    }

    // Инициализация Discord SDK
    void InitializeDiscord()
    {
        // Инициализация Discord с Client ID вашего приложения из Discord Developer Portal
        discord = new Discord.Discord(1253649558169325671, (ulong)Discord.CreateFlags.Default);

        Debug.Log("Discord SDK инициализирован!");
        UpdatePresence();
    }

    // Метод для обновления активности в Discord
    public void UpdatePresence()
    {
        var activityManager = discord.GetActivityManager();
        var activity = new Discord.Activity
        {
            // В зависимости от режима изменяем описание активности
            Details = isSoloMode ? "Выживание" : "Multiplayer Mode",
            Timestamps =
            {
                Start = System.DateTimeOffset.Now.ToUnixTimeSeconds(), // Время начала сессии
            },
            Assets =
            {
                LargeImage = "logo", // ключ изображения, загруженного в Discord Developer Portal
                LargeText = "Ты лох",   // текст при наведении на изображение
                SmallText = isSoloMode ? "Rogue - Level 100" : "Rogue - Level 100 (Multiplayer)", // текст при наведении на маленькую иконку
            }
        };

        // Если не одиночная игра, добавляем информацию о пати и возможности присоединения
        if (!isSoloMode)
        {
            activity.Party = new Discord.ActivityParty
            {
                Id = "multiplayer_party_id", // Идентификатор пати для многопользовательского режима
                Size = new Discord.PartySize
                {
                    CurrentSize = 2,  // Текущее количество участников
                    MaxSize = 5,      // Максимальное количество участников
                },
            };
            activity.Secrets = new Discord.ActivitySecrets
            {
                Join = "MTI4NzM0OjFpMmhuZToxMjMxMjM=", // Секретный ключ для присоединения к игре
            };
        }

        // Обновление активности
        activityManager.UpdateActivity(activity, (result) =>
        {
            if (result == Discord.Result.Ok)
            {
                Debug.Log("Rich Presence обновлено!");
            }
            else
            {
                Debug.LogError("Не удалось обновить Rich Presence: " + result);
            }
        });
    }

    // Обработка событий Discord SDK в каждом кадре
    void Update()
    {
        discord.RunCallbacks(); // Обрабатывает события Discord SDK
    }

    // Очистка ресурсов при завершении приложения
    void OnApplicationQuit()
    {
        discord.Dispose(); // Освобождение ресурсов
    }
}
