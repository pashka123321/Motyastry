using System.IO;
using UnityEngine;
using System.Collections.Generic;

public class ModScanner : MonoBehaviour
{
    public List<string> modPaths = new List<string>();

    public void ScanForMods()
    {
        string modsDirectory = Path.Combine(Application.dataPath, "../Mods");

        if (Directory.Exists(modsDirectory))
        {
            string[] modDirectories = Directory.GetDirectories(modsDirectory);

            foreach (string modDir in modDirectories)
            {
                string[] luaFiles = Directory.GetFiles(modDir, "*.lua");

                foreach (string luaFile in luaFiles)
                {
                    modPaths.Add(luaFile);
                }
            }

            Debug.Log("Найдено модов: " + modPaths.Count);
        }
        else
        {
            Debug.LogWarning("Папка Mods не найдена!");
        }
    }
}