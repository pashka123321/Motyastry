 using UnityEngine;

public class PlayerModelSwitcher : MonoBehaviour
{
    // Переменные для хранения моделей персонажей
    public GameObject playerModel1; // Первая модель
    public GameObject playerModel2; // Вторая модель

    private GameObject currentPlayerModel; // Текущая активная модель

    void Start()
    {
        // Изначально вторая модель должна быть скрыта
        if (playerModel2 != null)
        {
            playerModel2.SetActive(false);
        }

        // Используем первую модель как текущую
        currentPlayerModel = playerModel1;
        currentPlayerModel.SetActive(true);

        // Помещаем игрока в иерархию текущей модели
        transform.parent = currentPlayerModel.transform;
        transform.localPosition = Vector3.zero;
    }

    void Update()
    {
        // Переключение моделей по нажатию клавиши F1
        if (Input.GetKeyDown(KeyCode.F1))
        {
            SwitchPlayerModel();
        }
    }

    void SwitchPlayerModel()
    {
        // Определяем текущую и новую модели для переключения
        GameObject newPlayerModel = (currentPlayerModel == playerModel1) ? playerModel2 : playerModel1;

        // Перемещаем игрока в иерархию новой модели
        transform.parent = newPlayerModel.transform;
        transform.localPosition = Vector3.zero;

        // Выключаем текущую модель и включаем новую модель
        currentPlayerModel.SetActive(false);
        newPlayerModel.SetActive(true);

        // Обновляем текущую модель
        currentPlayerModel = newPlayerModel;
    }
}