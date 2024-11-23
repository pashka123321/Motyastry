using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
public class ModUIManager : MonoBehaviour
{
    public GameObject modTogglePrefab; // Префаб чекбокса
    public Transform modListContainer; // Контейнер для списка модов

    public Dictionary<Toggle, string> modToggles = new Dictionary<Toggle, string>();

public void PopulateModList(List<string> modPaths)
{
    ModLoader modLoader = FindObjectOfType<ModLoader>();

    foreach (string modPath in modPaths)
    {
        GameObject toggleObj = Instantiate(modTogglePrefab, modListContainer);
        Toggle toggle = toggleObj.GetComponent<Toggle>();
        Text label = toggleObj.GetComponentInChildren<Text>();
        
        // Устанавливаем текст для чекбокса
        label.text = Path.GetFileName(Path.GetDirectoryName(modPath)); // Имя мода (папка)

        // Устанавливаем состояние чекбокса
        bool isEnabled = modLoader.GetModState(modPath);
        toggle.isOn = isEnabled;

        // Сохраняем связь
        modToggles[toggle] = modPath;

        // Добавляем обработчик изменения состояния
        toggle.onValueChanged.AddListener(isOn =>
        {
            modLoader.UpdateModState(modPath, isOn);

            if (isOn)
            {
                Debug.Log("Активирован мод: " + modPath);
            }
            else
            {
                Debug.Log("Деактивирован мод: " + modPath);
            }
        });
    }
}

}