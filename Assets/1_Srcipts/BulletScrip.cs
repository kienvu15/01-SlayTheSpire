using UnityEngine;

public class BulletScrip : MonoBehaviour
{
    public Rigidbody2D rb;

    private void Update()
    {
        Vector3 dir = Vector3.right;

        rb.linearVelocity = dir * 10f;
    }
}
