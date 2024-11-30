using System;
using System.Collections.Generic;
using System.IO;
using MoonSharp.Interpreter;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ModLoader : MonoBehaviour
{
    private List<Script> loadedScripts = new List<Script>();

    private Dictionary<string, bool> modStates = new Dictionary<string, bool>();

    private void Log(string message)
    {
        Debug.Log("[ModLoader] " + message);
    }

    private bool FileExists(string path)
    {
        return File.Exists(path);
    }

    private string SaveFilePath => Path.Combine(Application.persistentDataPath, "ModStates.json");
private void LoadModStates()
{
    if (File.Exists(SaveFilePath))
    {
        string json = File.ReadAllText(SaveFilePath);
        ModStateData data = JsonUtility.FromJson<ModStateData>(json);

        modStates = new Dictionary<string, bool>();
        for (int i = 0; i < data.modPaths.Count; i++)
        {
            modStates[data.modPaths[i]] = data.modStates[i];
        }
    }
    else
    {
        modStates = new Dictionary<string, bool>();
    }
}


private void SaveModStates()
{
    ModStateData data = new ModStateData();
    foreach (var entry in modStates)
    {
        data.modPaths.Add(entry.Key);
        data.modStates.Add(entry.Value);
    }

    string json = JsonUtility.ToJson(data);
    File.WriteAllText(SaveFilePath, json);
}


public bool GetModState(string modPath)
{
    return modStates.TryGetValue(modPath, out var isEnabled) && isEnabled;
}

[System.Serializable]
private class ModStateData
{
    public List<string> modPaths = new List<string>();
    public List<bool> modStates = new List<bool>();
}



public void UpdateModState(string modPath, bool isEnabled)
{
    modStates[modPath] = isEnabled; // Сохраняем состояние
    SaveModStates(); // Записываем в файл

    if (isEnabled)
    {
        LoadScript(modPath); // Загрузка мода
    }
    else
    {
        UnloadScript(modPath); // Выгрузка мода
    }
}



private void UnloadScript(string modPath)
{
    Script script = loadedScripts.Find(s => s.Globals.Get("ScriptPath").String == modPath);
    if (script != null)
    {
        loadedScripts.Remove(script);
        Debug.Log($"Мод {modPath} выгружен.");
    }
}

    private IEnumerator WaitForAndPlayAudio(WWW audioLoader)
    {
        yield return audioLoader;

        if (string.IsNullOrEmpty(audioLoader.error))
        {
            AudioClip audioClip = audioLoader.GetAudioClip();
            if (audioClip != null)
            {
                GameObject audioObject = new GameObject("TemporaryAudio");
                AudioSource audioSource = audioObject.AddComponent<AudioSource>();
                audioSource.clip = audioClip;
                audioSource.Play();
                Log("Audio played successfully!");
                Destroy(audioObject, audioClip.length);
            }
            else
            {
                Log("Failed to load audio clip.");
            }
        }
        else
        {
            Log("Error loading audio: " + audioLoader.error);
        }
    }

    private bool IsKeyPressed(string key)
    {
        try
        {
            KeyCode keyCode = (KeyCode)Enum.Parse(typeof(KeyCode), key, true);
            return Input.GetKeyDown(keyCode);
        }
        catch
        {
            Log($"Unknown key: {key}");
            return false;
        }
    }

private void Start()
{
    LoadModStates(); // Загружаем сохранённые состояния модов

    string modsFolder = Path.Combine(Application.dataPath, "../Mods");
    if (!Directory.Exists(modsFolder))
    {
        Log("Папка модов не найдена.");
        return;
    }

    foreach (var modFolder in Directory.GetDirectories(modsFolder))
    {
        string scriptPath = Path.Combine(modFolder, "script.lua");
        if (File.Exists(scriptPath))
        {
            // Учитываем состояние мода
            bool isEnabled = modStates.TryGetValue(scriptPath, out var enabled) && enabled;
            if (isEnabled)
            {
                LoadScript(scriptPath);
            }
        }
    }
}


    private void Update()
    {
        foreach (var script in loadedScripts)
        {
            DynValue updateFunction = script.Globals.Get("Update");
            if (updateFunction.Type == DataType.Function)
            {
                try
                {
                    script.Call(updateFunction);
                }
                catch (Exception e)
                {
                    Log($"Error in Lua Update: {e.Message}");
                }
            }
        }
    }


private void LoadScript(string scriptPath)
{
    try
    {
        Log("Загрузка скрипта: " + scriptPath);
        Script luaScript = new Script();
        luaScript.Options.ScriptLoader = new CustomScriptLoader();

        // Регистрируем API
        luaScript.Globals["Log"] = (Action<string>)Log;
        luaScript.Globals["FileExists"] = (Func<string, bool>)FileExists;
        luaScript.Globals["PlayAudio"] = (Action<string>)PlayAudio;
        luaScript.Globals["IsKeyPressed"] = (Func<string, bool>)IsKeyPressed;
        luaScript.Globals["LoadScene"] = (Action<string>)LoadScene;
        luaScript.Globals["CreateSprite"] = (Func<string, LuaVector3, DynValue>)((path, pos) => UserData.Create(CreateSprite(path, pos)));
        luaScript.Globals["AttachToParent"] = (Action<DynValue, string>)((child, parentName) =>
        {
            AttachToParent(child.ToObject<GameObject>(), parentName);
        });

luaScript.Globals["ReplaceSprite"] = (Action<string, string>)((objectName, spritePath) =>
{
    ReplaceSprite(objectName, spritePath);
});



luaScript.Globals["SetObjectPosition"] = (Action<DynValue, LuaVector3>)((obj, position) =>
{
    SetObjectPosition(obj.ToObject<GameObject>(), position);
});

luaScript.Globals["AttachToParent"] = (Action<DynValue, string, DynValue>)((child, parentName, offset) =>
{
    LuaVector3 luaOffset = offset?.ToObject<LuaVector3>();
    AttachToParent(child.ToObject<GameObject>(), parentName, luaOffset);
});

luaScript.Globals["GetObjectPosition"] = (Func<DynValue, LuaVector3>)(obj =>
{
    return GetObjectPosition(obj.ToObject<GameObject>());
});

luaScript.Globals["CreateCanvas"] = (Func<string, DynValue>)(canvasName =>
{
    GameObject canvasObject = CreateCanvas(canvasName);
    return UserData.Create(canvasObject);
});

luaScript.Globals["AddText"] = (Func<DynValue, string, int, LuaVector3, LuaVector3, DynValue>)((parent, text, fontSize, position, color) =>
{
    GameObject parentObject = parent.ToObject<GameObject>();
    Color textColor = new Color(color.x, color.y, color.z); // Преобразуем LuaVector3 в Color
    GameObject textObject = AddTextToObject(parentObject, text, fontSize, textColor, position);
    return UserData.Create(textObject);
});

luaScript.Globals["CreateButton"] = (Func<string, string, LuaVector3, DynValue, string, DynValue>)((name, buttonText, position, parent, targetScene) =>
{
    GameObject parentObject = parent?.ToObject<GameObject>();
    if (parentObject == null)
    {
        Log("Ошибка: родительский объект не найден.");
        return null;
    }

    GameObject button = CreateButton(name, buttonText, position, parentObject, targetScene);
    return UserData.Create(button);
});


luaScript.Globals["CreateButton"] = (Func<string, string, LuaVector3, DynValue, DynValue>)((buttonName, text, position, parentCanvas) =>
{
    GameObject canvasObject = parentCanvas.ToObject<GameObject>();
    GameObject buttonObject = CreateButton(buttonName, text, position, canvasObject);
    return UserData.Create(buttonObject);
});

luaScript.Globals["ScaleToFitCamera"] = (Action<DynValue, string>)((obj, cameraName) =>
{
    Camera camera = GameObject.Find(cameraName)?.GetComponent<Camera>();
    if (camera != null)
    {
        ScaleToFitCamera(obj.ToObject<GameObject>(), camera);
    }
    else
    {
        Log($"Ошибка: камера с именем {cameraName} не найдена.");
    }
});


luaScript.Globals["ReplaceTextureByName"] = (Action<string, string>)((textureName, texturePath) =>
{
    // Загружаем новую текстуру с указанного пути
    Texture2D newTexture = LoadTextureFromFile(texturePath);
    if (newTexture != null)
    {
        // Ищем текстуру в Resources
        Texture2D existingTexture = Resources.Load<Texture2D>(textureName);
        if (existingTexture != null)
        {
            // Если текстура найдена, заменяем её
            existingTexture = newTexture;
            Log($"Текстура '{textureName}' успешно заменена на '{texturePath}'.");
        }
        else
        {
            Log($"Ошибка: текстура с именем '{textureName}' не найдена в Resources.");
        }
    }
    else
    {
        Log($"Ошибка: текстура по пути '{texturePath}' не найдена.");
    }
});



        luaScript.Globals["SetObjectScale"] = (Action<DynValue, LuaVector3>)((obj, scale) =>
        {
            SetObjectScale(obj.ToObject<GameObject>(), scale);
        });

        // Регистрируем LuaVector3
        UserData.RegisterType<LuaVector3>();
        luaScript.Globals["Vector3"] = (Func<float, float, float, LuaVector3>)((x, y, z) => new LuaVector3(x, y, z));

        // Регистрируем GameObject
        UserData.RegisterType<GameObject>();

        luaScript.DoFile(scriptPath);
        loadedScripts.Add(luaScript);
        
    }
    catch (Exception e)
    {
        Log("Ошибка загрузки скрипта: " + e.Message);
    }
}



    private void LoadScene(string sceneName)
    {
        try
        {
            SceneManager.LoadScene(sceneName);
            Log($"Scene {sceneName} loaded successfully.");
        }
        catch (Exception e)
        {
            Log($"Error loading scene {sceneName}: {e.Message}");
        }
    }

private GameObject CreateCanvas(string canvasName)
{
    try
    {
        Log($"Создание канваса: {canvasName}");

        // Создаем GameObject для канваса
        GameObject canvasObject = new GameObject(canvasName);

        // Добавляем компонент Canvas
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay; // Устанавливаем режим отрисовки

        // Добавляем CanvasScaler для автоматического масштабирования
        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080); // Референсное разрешение

        // Добавляем GraphicRaycaster для обработки событий UI
        canvasObject.AddComponent<GraphicRaycaster>();

        return canvasObject;
    }
    catch (Exception e)
    {
        Log($"Ошибка создания канваса: {e.Message}");
        return null;
    }
}

    private void PlayAudio(string path)
    {
        try
        {
            string absolutePath = Path.Combine(Application.dataPath, "../" + path);
            absolutePath = "file://" + absolutePath.Replace("\\", "/");
            Log("Playing audio from: " + absolutePath);

            WWW audioLoader = new WWW(absolutePath);
            StartCoroutine(WaitForAndPlayAudio(audioLoader));
        }
        catch (Exception e)
        {
            Log("Error playing audio: " + e.Message);
        }
    }



