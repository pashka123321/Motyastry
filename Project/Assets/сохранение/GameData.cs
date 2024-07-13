using System;
using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public string sceneName;
    public List<ObjectData> objects = new List<ObjectData>();
}

[System.Serializable]
public class ObjectData
{
    public string name;
    public string prefabName;  // Для идентификации префаба при загрузке
    public float posX;
    public float posY;
    public float posZ;
    public float rotX;
    public float rotY;
    public float rotZ;
    public bool isActive;
}
