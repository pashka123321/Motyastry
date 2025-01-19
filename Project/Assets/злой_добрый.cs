using UnityEngine;

public class TagChanger : MonoBehaviour
{
    public GameObject enemyPrefab;
    private EnemyLogic enemyLogic;

    void Start()
    {
        enemyLogic = enemyPrefab.GetComponent<EnemyLogic>();
        if (enemyLogic != null)
        {
            ChangeTag();
        }
    }

    void Update()
    {
        ChangeTag();
    }

    void ChangeTag()
    {
        if (enemyLogic != null)
        {
            if (enemyLogic.isFriendly)
            {
                enemyPrefab.tag = "Friendly";
            }
            else
            {
                enemyPrefab.tag = "Enemy";
            }
        }
    }
}