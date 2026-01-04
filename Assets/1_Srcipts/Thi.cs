using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public Vector3 MoveCharacter(float speed, Vector3 direction)
    {
        return direction.normalized * speed;
    }

    private void Update()
    {
        MoveCharacter(5f, Vector3.right);
    }
}
