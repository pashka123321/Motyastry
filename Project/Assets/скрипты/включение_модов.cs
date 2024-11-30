using System.IO;
using MoonSharp.Interpreter;
using UnityEngine;

public class LuaModExecutor : MonoBehaviour
{
    public void ExecuteLuaScript(string path)
    {
        if (File.Exists(path))
        {
            string scriptCode = File.ReadAllText(path);

            try
            {
                Script script = new Script();
                script.DoString(scriptCode);
                Debug.Log("Скрипт выполнен успешно: " + path);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Ошибка при выполнении скрипта: " + ex.Message);
            }
        }
        else
        {
            Debug.LogError("Файл не найден: " + path);
        }
    }
}