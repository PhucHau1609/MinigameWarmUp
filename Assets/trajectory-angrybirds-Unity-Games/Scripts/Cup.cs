using UnityEngine;

public class Cup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Ball bird = other.GetComponent<Ball>();
        if (bird != null)
        {
            GameManager.Instance.CupScored(bird);
            
            // Thông báo cho CupManager
            CupManager cupManager = FindObjectOfType<CupManager>();
            if (cupManager != null)
            {
                cupManager.OnCupDestroyed();
            }
        }
        Destroy(gameObject);
    }
}