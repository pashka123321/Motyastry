using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UIHoverDescription : MonoBehaviour
{
    [System.Serializable]
    public class UIElementDescription
    {
        public GameObject uiElement;
        public string descriptionText;
    }

    public UIElementDescription[] elements; // Массив объектов и их описаний
    public GameObject descriptionPanel; // Панель описания
    public Text descriptionField; // Поле для текста описания

    private string currentDescription;
    private bool isDescriptionFixed = false;

    void Start()
    {
        if (descriptionPanel != null)
        {
            descriptionPanel.SetActive(false); // Скрываем панель в начале
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // ПКМ
        {
            isDescriptionFixed = false;
            HideDescription();
        }

        if (!isDescriptionFixed)
        {
            CheckMouseHover();
        }
    }

    private void CheckMouseHover()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };
        
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        
        bool isHovering = false;
        foreach (var result in results)
        {
            foreach (var element in elements)
            {
                if (result.gameObject == element.uiElement)
                {
                    if (Input.GetMouseButtonDown(0)) // ЛКМ
                    {
                        isDescriptionFixed = true;
                    }
                    currentDescription = element.descriptionText;
                    ShowDescription();
                    isHovering = true;
                    break;
                }
            }
            if (isHovering) break;
        }
        
        if (!isHovering)
        {
            HideDescription();
        }
    }

    private void ShowDescription()
    {
        if (descriptionPanel != null && descriptionField != null)
        {
            descriptionField.text = currentDescription;
            descriptionPanel.SetActive(true);
        }
    }

    private void HideDescription()
    {
        if (descriptionPanel != null)
        {
            descriptionPanel.SetActive(false);
        }
    }
}
