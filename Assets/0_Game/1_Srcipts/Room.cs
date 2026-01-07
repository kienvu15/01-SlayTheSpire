using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [Header("Runtime")]
    public RoomType type;
    public Vector2Int gridPos; 

    [HideInInspector]
    public List<Room> outgoing = new List<Room>();
    [HideInInspector]
    public List<Room> incoming = new List<Room>();

    [Header("References")]
    public Transform canvasHolder; 

    [Header("Connection Anchor")]
    public Transform connectionAnchor; 
    public bool visited = false; 


    public void SetCanvas(GameObject canvasPrefab)
    {
        if (canvasHolder == null) canvasHolder = transform;
        if (canvasPrefab == null) return;
        // destroy existing children in holder
        for (int i = canvasHolder.childCount - 1; i >= 0; --i) DestroyImmediate(canvasHolder.GetChild(i).gameObject);
        var go = Instantiate(canvasPrefab, canvasHolder);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;
    }

    public Vector3 GetAnchorPos()
    {
        return connectionAnchor != null ? connectionAnchor.position : transform.position;
    }

    public void ConnectTo(Room target)
    {
        if (target == null || outgoing.Contains(target)) return;
        outgoing.Add(target);
        target.incoming.Add(this);
    }
}
