using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Core : MonoBehaviour
{
    public int maxHealth = 1000;
    public int currentHealth;
    public Sprite normalSprite;  // Спрайт для нормального состояния блока.
    public Sprite damagedSprite; // Спрайт для поврежденного состояния блока.
    public GameObject gameOverPrefab; // Префаб текста Game Over.
    public GameObject blockToDestroy; // Указанный объект, который будет удалён.
    private SpriteRenderer spriteRenderer;
    private bool isGameOver = false; // Флаг для предотвращения повторных вызовов GameOver.

    private void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = blockToDestroy.GetComponent<SpriteRenderer>(); // Получаем спрайт указанного блока.
    }

    private void Update()
    {
        // Обновляем состояние спрайта блока, если здоровье меньше половины.
        if (currentHealth <= maxHealth / 2 && spriteRenderer.sprite != damagedSprite)
        {
            spriteRenderer.sprite = damagedSprite;
        }

        // Проверка на нулевое здоровье и вызов GameOver один раз.
        if (currentHealth <= 0 && !isGameOver)
        {
            GameOver();
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Ограничиваем здоровье нулем.
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
    }

    private void GameOver()
    {
        isGameOver = true;  // Ставим флаг, чтобы не вызывать GameOver повторно.
        
        // Создаем префаб текста Game Over в центре экрана.
        CreateGameOverText();

        // Отключаем взаимодействие с ядром, но не удаляем его сразу.
        DisableCoreComponents();

        // Переходим на сцену через 5 секунд.
        Invoke("LoadMenuScene", 5f);  
    }

    private void CreateGameOverText()
    {
        // Получаем центр экрана в мировых координатах.
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Vector3 worldCenter = Camera.main.ScreenToWorldPoint(screenCenter);
        worldCenter.z = 0;  // Обнуляем Z-координату для 2D.

        // Создаем объект текста Game Over в центре экрана.
        Instantiate(gameOverPrefab, worldCenter, Quaternion.identity);
    }

    private void LoadMenuScene()
    {
        SceneManager.LoadScene("menu"); // Загрузка сцены меню.
    }

    private void DisableCoreComponents()
    {
        // Отключаем основные компоненты ядра, чтобы оно больше не реагировало на действия.
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false; // Отключаем отображение спрайта.
        }

        // Можно отключить другие компоненты, например, физику или обработку ввода, если они есть.
        // Например: GetComponent<Collider2D>().enabled = false;
    }

    private void DestroyBlock()
    {
        // Удаляем конкретный указанный блок.
        if (blockToDestroy != null)
        {
            Destroy(blockToDestroy); // Удаляем выбранный блок.
        }
    }
}
