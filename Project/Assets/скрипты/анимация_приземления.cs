using UnityEngine;

public class PlayAnimationOnce : MonoBehaviour
{
    private Animator animator;
    private bool hasPlayed = false;

    // Массив имен анимаций, которые будут воспроизводиться случайным образом
    public string[] animationNames;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (animator != null)
        {
            PlayRandomAnimation();
        }
        else
        {
            Debug.LogError("Animator component is missing on the object " + gameObject.name);
        }
    }

    void PlayRandomAnimation()
    {
        if (!hasPlayed && animator != null)
        {
            // Проверяем, что массив анимаций не пустой
            if (animationNames.Length > 0)
            {
                // Выбираем случайную анимацию из массива
                int randomIndex = Random.Range(0, animationNames.Length);
                string randomAnimation = animationNames[randomIndex];

                // Проверяем, есть ли такое состояние в Animator
                if (animator.HasState(0, Animator.StringToHash(randomAnimation)))
                {
                    // Воспроизводим выбранную анимацию
                    animator.Play(randomAnimation, 0); // Указываем слой 0
                    hasPlayed = true;
                }
                else
                {
                    Debug.LogError("Animator does not contain the state: " + randomAnimation);
                }
            }
            else
            {
                Debug.LogError("Animation names array is empty.");
            }
        }
    }
}
