/*
using UnityEngine;

public class GameManager : MonoBehaviour
{
	#region Singleton class: GameManager

	public static GameManager Instance;

	void Awake ()
	{
		if (Instance == null) {
			Instance = this;
		}
	}

	#endregion

	Camera cam;

	public Ball ball;
	public Trajectory trajectory;
	[SerializeField] float pushForce = 4f;

	bool isDragging = false;

	Vector2 startPoint;
	Vector2 endPoint;
	Vector2 direction;
	Vector2 force;
	float distance;

	//---------------------------------------
	void Start ()
	{
		cam = Camera.main;
		ball.DesactivateRb ();
	}

	void Update ()
	{
		if (Input.GetMouseButtonDown (0)) {
			isDragging = true;
			OnDragStart ();
		}
		if (Input.GetMouseButtonUp (0)) {
			isDragging = false;
			OnDragEnd ();
		}

		if (isDragging) {
			OnDrag ();
		}
	}

	//-Drag--------------------------------------
	void OnDragStart ()
	{
		ball.DesactivateRb ();
		startPoint = cam.ScreenToWorldPoint (Input.mousePosition);

		trajectory.Show ();
	}

	void OnDrag ()
	{
		endPoint = cam.ScreenToWorldPoint (Input.mousePosition);
		distance = Vector2.Distance (startPoint, endPoint);
		direction = (startPoint - endPoint).normalized;
		force = direction * distance * pushForce;

		//just for debug
		Debug.DrawLine (startPoint, endPoint);


		trajectory.UpdateDots (ball.pos, force);
	}

	void OnDragEnd ()
	{
		//push the ball
		ball.ActivateRb ();

		ball.Push (force);

		trajectory.Hide ();
	}

}
*/



using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    void Awake() { if (Instance == null) Instance = this; }

    public List<Ball> birds;
    public Trajectory trajectory;
    public float pushForce = 4f;

    private Ball currentBird = null;
    private bool isDragging = false;
    private Vector2 startPoint, endPoint, direction, force;
    private float distance;

    public bool CanSelectBird => true;

    void Start()
    {
        foreach (var bird in birds)
            bird.DesactivateRb();
    }

    void Update()
    {
        if (currentBird == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            OnDragStart();
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            OnDragEnd();
        }

        if (isDragging) OnDrag();
    }

    public void SelectBird(Ball bird)
    {
        if (bird.IsUsed) return;

        if (currentBird != null && currentBird != bird)
        {
            // Nếu chọn chim mới => đánh dấu chim cũ là đã dùng (bỏ lượt 2)
            currentBird.MarkUsed();
            Debug.Log("Bỏ lượt còn lại của chim: " + currentBird.name);
        }

        currentBird = bird;
        Debug.Log("Đã chọn chim: " + bird.name);
    }

    void OnDragStart()
    {
        currentBird.DesactivateRb();
        startPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        trajectory.Show();
    }

    void OnDrag()
    {
        endPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        distance = Vector2.Distance(startPoint, endPoint);
        direction = (startPoint - endPoint).normalized;
        force = direction * distance * pushForce;
        trajectory.UpdateDots(currentBird.pos, force);
    }

    void OnDragEnd()
    {
        currentBird.ActivateRb();
        currentBird.Push(force);
        trajectory.Hide();

        StartCoroutine(WaitForLanding());
    }

    IEnumerator WaitForLanding()
    {
        yield return new WaitForSeconds(2f);

        if (currentBird.IsUsed)
        {
            Debug.Log("Chim đã hết lượt: " + currentBird.name);
            currentBird = null;
        }
        else
        {
            Debug.Log("Chim vẫn còn 1 lượt nữa: " + currentBird.name);
            // Hiện UI nếu muốn, hoặc chờ người chơi tự chọn lại
        }
    }

    public void CupScored(Ball bird)
    {
        Debug.Log($"CUP HIT! Bird: {bird.name}");
        // Tuỳ chỉnh thêm nếu muốn check win tại đây
    }
}

