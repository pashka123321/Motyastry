using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class случайный_спрайт : MonoBehaviour
{
    public GameObject targetObject; // Объект, на котором будет меняться спрайт
    public Sprite[] sprites; // Массив спрайтов

    // Start is called before the first frame update
    void Start()
    {
        if (targetObject != null && sprites.Length > 0)
        {
            SpriteRenderer spriteRenderer = targetObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
