using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Thêm namespace cho TextMeshPro

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Objects")]
    public GameObject ball;
    public Trajectory trajectory;
    public GameObject panelGameOver;
    
    [Header("UI Elements")]
    public TextMeshProUGUI shotCountText; // Text hiển thị số lần bắn
    
    [Header("Spawn Settings")]
    public Transform[] ballSpawnPoints;
    
    [Header("Game Settings")]
    public float pushForce = 4f;
    public string wheelSceneName = "Wheel";
    public string gameplaySceneName = "Gameplay_Thuan";

    private Ball currentBird = null;
    private bool isDragging = false;
    private Vector2 startPoint, endPoint, direction, force;
    private float distance;
    private bool gameOver = false;
    private bool ballIsFlying = false; // Trạng thái bay của ball

    public bool CanSelectBird => !gameOver && currentBird != null && !currentBird.IsUsed && !ballIsFlying;

    void Awake() 
    { 
        if (Instance == null) 
            Instance = this; 
        else
            Destroy(gameObject);
    }

    void Start()
    {
        InitializeGame();
    }

    void InitializeGame()
    {
        gameOver = false;
        ballIsFlying = false;

        if (panelGameOver != null)
            panelGameOver.SetActive(false);

        SpawnBallAtRandomPosition();
        UpdateShotCountUI(); // Cập nhật UI ban đầu
    }

    void SpawnBallAtRandomPosition()
    {
        if (ballSpawnPoints.Length > 0 && ball != null)
        {
            int randomIndex = Random.Range(0, ballSpawnPoints.Length);
            Transform spawnPoint = ballSpawnPoints[randomIndex];

            GameObject ballObj = Instantiate(ball, spawnPoint.position, Quaternion.identity);
            currentBird = ballObj.GetComponent<Ball>();
            
            if (currentBird != null)
            {
                currentBird.ResetBall();
                currentBird.DesactivateRb();
            }

            Debug.Log($"Ball spawned at: {spawnPoint.position}");
        }
        else
        {
            Debug.LogWarning("Missing spawn points or ball prefab!");
        }
    }

    void Update()
    {
        if (gameOver || currentBird == null) return;

        // Chỉ cho phép kéo bắn khi ball không bay
        if (!ballIsFlying)
        {
            if (Input.GetMouseButtonDown(0))
            {
                isDragging = true;
                OnDragStart();
            }

            if (Input.GetMouseButtonUp(0) && isDragging)
            {
                isDragging = false;
                OnDragEnd();
            }

            if (isDragging) OnDrag();
        }
    }

    public void SelectBird(Ball bird)
    {
        if (bird.IsUsed || gameOver || ballIsFlying) return;

        if (currentBird != null && currentBird != bird)
        {
            currentBird.MarkUsed();
            Debug.Log("Bỏ lượt còn lại của chim: " + currentBird.name);
        }

        currentBird = bird;
        Debug.Log("Đã chọn chim: " + bird.name);
    }

    void OnDragStart()
    {
        if (currentBird == null || !currentBird.CanShoot()) return;
        
        currentBird.DesactivateRb();
        startPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        trajectory.Show();
    }

    void OnDrag()
    {
        if (currentBird == null || !currentBird.CanShoot()) return;
        
        endPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        distance = Vector2.Distance(startPoint, endPoint);
        direction = (startPoint - endPoint).normalized;
        force = direction * distance * pushForce;
        trajectory.UpdateDots(currentBird.pos, force);
    }

    void OnDragEnd()
    {
        if (currentBird == null || !currentBird.CanShoot()) return;
        
        currentBird.ActivateRb();
        currentBird.Push(force);
        trajectory.Hide();
    }

    // Được gọi khi ball bắt đầu bay
    public void OnBallStartFlying()
    {
        ballIsFlying = true;
        Debug.Log("Ball started flying - Input disabled");
    }

    // Được gọi khi ball dừng bay
    public void OnBallStoppedFlying()
    {
        ballIsFlying = false;
        Debug.Log("Ball stopped flying - Input enabled");
    }

    // Cập nhật UI hiển thị số lần bắn
    public void UpdateShotCountUI()
    {
        if (shotCountText != null && currentBird != null)
        {
            int remaining = currentBird.maxShots - currentBird.CurrentShotCount;
            shotCountText.text = $"{remaining}/{currentBird.maxShots}";
        }
    }

    public void OnAllShotsUsed()
    {
        Debug.Log("Đã hết 5 lượt bắn!");
        ShowGameOver();
    }

    public void ShowGameOver()
    {
        gameOver = true;
        if (panelGameOver != null)
        {
            panelGameOver.SetActive(true);
        }
        Time.timeScale = 0f;
        Debug.Log("Game Over!");
    }

    public void CupScored(Ball bird)
    {
        Debug.Log($"CUP HIT! Bird: {bird.name}");
    }

    public void LoadWheelScene()
    {
        Debug.Log("Loading Wheel Scene...");
        SceneManager.LoadScene(wheelSceneName);
    }

    public void RestartGame()
    {
        Debug.Log("Restarting game...");
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameplaySceneName);
    }
}