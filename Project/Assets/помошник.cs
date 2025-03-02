using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameHelper : MonoBehaviour
{
    public Text notificationText;
    public float fadeDuration = 1f;
    public int fixedFontSize = 30;
    private CanvasGroup canvasGroup;

    void Start()
    {
        // Добавляем CanvasGroup, если его нет
        canvasGroup = notificationText.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = notificationText.gameObject.AddComponent<CanvasGroup>();

        notificationText.fontSize = fixedFontSize; 
        canvasGroup.alpha = 0; 
        notificationText.gameObject.SetActive(false);

        StartCoroutine(ShowNotification());
    }

    IEnumerator ShowNotification()
    {
        yield return new WaitForSeconds(5f);
        notificationText.text = "Найди медь, добудь буром, переплавь и проведи к ядру!";
        notificationText.gameObject.SetActive(true);
        yield return StartCoroutine(FadeIn());
    }

    public void HideNotification()
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1;
    }

    IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(1 - (elapsedTime / fadeDuration));
            yield return null;
        }
        canvasGroup.alpha = 0;
        notificationText.gameObject.SetActive(false);
    }
}
