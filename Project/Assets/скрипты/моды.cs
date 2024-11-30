using System;
using System.Collections.Generic;
using System.IO;
using MoonSharp.Interpreter;
using UnityEngine;
public class PonModManager : MonoBehaviour
{
    public ModScanner scanner;
    public ModUIManager uiManager;
    public LuaModExecutor executor;

    void Start()
    {
        // Сканируем моды
        scanner.ScanForMods();

        // Заполняем UI
        uiManager.PopulateModList(scanner.modPaths);
    }

    public void ExecuteSelectedMods()
    {
        foreach (var toggleEntry in uiManager.modToggles)
        {
            if (toggleEntry.Key.isOn)
            {
                executor.ExecuteLuaScript(toggleEntry.Value);
            }
        }
    }
}