using UnityEngine;

public class Ball : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public CircleCollider2D col;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    public int CurrentShotCount { get; private set; } = 0;
    public bool IsUsed { get; private set; } = false;
    public bool IsFlying { get; private set; } = false; // Trạng thái bay
    public int maxShots = 5; // Tối đa 5 lượt bắn
    private bool isLastShot = false;
    private bool hasStartedFlying = false;

    [Header("Flying Settings")]
    public float stoppingVelocity = 0.1f; // Ngưỡng vận tốc để coi như đã dừng

    public Vector3 pos => transform.position;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CircleCollider2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        if (rb == null || anim == null) return;

        bool isCurrentlyFlying = rb.velocity.magnitude > stoppingVelocity;
        
        // Cập nhật flip theo hướng bay
        if (isCurrentlyFlying && spriteRenderer != null)
        {
            // Nếu vận tốc x âm (bay sang trái) thì flip
            if (rb.velocity.x < 0)
            {
                spriteRenderer.flipX = true;
            }
            // Nếu vận tốc x dương (bay sang phải) thì không flip
            else if (rb.velocity.x > 0)
            {
                spriteRenderer.flipX = false;
            }
        }
        
        // Cập nhật trạng thái bay
        if (isCurrentlyFlying != IsFlying)
        {
            IsFlying = isCurrentlyFlying;
            anim.SetBool("isFlying", IsFlying);
            
            // Thông báo cho GameManager về trạng thái bay
            if (IsFlying)
            {
                GameManager.Instance.OnBallStartFlying();
            }
            else
            {
                GameManager.Instance.OnBallStoppedFlying();
            }
        }

        // Nếu đây là lượt bắn cuối
        if (isLastShot)
        {
            // Kiểm tra xem bóng đã bắt đầu bay chưa
            if (IsFlying && !hasStartedFlying)
            {
                hasStartedFlying = true;
                Debug.Log("Bóng bắt đầu bay ở lượt cuối!");
            }
            
            // Chỉ kiểm tra dừng khi bóng đã từng bay
            if (hasStartedFlying && !IsFlying)
            {
                Debug.Log("Bóng đã dừng sau lượt bắn cuối!");
                isLastShot = false;
                hasStartedFlying = false;
                MarkUsed();
                GameManager.Instance.OnAllShotsUsed();
            }
        }
    }

    public void Push(Vector2 force)
    {
        rb.AddForce(force, ForceMode2D.Impulse);
        CurrentShotCount++;
        
        Debug.Log($"Shot {CurrentShotCount}/{maxShots}");
        
        // Cập nhật UI ngay lập tức
        GameManager.Instance.UpdateShotCountUI();

        if (CurrentShotCount >= maxShots)
        {
            isLastShot = true;
            hasStartedFlying = false;
            Debug.Log("Đây là lượt bắn cuối cùng!");
        }
    }

    public void MarkUsed()
    {
        IsUsed = true;
    }

    public void ResetPhysics()
    {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.isKinematic = true;
        IsFlying = false;
    }

    public void ActivateRb() => rb.isKinematic = false;

    public void DesactivateRb() => ResetPhysics();
    
    public void ResetBall()
    {
        CurrentShotCount = 0;
        IsUsed = false;
        IsFlying = false;
        isLastShot = false;
        hasStartedFlying = false;
        
        // Reset flip về trạng thái mặc định (hướng phải)
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = false;
        }
        
        ResetPhysics();
    }

    // Kiểm tra xem có thể bắn hay không
    public bool CanShoot()
    {
        return !IsUsed && !IsFlying && CurrentShotCount < maxShots;
    }

    private void OnMouseDown()
    {
        if (CanShoot() && GameManager.Instance.CanSelectBird)
        {
            GameManager.Instance.SelectBird(this);
        }
    }
}