private GameObject CreateSprite(string imagePath, LuaVector3 luaPosition)
{
    try
    {
        // Получаем полный путь к изображению
        string fullPath = Path.Combine(Application.dataPath, "../" + imagePath);
        if (!File.Exists(fullPath))
        {
            Log($"Изображение не найдено по пути: {fullPath}");
            return null;
        }

        // Загружаем изображение
        byte[] imageData = File.ReadAllBytes(fullPath);
        Texture2D texture = new Texture2D(2, 2);
        if (!texture.LoadImage(imageData))
        {
            Log($"Не удалось загрузить изображение: {imagePath}");
            return null;
        }

        // Создаём спрайт
        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f)
        );

        // Создаём GameObject для спрайта
        GameObject spriteObject = new GameObject("SpriteObject");
        spriteObject.transform.position = luaPosition.ToUnityVector3();

        // Добавляем SpriteRenderer для отображения
        SpriteRenderer renderer = spriteObject.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;

        Log($"Спрайт создан по пути {imagePath} и размещён в позиции {luaPosition}.");
        return spriteObject;
    }
    catch (Exception e)
    {
        Log($"Ошибка при создании спрайта: {e.Message}");
        return null;
    }
}

private Texture2D LoadTextureFromFile(string path)
{
    if (File.Exists(path))
    {
        byte[] fileData = File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(2, 2);
        if (texture.LoadImage(fileData))
        {
            return texture;
        }
    }
    return null;
}

