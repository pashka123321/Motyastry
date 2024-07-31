using System.Linq;
using UnityEngine;

public class Marchritizator : MonoBehaviour
{
    public Transform[] spawnPoints;    // ����� ������ ���������
    public bool[] activeSP;

    private int i = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<MovementController>() == null) return;

        if (spawnPoints == null || spawnPoints.Length == 0 || activeSP == null || activeSP.Length == 0)
        {
            return; // ������������� ���������� ���� ����� ������ ��� �������� ����� �� ������
        }

        int count = activeSP.Where(c => c).Count();

        if (count == 0)
        {
            return; // ���� ��� �������� ����� ������, �������
        }

        if (i >= spawnPoints.Length)
        {
            i = 0; // ���������� ������ ���� �� ������� �� ������� ������� ����� ������
        }

        while (activeSP[i] == false)
        {
            i++;
            if (i >= spawnPoints.Length)
            {
                i = 0; // ���������� ������ ���� �� ������� �� ������� ������� ����� ������
            }
        }

        Instantiate(collision.gameObject, spawnPoints[i].position, Quaternion.identity);

        Destroy(collision.gameObject);
        i++;
    }

    public void ActivateSpawnPoint(int spIndex)
    {
        if (spIndex >= 0 && spIndex < activeSP.Length)
        {
            activeSP[spIndex] = true;
        }
    }

    public void DeactivateSpawnPoint(int spIndex)
    {
        if (spIndex >= 0 && spIndex < activeSP.Length)
        {
            activeSP[spIndex] = false;
        }
    }
}
