using UnityEngine;

public class CharacterSwitcher : MonoBehaviour
{
    public GameObject defaultCharacter;   // Объект дефолтного персонажа
    public GameObject modifiedCharacter;  // Объект модифицированного персонажа
    public Vector3 respawnPosition;       // Координаты для респавна
    public CameraFollow cameraFollow;     // Ссылка на скрипт CameraFollow

    private bool isUsingDefaultCharacter = true;

    void Start()
    {
        // Изначально выключаем оба персонажа
        defaultCharacter.SetActive(false);
        modifiedCharacter.SetActive(false);

        // Включаем начального персонажа и устанавливаем цель для камеры
        SwitchCharacter();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            // Переключение персонажа при нажатии F1
            isUsingDefaultCharacter = !isUsingDefaultCharacter;
            SwitchCharacter();
        }
    }

    void SwitchCharacter()
    {
        if (isUsingDefaultCharacter)
        {
            // Деактивация модифицированного персонажа и активация дефолтного
            modifiedCharacter.SetActive(false);
            defaultCharacter.transform.position = respawnPosition;
            defaultCharacter.SetActive(true);
            cameraFollow.SetTarget(defaultCharacter.transform);
        }
        else
        {
            // Деактивация дефолтного персонажа и активация модифицированного
            defaultCharacter.SetActive(false);
            modifiedCharacter.transform.position = respawnPosition;
            modifiedCharacter.SetActive(true);
            cameraFollow.SetTarget(modifiedCharacter.transform);
        }
    }
}
