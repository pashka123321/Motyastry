using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class QueuedBlock
{
    public GameObject blockPrefab;
    public Vector3 position;
    public Quaternion rotation;
    public float buildTime;

    public QueuedBlock(GameObject blockPrefab, Vector3 position, Quaternion rotation, float buildTime)
    {
        this.blockPrefab = blockPrefab;
        this.position = position;
        this.rotation = rotation;
        this.buildTime = buildTime;
    }
    
public class TimerText : MonoBehaviour
{
    public Text timerText;

    public void SetText(string text)
    {
        if (timerText != null)
        {
            timerText.text = text;
        }
    }
}

}

