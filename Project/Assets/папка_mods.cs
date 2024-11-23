using System.IO;
using UnityEngine;

public class ModManager : MonoBehaviour
{
    void Start()
    {
        // Определяем путь к директории с .exe файлом
        string modsPath = Path.Combine(Application.dataPath, "../Mods");

        // Преобразуем относительный путь в абсолютный
        modsPath = Path.GetFullPath(modsPath);

        // Проверяем, существует ли папка
        if (!Directory.Exists(modsPath))
        {
            // Создаём папку
            Directory.CreateDirectory(modsPath);
        }
        else
        {
            Debug.Log("Папка Mods уже есть");
        }
    }
}
