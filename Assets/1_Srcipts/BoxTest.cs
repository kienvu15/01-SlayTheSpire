using UnityEngine;

public class BoxTest : MonoBehaviour
{
    int health = 100;

    private void Update()
    {
        if(health <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            health -= 25;
        }
    }
}
