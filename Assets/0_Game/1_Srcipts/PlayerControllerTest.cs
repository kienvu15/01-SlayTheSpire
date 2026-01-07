using UnityEngine;

public class PlayerControllerTest : MonoBehaviour
{
    public GameObject bulletPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for(int i = 0; i<5; i++)
        {
            Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        }
    }

   
}
