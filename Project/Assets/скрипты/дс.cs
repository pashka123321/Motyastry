using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OpenLinkOnClick : MonoBehaviour
{
    public string url = "https://discord.gg/EvHBPtzHZ3";

    void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OpenURL);
        }
    }

    void OpenURL()
    {
        Application.OpenURL(url);
    }
}
