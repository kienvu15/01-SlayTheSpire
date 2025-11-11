using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MapGenerator : MonoBehaviour
{
    [Header("Grid Settings")]
    public int minHeight = 6;
    public int maxHeight = 9;
    public int height;
    public int maxPerRow = 4;
    public float ySpacing = 2.5f;
    public float minXSpacing = 1.5f;
    public float maxXSpacing = 3f;

    [Header("Prefabs & Palette")]
    public GameObject roomPrefab;
    public RoomPalette palette;
    public Transform gridParent;
    public GameObject connectionPrefab;

    private List<List<Room>> rows = new List<List<Room>>();
    private Room bossRoom;

    private int lastRestRow = -999;
    private int lastShopRow = -999;
    private int lastEliteRow = -999;

    public List<GameObject> backMap;

    public Vector2 originButtonAnchoredPos;
    public RectTransform buttonPar;

    void Start()
    {
        originButtonAnchoredPos = buttonPar.anchoredPosition;
        GenerateMap();
    }

    [ContextMenu("Generate Map")]
    public void GenerateMap()
    {
        ClearMap();
        rows.Clear();
        lastRestRow = -999;
        lastShopRow = -999;
        lastEliteRow = -999;

        if (MapUIManager.Instance != null)
            MapUIManager.Instance.battleRoomCount = 0;

        height = Random.Range(minHeight, maxHeight + 1);

        // --- ROW 0: START ---
        var startRoom = CreateRoomAt(0, RoomType.Start);
        //startRoom.gameObject.GetComponent<Canvas>().overrideSorting = true;
        //startRoom.gameObject.GetComponent<Canvas>().sortingLayerName = "UI";
        rows.Add(new List<Room> { startRoom });

        PlayerMapController.Instance.currentRoom = startRoom;
        MapUIManager.Instance.UpdateRoomHighlights();

        // --- ROWS 1 -> height-2 ---
        for (int r = 1; r < height - 1; r++)
        {
            int count = Random.Range(1, maxPerRow + 1);
            var row = CreateRow(r, count, true);
            rows.Add(row);

            if (r == 1)
            {
                foreach (var room in row)
                    startRoom.ConnectTo(room);
            }
            else
            {
                EnsureConnectionsBetween(rows[r - 1], row);
            }
        }

        // --- BOSS ROW ---
        bossRoom = CreateRoomAt(height - 1, RoomType.Boss);
        rows.Add(new List<Room> { bossRoom });

        var penultimate = rows[rows.Count - 2];
        foreach (var r in penultimate) r.ConnectTo(bossRoom);

        DrawAllConnections();
        SetCameraBounds();
    }

    private void SetCameraBounds()
    {
        PanCamera cam = Camera.main.GetComponent<PanCamera>();
        if (cam == null) return;

        // Giới hạn dọc (Y)
        float minY = -1f; // cho phép thấy Start room
        float maxY = (height - 1) * ySpacing + 2f; // boss row + margin

        cam.minBounds = new Vector2(0, minY);
        cam.maxBounds = new Vector2(0, maxY);
    }

    private RoomType GetRandomRoomType(int rowIndex)
    {
        List<RoomType> options = new List<RoomType>();
        options.Add(RoomType.Battle);

        if (rowIndex >= 3) options.Add(RoomType.Event);
        if (rowIndex >= 3 && rowIndex >= lastShopRow +3) options.Add(RoomType.Shop);
        if (rowIndex >= 6 && rowIndex >= lastEliteRow + 5) options.Add(RoomType.Elite);
        if (rowIndex >= 4 && rowIndex >= lastRestRow + 4) options.Add(RoomType.Rest);

        RoomType chosen = options[Random.Range(0, options.Count)];
        if (chosen == RoomType.Rest) lastRestRow = rowIndex;
        if (chosen == RoomType.Shop) lastShopRow = rowIndex;
        if (chosen == RoomType.Elite) lastEliteRow = rowIndex;
        return chosen;
    }

    private List<Room> CreateRow(int rowIndex, int count, bool allowRandom)
    {
        var list = new List<Room>();
        float spacing = Random.Range(minXSpacing, maxXSpacing);

        float startX = -(count - 1) * spacing * 0.5f;
        float baseY = rowIndex * ySpacing;

        for (int i = 0; i < count; i++)
        {
            float x = startX + i * spacing;

            // random Y offset, đảm bảo < ySpacing - 2
            float yOffset = Random.Range(( 2f), (ySpacing - 2f));

            float y = baseY + yOffset;
            Vector3 pos = new Vector3(x, y, 0);

            RoomType type = allowRandom ? GetRandomRoomType(rowIndex) : RoomType.Battle;
            list.Add(CreateRoom(pos, type));
        }

        return list;
    }

    private Room CreateRoomAt(int rowIndex, RoomType type)
    {
        float y = rowIndex * ySpacing;
        Vector3 pos = new Vector3(0, y, 0);
        return CreateRoom(pos, type);
    }

    private Room CreateRoom(Vector3 pos, RoomType type)
    {
        var go = Instantiate(roomPrefab, pos, Quaternion.identity, gridParent);
        var room = go.GetComponent<Room>();
        room.type = type;
        room.gridPos = new Vector2Int(0, (int)(pos.y / ySpacing));
        room.SetCanvas(palette.GetPrefabFor(type));
        return room;
    }

    private void EnsureConnectionsBetween(List<Room> prevRow, List<Room> currentRow)
    {
        foreach (var curr in currentRow)
        {
            if (curr.incoming.Count == 0)
            {
                var from = prevRow[Random.Range(0, prevRow.Count)];
                from.ConnectTo(curr);
            }
        }

        foreach (var prev in prevRow)
        {
            if (prev.outgoing.Count == 0)
            {
                var to = currentRow[Random.Range(0, currentRow.Count)];
                prev.ConnectTo(to);
            }
        }

        int extra = Random.Range(0, Mathf.Max(prevRow.Count, currentRow.Count));
        for (int i = 0; i < extra; i++)
        {
            var p = prevRow[Random.Range(0, prevRow.Count)];
            var c = currentRow[Random.Range(0, currentRow.Count)];
            p.ConnectTo(c);
        }
    }

    private void DrawAllConnections()
    {
        foreach (var row in rows)
        {
            foreach (var room in row)
            {
                foreach (var target in room.outgoing)
                {
                    if (room.transform.position.y < target.transform.position.y)
                    {
                        DrawConnection(room.GetAnchorPos(), target.GetAnchorPos());
                    }
                }
            }
        }
    }

    private void DrawConnection(Vector3 a, Vector3 b)
    {
        var line = Instantiate(connectionPrefab, gridParent);
        var lr = line.GetComponent<LineRenderer>();
        if (lr != null)
        {
            int points = 20; 
            lr.positionCount = points;

            Vector3 mid = (a + b) / 2f;
            Vector3 dir = (b - a).normalized;
            Vector3 perp = new Vector3(-dir.y, dir.x, 0); 
            float curveAmount = Random.Range(-1.5f, 1.5f); 
            mid += perp * curveAmount;

            for (int i = 0; i < points; i++)
            {
                float t = i / (float)(points - 1);
                Vector3 p = Mathf.Pow(1 - t, 2) * a +
                            2 * (1 - t) * t * mid +
                            Mathf.Pow(t, 2) * b;
                lr.SetPosition(i, p);
            }
        }
    }

    private void ClearMap()
    {
        for (int i = gridParent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(gridParent.GetChild(i).gameObject);
        }
    }

    public void PopUp()
    {
        SoundManager.Instance.Play("SelectButton", null, 1);
        foreach (GameObject button in backMap)
        {
            button.SetActive(!button.activeSelf);
        }

        buttonPar.anchoredPosition = new Vector2(originButtonAnchoredPos.x, buttonPar.anchoredPosition.y);
    }

    public void PopUp2()
    {
        SoundManager.Instance.Play("SelectButton", null, 1);
        foreach (GameObject button in backMap)
        {
            button.SetActive(!button.activeSelf);
        }

        if (buttonPar.position.x != 380.33)
        {
            Debug.Log("Move Back");
            buttonPar.anchoredPosition = new Vector2(originButtonAnchoredPos.x, buttonPar.anchoredPosition.y);
        }
        else
        {
            buttonPar.anchoredPosition = new Vector2(380.33f, buttonPar.anchoredPosition.y);
        }

    }
}
