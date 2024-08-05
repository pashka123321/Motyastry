using UnityEngine;

public class SetLayer : MonoBehaviour
{
    [SerializeField]
    private int layer; // Слой, который можно установить в инспекторе

    private void Start()
    {
        // Установить слой объекту при старте сцены
        gameObject.layer = layer;
    }

    // Этот метод позволит установить слой из другого скрипта, если понадобится
    public void SetObjectLayer(int newLayer)
    {
        gameObject.layer = newLayer;
    }
}
