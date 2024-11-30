using UnityEngine;

public class PlayAnimationOnce : MonoBehaviour
{
    [SerializeField] private Animator animator; // Ссылка на Animator
    [SerializeField] private string animationName; // Имя анимации, которую нужно проиграть

    private void Start()
    {
        // Проверяем, указана ли анимация
        if (animator == null || string.IsNullOrEmpty(animationName))
        {
            Debug.LogError("Animator или имя анимации не назначены!");
            return;
        }

        // Проигрываем анимацию
        animator.Play(animationName);

        // Получаем длину анимации
        AnimationClip clip = GetAnimationClip(animationName);
        if (clip != null)
        {
            // Отключаем Animator после завершения анимации
            Invoke(nameof(DisableAnimator), clip.length);
        }
        else
        {
            Debug.LogWarning($"Анимация {animationName} не найдена!");
        }
    }

    private AnimationClip GetAnimationClip(string name)
    {
        // Получаем анимационный клип по имени
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == name)
                return clip;
        }
        return null;
    }

    private void DisableAnimator()
    {
        animator.enabled = false;
    }
}
