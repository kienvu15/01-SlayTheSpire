using UnityEngine;

public class Spot : MonoBehaviour
{
    public bool isOccupied = false;
    public EnemyView occupant;

    public Vector3 GetPosition()
    {
        return transform.position;
    }

}
