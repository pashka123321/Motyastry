using System.IO;
using MoonSharp.Interpreter.Loaders;
using MoonSharp.Interpreter;

public class CustomScriptLoader : ScriptLoaderBase
{
    public override object LoadFile(string file, Table globalContext)
    {
        if (!File.Exists(file))
        {
            throw new FileNotFoundException($"Cannot find script file: {file}");
        }

        return File.ReadAllText(file); // Загружаем содержимое скрипта
    }

    public override bool ScriptFileExists(string file)
    {
        return File.Exists(file);
    }
}
