using UnityEngine;
using System.Collections.Generic;

public class ObstacleManager : MonoBehaviour
{
    [Header("Obstacle Settings")]
    public GameObject[] obstaclePrefabs; // 8 loại chướng ngại vật
    public Transform[] spawnPoints; // Các điểm spawn có thể
    public int minObstacles = 4;
    public int maxObstacles = 8;
    
    private List<GameObject> spawnedObstacles = new List<GameObject>();
    
    void Start()
    {
        SpawnRandomObstacles();
    }
    
    void SpawnRandomObstacles()
    {
        // Xóa obstacles cũ nếu có
        ClearObstacles();
        
        // Random số lượng obstacles
        int obstacleCount = Random.Range(minObstacles, maxObstacles + 1);
        
        // Tạo danh sách các vị trí có thể spawn
        List<Transform> availableSpawnPoints = new List<Transform>(spawnPoints);
        
        for (int i = 0; i < obstacleCount && availableSpawnPoints.Count > 0; i++)
        {
            // Random vị trí spawn
            int randomSpawnIndex = Random.Range(0, availableSpawnPoints.Count);
            Transform spawnPoint = availableSpawnPoints[randomSpawnIndex];
            availableSpawnPoints.RemoveAt(randomSpawnIndex);
            
            // Random loại obstacle
            int randomObstacleIndex = Random.Range(0, obstaclePrefabs.Length);
            GameObject obstaclePrefab = obstaclePrefabs[randomObstacleIndex];
            
            // Spawn obstacle
            GameObject obstacle = Instantiate(obstaclePrefab, spawnPoint.position, spawnPoint.rotation);
            spawnedObstacles.Add(obstacle);
        }
    }
    
    void ClearObstacles()
    {
        foreach (GameObject obstacle in spawnedObstacles)
        {
            if (obstacle != null)
                Destroy(obstacle);
        }
        spawnedObstacles.Clear();
    }
    
    public void RespawnObstacles()
    {
        SpawnRandomObstacles();
    }
}