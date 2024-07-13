using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public List<GameObject> gameObjects; // Все объекты, которые нужно сохранять
    public List<GameObject> prefabList;  // Список всех возможных префабов для динамического создания

    // Сохранение игры
    public void SaveGame()
    {
        GameData data = new GameData();
        data.sceneName = SceneManager.GetActiveScene().name;

        foreach (GameObject obj in gameObjects)
        {
            ObjectData objectData = new ObjectData();
            objectData.name = obj.name;
            objectData.prefabName = obj.name; // Предполагается, что имя объекта совпадает с именем префаба
            objectData.posX = obj.transform.position.x;
            objectData.posY = obj.transform.position.y;
            objectData.posZ = obj.transform.position.z;
            objectData.rotX = obj.transform.rotation.eulerAngles.x;
            objectData.rotY = obj.transform.rotation.eulerAngles.y;
            objectData.rotZ = obj.transform.rotation.eulerAngles.z;
            objectData.isActive = obj.activeSelf;
            data.objects.Add(objectData);
        }

        SaveSystem.SaveGame(data, "savefile");
    }

    // Загрузка игры
    public void LoadGame()
    {
        GameData data = SaveSystem.LoadGame("savefile");
        if (data == null)
            return;

        StartCoroutine(LoadScene(data));
    }

    // Асинхронная загрузка сцены
    private IEnumerator LoadScene(GameData data)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(data.sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Восстанавливаем состояния объектов после загрузки сцены
        foreach (ObjectData objectData in data.objects)
        {
            GameObject obj = GameObject.Find(objectData.name);
            if (obj == null)
            {
                // Объект не найден, создаем новый из префаба
                GameObject prefab = prefabList.Find(p => p.name == objectData.prefabName);
                if (prefab != null)
                {
                    obj = Instantiate(prefab);
                    obj.name = objectData.name; // Устанавливаем имя объекта для соответствия данным
                }
            }

            if (obj != null)
            {
                obj.transform.position = new Vector3(objectData.posX, objectData.posY, objectData.posZ);
                obj.transform.rotation = Quaternion.Euler(objectData.rotX, objectData.rotY, objectData.rotZ);
                obj.SetActive(objectData.isActive);
            }
        }
    }

    // Метод для динамического создания объектов
    public void CreateObject(string prefabName, Vector3 position, Quaternion rotation)
    {
        GameObject prefab = prefabList.Find(p => p.name == prefabName);
        if (prefab != null)
        {
            GameObject obj = Instantiate(prefab, position, rotation);
            obj.name = prefabName; // Устанавливаем имя объекта для соответствия данным
            gameObjects.Add(obj);
        }
    }
}
