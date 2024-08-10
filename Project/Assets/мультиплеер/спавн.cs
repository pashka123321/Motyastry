using UnityEngine;
using Mirror;

public class MyNetworkManagerm : NetworkManager
{
    public Transform[] spawnPoints; // Массив точек спавна

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        // Выбираем случайную точку спавна
        Transform start = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Создаем игрока на выбранной точке спавна
        GameObject player = Instantiate(playerPrefab, start.position, start.rotation);

        // Добавляем игрока на сервер
        NetworkServer.AddPlayerForConnection(conn, player);
    }
}