private void SetObjectScale(GameObject obj, LuaVector3 scale)
{
    if (obj == null)
    {
        Log("Ошибка: объект не существует.");
        return;
    }

    obj.transform.localScale = scale.ToUnityVector3();
    Log($"Размер объекта {obj.name} изменён на {scale}.");
}


private void AttachToParent(GameObject child, string parentName, LuaVector3 offset = null)
{
    GameObject parent = GameObject.Find(parentName);
    if (parent == null)
    {
        Log($"Ошибка: родительский объект {parentName} не найден.");
        return;
    }

    child.transform.SetParent(parent.transform, true);

    // Установка позиции с учётом смещения
    Vector3 parentPosition = parent.transform.position;
    Vector3 finalPosition = parentPosition;

    if (offset != null)
    {
        finalPosition += offset.ToUnityVector3();
    }

    child.transform.position = finalPosition;
    Log($"Объект {child.name} привязан к {parentName} с позицией {finalPosition}.");
}

private GameObject CreateButton(string buttonName, string text, LuaVector3 position, GameObject parentCanvas)
{
    try
    {
        Log($"Создание кнопки: {buttonName}");

        // Убедимся, что родитель - это канвас
        if (parentCanvas.GetComponent<Canvas>() == null)
        {
            Log("Ошибка: Указанный родитель не является канвасом.");
            return null;
        }

        // Создаем GameObject для кнопки
        GameObject buttonObject = new GameObject(buttonName);

        // Добавляем RectTransform для позиционирования в UI
        RectTransform rectTransform = buttonObject.AddComponent<RectTransform>();
        rectTransform.SetParent(parentCanvas.transform, false); // Присоединяем к канвасу
        rectTransform.anchoredPosition = new Vector2(position.x, position.y); // Устанавливаем позицию

        // Добавляем компонент Button
        Button button = buttonObject.AddComponent<Button>();

        // Добавляем Image для визуального отображения кнопки
        Image image = buttonObject.AddComponent<Image>();
        image.color = Color.white; // Цвет кнопки (можно настроить)

        // Создаем текстовый объект как дочерний элемент
        GameObject textObject = new GameObject("ButtonText");
        textObject.transform.SetParent(buttonObject.transform, false);

        Text textComponent = textObject.AddComponent<Text>();
        textComponent.text = text;
        textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf"); // Встроенный шрифт
        textComponent.color = Color.black; // Цвет текста
        textComponent.alignment = TextAnchor.MiddleCenter; // Центрирование текста

        // Настраиваем RectTransform текста
        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero; // Заполнение кнопки текстом

        return buttonObject;
    }
    catch (Exception e)
    {
        Log($"Ошибка создания кнопки: {e.Message}");
        return null;
    }
}


    private void SpawnObject(string prefabName, Vector3 position)
    {
        GameObject prefab = Resources.Load<GameObject>(prefabName);
        if (prefab != null)
        {
            Instantiate(prefab, position, Quaternion.identity);
            Log($"Object {prefabName} spawned at {position}.");
        }
        else
        {
            Log($"Prefab {prefabName} not found.");
        }
    }

