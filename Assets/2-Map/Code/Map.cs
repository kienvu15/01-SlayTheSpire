///*
//Slay the Spire style Map generator for Unity
//- Single file contains:
//  - RoomType enum
//  - RoomPalette ScriptableObject (map from RoomType -> Canvas prefab)
//  - Room.cs (component for each node)
//  - MapGenerator.cs (main generator)

//How to use:
//1. Create a RoomPalette ScriptableObject asset and assign canvas prefabs for each RoomType.
//2. Create a roomPrefab: a simple GameObject with a RectTransform (or empty), add the Room.cs script and a child to hold the room canvas visuals.
//   - The Room prefab can contain a placeholder Image or Button for hit detection.
//3. Create a Line prefab (optional) that uses LineRenderer or an Image to draw connection lines.
//4. Create an empty GameObject in the scene, attach MapGenerator and assign references (roomPrefab, palette, gridParent, connectionPrefab).
//5. Call GenerateMap() from inspector or Start to create a new map.

//Rules implemented:
//- Map flows vertically from bottom (row 0) to top (row height-1)
//- Each row can have up to maxPerRow rooms
//- Every room in rows 1..height-1 has at least one incoming connection from previous row
//- Every room in previous row has at least one outgoing connection to next row (so player can progress)
//- Topmost room is Boss room (single), and every room in the row below connects to the Boss
//- Room canvas is selected by RoomType from RoomPalette

//Spacing: xSpacing and ySpacing (units) controlled in MapGenerator

//*/

//using System.Collections.Generic;
//using UnityEngine;

//public class Room : MonoBehaviour
//{
//    [Header("Runtime")]
//    public RoomType type;
//    public Vector2Int gridPos; // x = column index, y = row index (0 = bottom)

//    [HideInInspector]
//    public List<Room> outgoing = new List<Room>();
//    [HideInInspector]
//    public List<Room> incoming = new List<Room>();

//    [Header("References")]
//    public Transform canvasHolder; // child transform where canvas prefab will be instantiated

//    // call to instantiate canvas prefab for this room based on palette
//    public void SetCanvas(GameObject canvasPrefab)
//    {
//        if (canvasHolder == null) canvasHolder = transform;
//        if (canvasPrefab == null) return;
//        // destroy existing children in holder
//        for (int i = canvasHolder.childCount - 1; i >= 0; --i) DestroyImmediate(canvasHolder.GetChild(i).gameObject);
//        var go = Instantiate(canvasPrefab, canvasHolder);
//        go.transform.localPosition = Vector3.zero;
//        go.transform.localRotation = Quaternion.identity;
//        go.transform.localScale = Vector3.one;
//    }

//    public void ConnectTo(Room target)
//    {
//        if (target == null || outgoing.Contains(target)) return;
//        outgoing.Add(target);
//        target.incoming.Add(this);
//    }
//}


