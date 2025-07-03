using UnityEngine;
using System.Collections.Generic;

public class CupManager : MonoBehaviour
{
    [Header("Cup Settings")]
    public GameObject cupPrefab;
    public Transform[] cupSpawnPoints; // 3 điểm spawn cho cup
    public int totalCups = 3;
    
    private List<GameObject> spawnedCups = new List<GameObject>();
    private int cupsDestroyed = 0;
    
    void Start()
    {
        SpawnRandomCups();
    }
    
    void SpawnRandomCups()
    {
        // Xóa cups cũ nếu có
        ClearCups();
        cupsDestroyed = 0;
        
        // Tạo danh sách các vị trí có thể spawn
        List<Transform> availableSpawnPoints = new List<Transform>(cupSpawnPoints);
        
        for (int i = 0; i < totalCups && availableSpawnPoints.Count > 0; i++)
        {
            // Random vị trí spawn
            int randomSpawnIndex = Random.Range(0, availableSpawnPoints.Count);
            Transform spawnPoint = availableSpawnPoints[randomSpawnIndex];
            availableSpawnPoints.RemoveAt(randomSpawnIndex);
            
            // Spawn cup
            GameObject cup = Instantiate(cupPrefab, spawnPoint.position, spawnPoint.rotation);
            spawnedCups.Add(cup);
        }
    }
    
    void ClearCups()
    {
        foreach (GameObject cup in spawnedCups)
        {
            if (cup != null)
                Destroy(cup);
        }
        spawnedCups.Clear();
    }
    
    public void OnCupDestroyed()
    {
        cupsDestroyed++;
        Debug.Log($"Cup destroyed! {cupsDestroyed}/{totalCups}");
        
        if (cupsDestroyed >= totalCups)
        {
            // Chuyển scene "Wheel"
            GameManager.Instance.LoadWheelScene();
        }
    }
    
    public void RespawnCups()
    {
        SpawnRandomCups();
    }
}