[MoonSharpUserData]
public class LuaVector3
{
    public float x;
    public float y;
    public float z;

    public LuaVector3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public LuaVector3(Vector3 vector)
    {
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }

    public Vector3 ToUnityVector3()
    {
        return new Vector3(x, y, z);
    }

    public override string ToString()
    {
        return $"LuaVector3({x}, {y}, {z})";
    }
}

private void ScaleToFitCamera(GameObject obj, Camera camera)
{
    if (obj == null)
    {
        Log("Ошибка: объект не существует.");
        return;
    }

    if (camera == null)
    {
        Log("Ошибка: камера не найдена.");
        return;
    }

    // Получаем размер камеры в мире
    float height = 2f * camera.orthographicSize;
    float width = height * camera.aspect;

    // Изменяем размер объекта
    SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
    if (spriteRenderer != null)
    {
        // Получаем размер текстуры
        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;

        // Рассчитываем масштаб
        float scaleX = width / spriteSize.x;
        float scaleY = height / spriteSize.y;

        // Применяем масштаб
        obj.transform.localScale = new Vector3(scaleX, scaleY, 1f);
        Log($"Размер объекта {obj.name} изменён для заполнения камеры.");
    }
    else
    {
        Log($"Ошибка: объект {obj.name} не имеет компонента SpriteRenderer.");
    }
}

private void SetObjectPosition(GameObject obj, LuaVector3 position)
{
    if (obj == null)
    {
        Log("Ошибка: объект не существует.");
        return;
    }

    obj.transform.position = position.ToUnityVector3();
    Log($"Позиция объекта {obj.name} установлена в {position}.");
}

private LuaVector3 GetObjectPosition(GameObject obj)
{
    if (obj == null)
    {
        Log("Ошибка: объект не существует.");
        return null;
    }

    Vector3 position = obj.transform.position;
    Log($"Позиция объекта {obj.name}: {position}");
    return new LuaVector3(position);
}

