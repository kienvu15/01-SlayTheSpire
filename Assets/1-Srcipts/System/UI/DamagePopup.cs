using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    public float moveSpeed = 3.5f;
    public float lifeTime = 0.3f;

    private float timer;
    private Vector3 moveDirection;

    public void Setup(int damageAmount, bool isMiss = false)
    {
        if (isMiss)
            textMesh.text = "MISS";
        else
            textMesh.text = damageAmount.ToString();

        textMesh.color = isMiss ? Color.gray : Color.white;

        float randomX = Random.Range(-0.5f, 0.5f);
        float randomY = Random.Range(0.8f, 1.2f);
        moveDirection = new Vector3(randomX, randomY, 0).normalized;
    }

    private void Update()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            Destroy(gameObject);
        }
    }
}
