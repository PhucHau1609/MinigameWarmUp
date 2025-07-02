using UnityEngine;

public class Cup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Ball bird = other.GetComponent<Ball>();
        if (bird != null)
        {
            GameManager.Instance.CupScored(bird);
            Destroy(bird.gameObject); // hoặc disable để không bay lại
        }
    }
}