private GameObject AddTextToObject(GameObject parentObject, string text, int fontSize, Color color, LuaVector3 position)
{
    try
    {
        Log($"Добавление текста к объекту: {parentObject.name}");

        // Создаем объект для текста
        GameObject textObject = new GameObject("Text");
        textObject.transform.SetParent(parentObject.transform, false);

        // Настраиваем RectTransform
        RectTransform rectTransform = textObject.AddComponent<RectTransform>();
        rectTransform.localPosition = new Vector3(position.x, position.y, position.z); // Позиция внутри родителя
        rectTransform.sizeDelta = new Vector2(160, 30); // Размер текста
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        // Добавляем компонент Text
        Text textComponent = textObject.AddComponent<Text>();
        textComponent.text = text;
        textComponent.fontSize = fontSize;
        textComponent.color = color;

        // Настраиваем шрифт
        Font defaultFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
        if (defaultFont != null)
        {
            textComponent.font = defaultFont;
        }
        else
        {
            Log("Ошибка: Шрифт Arial не найден.");
        }

        textComponent.alignment = TextAnchor.MiddleCenter; // Центрируем текст

        return textObject;
    }
    catch (Exception e)
    {
        Log($"Ошибка добавления текста: {e.Message}");
        return null;
    }
}

private GameObject CreateButton(string name, string text, LuaVector3 position, GameObject parent, string targetScene)
{
    try
    {
        Log($"Создание кнопки: {name}");

        // Создаем объект кнопки
        GameObject buttonObject = new GameObject(name);
        buttonObject.transform.SetParent(parent.transform, false);

        // Настраиваем RectTransform
        RectTransform rectTransform = buttonObject.AddComponent<RectTransform>();
        rectTransform.localPosition = new Vector3(position.x, position.y, position.z);
        rectTransform.sizeDelta = new Vector2(160, 30); // Размер кнопки
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        // Добавляем компонент Button
        Button buttonComponent = buttonObject.AddComponent<Button>();

        // Добавляем Background для кнопки
        Image buttonImage = buttonObject.AddComponent<Image>();
        buttonImage.color = Color.white; // Цвет фона кнопки
        buttonComponent.targetGraphic = buttonImage;

        // Проверка targetScene
        if (string.IsNullOrEmpty(targetScene))
        {
            Log($"Ошибка: targetScene не указана для кнопки {name}");
        }

        // Настраиваем событие OnClick для кнопки
        buttonComponent.onClick.AddListener(() =>
        {
            Log($"Кнопка {name} нажата. Переход на сцену: {targetScene}");
            LoadScene(targetScene);
        });

        // Добавляем текст к кнопке
        GameObject textObject = new GameObject("Text");
        textObject.transform.SetParent(buttonObject.transform, false);

        RectTransform textRectTransform = textObject.AddComponent<RectTransform>();
        textRectTransform.localPosition = Vector3.zero;
        textRectTransform.sizeDelta = rectTransform.sizeDelta; // Размер текста совпадает с размером кнопки
        textRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        textRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        textRectTransform.pivot = new Vector2(0.5f, 0.5f);

        Text textComponent = textObject.AddComponent<Text>();
        textComponent.text = text;
        textComponent.fontSize = 24;
        textComponent.color = Color.black;

        Font defaultFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
        if (defaultFont != null)
        {
            textComponent.font = defaultFont;
        }
        else
        {
            Log("Ошибка: Шрифт Arial не найден.");
        }

        textComponent.alignment = TextAnchor.MiddleCenter;

        return buttonObject;
    }
    catch (Exception e)
    {
        Log($"Ошибка создания кнопки: {e.Message}");
        return null;
    }
}

private void ReplaceSprite(string objectName, string spritePath)
{
    try
    {
        // Найти объект по имени
        GameObject obj = GameObject.Find(objectName);
        if (obj == null)
        {
            Log($"Ошибка: объект с именем '{objectName}' не найден.");
            return;
        }

        // Проверить наличие SpriteRenderer
        SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Log($"Ошибка: объект '{objectName}' не содержит компонент SpriteRenderer.");
            return;
        }

        // Проверить, существует ли файл
        if (!File.Exists(spritePath))
        {
            Log($"Ошибка: файл '{spritePath}' не найден.");
            return;
        }

        // Загрузить изображение
        byte[] imageData = File.ReadAllBytes(spritePath);
        Texture2D texture = new Texture2D(2, 2);
        if (!texture.LoadImage(imageData))
        {
            Log($"Ошибка: не удалось загрузить текстуру из файла '{spritePath}'.");
            return;
        }

        // Установить фильтр текстуры на Point (без фильтрации)
        texture.filterMode = FilterMode.Point;
        texture.Apply(); // Применить изменения текстуры

        // Создать новый спрайт
        Sprite newSprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f)
        );

        // Установить новый спрайт
        spriteRenderer.sprite = newSprite;
        Log($"Спрайт объекта '{objectName}' успешно заменён.");
    }
    catch (Exception e)
    {
        Log($"Ошибка в ReplaceSprite: {e.Message}");
    }
}



}
