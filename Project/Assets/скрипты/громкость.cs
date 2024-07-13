using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    public Slider volumeSlider; // Ссылка на UI Slider для громкости

    private const string VolumePlayerPrefsKey = "VolumeLevel"; // Ключ для сохранения громкости в PlayerPrefs
    private const float DefaultVolume = 0.5f; // Значение громкости по умолчанию (50%)

    void Awake()
    {
        // Проверяем, есть ли сохранённое значение громкости в PlayerPrefs
        if (PlayerPrefs.HasKey(VolumePlayerPrefsKey))
        {
            float savedVolume = PlayerPrefs.GetFloat(VolumePlayerPrefsKey);
            volumeSlider.value = savedVolume; // Устанавливаем положение ползунка из сохранённого значения
            SetVolume(savedVolume); // Применяем громкость ко всем AudioSource
        }
        else
        {
            // Если нет сохранённого значения, используем значение по умолчанию
            volumeSlider.value = DefaultVolume; // Устанавливаем громкость на 50%
            SetVolume(DefaultVolume);
        }
    }

    void Start()
    {
        // Добавляем обработчик события изменения значения ползунка
        volumeSlider.onValueChanged.AddListener(delegate { OnVolumeChanged(); });
    }

    // Метод для применения текущего значения громкости ко всем AudioSource
    void SetVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    // Вызывается при изменении значения ползунка
    void OnVolumeChanged()
    {
        float currentVolume = volumeSlider.value;
        SetVolume(currentVolume); // Применяем новую громкость

        // Сохраняем текущее значение громкости в PlayerPrefs
        PlayerPrefs.SetFloat(VolumePlayerPrefsKey, currentVolume);
        PlayerPrefs.Save(); // Сохраняем изменения
    }

    // Освобождаем ресурсы
    private void OnDestroy()
    {
        // Удаляем обработчик события изменения значения ползунка
        volumeSlider.onValueChanged.RemoveAllListeners();
    }
}
