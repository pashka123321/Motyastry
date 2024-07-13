using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Button saveButton;
    public Button loadButton;
    public GameManager gameManager;

    void Start()
    {
        saveButton.onClick.AddListener(gameManager.SaveGame);
        loadButton.onClick.AddListener(gameManager.LoadGame);
    }
}
