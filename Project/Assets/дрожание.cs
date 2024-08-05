using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonShake : MonoBehaviour
{
    public Button[] buttons; // Массив кнопок
    public float shakeAmount = 10f; // Сила дрожания
    public float shakeDuration = 0.1f; // Длительность дрожания

    private RectTransform buttonTransform;
    private Vector2 originalPosition;
    private bool isShaking;
    private Coroutine shakeCoroutine;

    private void Start()
    {
        foreach (Button button in buttons)
        {
            // Добавляем обработчики событий на кнопки
            var buttonComponent = button.gameObject.AddComponent<ShakeButton>();
            buttonComponent.Initialize(this);
        }
    }

    public void OnPointerEnter(RectTransform rectTransform)
    {
        if (rectTransform == null)
            return;

        buttonTransform = rectTransform;
        originalPosition = buttonTransform.anchoredPosition;

        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        isShaking = true;
        shakeCoroutine = StartCoroutine(ShakeCoroutine());
    }

    public void OnPointerExit()
    {
        isShaking = false;
        if (buttonTransform != null)
        {
            StopCoroutine(shakeCoroutine);
            buttonTransform.anchoredPosition = originalPosition;
        }
    }

    private IEnumerator ShakeCoroutine()
    {
        float elapsed = 0f;

        while (isShaking)
        {
            if (buttonTransform != null)
            {
                Vector2 shakeOffset = Random.insideUnitCircle * shakeAmount;
                buttonTransform.anchoredPosition = originalPosition + shakeOffset;
            }

            elapsed += Time.deltaTime;
            if (elapsed >= shakeDuration)
            {
                if (buttonTransform != null)
                {
                    buttonTransform.anchoredPosition = originalPosition;
                }
                isShaking = false;
            }

            yield return null;
        }
    }

    // Вспомогательный класс для добавления обработчиков событий на кнопки
    public class ShakeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private ButtonShake buttonShake;

        public void Initialize(ButtonShake shake)
        {
            buttonShake = shake;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            var rectTransform = GetComponent<RectTransform>();
            buttonShake.OnPointerEnter(rectTransform);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            buttonShake.OnPointerExit();
        }
    }
}
