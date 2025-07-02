/*
using UnityEngine;

public class Ball : MonoBehaviour
{
	[HideInInspector] public Rigidbody2D rb;
	[HideInInspector] public CircleCollider2D col;

	[HideInInspector] public Vector3 pos { get { return transform.position; } }

	void Awake ()
	{
		rb = GetComponent<Rigidbody2D> ();
		col = GetComponent<CircleCollider2D> ();
	}

	public void Push (Vector2 force)
	{
		rb.AddForce (force, ForceMode2D.Impulse);
	}

	public void ActivateRb ()
	{
		rb.isKinematic = false;
	}

	public void DesactivateRb ()
	{
		rb.velocity = Vector3.zero;
		rb.angularVelocity = 0f;
		rb.isKinematic = true;
	}
}
*/

using UnityEngine;

public class Ball : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public CircleCollider2D col;

    public int CurrentShotCount { get; private set; } = 0;
    public bool IsUsed { get; private set; } = false;

    public Vector3 pos => transform.position;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CircleCollider2D>();
    }

    public void Push(Vector2 force)
    {
        rb.AddForce(force, ForceMode2D.Impulse);
        CurrentShotCount++;

        if (CurrentShotCount >= 2)
            MarkUsed();
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
    }

    public void ActivateRb() => rb.isKinematic = false;

    public void DesactivateRb() => ResetPhysics();

    private void OnMouseDown()
    {
        if (!IsUsed && GameManager.Instance.CanSelectBird)
        {
            GameManager.Instance.SelectBird(this);
        }
    }
}

