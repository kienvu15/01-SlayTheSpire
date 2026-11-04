using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapGenerator : MonoBehaviour
{
    [Header("Map Settings")]
    public int minHeight = 6;
    public int maxHeight = 9;
    public int height;
    public int maxPerRow = 4;

    [Header("Horizontal Layout")]
    public float columnSpacing = 400f;
    public float minRoomSpacing = 130f;
    public float maxRoomSpacing = 180f;
    public float horizontalPadding = 120f;
    public float verticalPadding = 60f;

    [Header("Prefabs & Palette")]
    public GameObject roomPrefab;
    //public RoomPalette palette;
    public GameObject connectionPrefab;

    [Header("UI References")]
    [SerializeField] private RectTransform mapViewport;
    [SerializeField] private RectTransform mapContent;
    [SerializeField] private ScrollRect scrollRect;
    //[SerializeField] private Slider mapSlider;

    private List<List<Room>> rows = new List<List<Room>>();
    private Room bossRoom;

    private int lastRestRow = -999;
    private int lastShopRow = -999;
    private int lastEliteRow = -999;

    private float roomWidth = 150f;

    private void OnEnable()
    {
        if (scrollRect != null)
            scrollRect.onValueChanged.AddListener(OnScrollChanged);

        //if (mapSlider != null)
            //mapSlider.onValueChanged.AddListener(OnSliderChanged);
    }

    private void OnDisable()
    {
        if (scrollRect != null)
            scrollRect.onValueChanged.RemoveListener(OnScrollChanged);

        //if (mapSlider != null)
            //mapSlider.onValueChanged.RemoveListener(OnSliderChanged);
    }

    private void Start()
    {
        GenerateMap();
    }

    [ContextMenu("Generate Map")]
    public void GenerateMap()
    {
        if (mapContent == null || mapViewport == null)
        {
            Debug.LogError("MapViewport or MapContent is missing!");
            return;
        }

        ClearMap();
        rows.Clear();

        lastRestRow = -999;
        lastShopRow = -999;
        lastEliteRow = -999;

        if (MapUIManager.Instance != null)
            MapUIManager.Instance.battleRoomCount = 0;

        if (roomPrefab.GetComponent<RectTransform>() != null)
        {
            float prefabWidth = roomPrefab.GetComponent<RectTransform>().rect.width;
            if (prefabWidth > 0)
                roomWidth = prefabWidth;
        }

        height = Random.Range(minHeight, maxHeight + 1);

        // Calculate content width
        float contentWidth = horizontalPadding + roomWidth + (height - 1) * columnSpacing + 50f;
        // Neo trái, stretch dọc theo Viewport
        mapContent.anchorMin = new Vector2(0f, 0f);
        mapContent.anchorMax = new Vector2(0f, 1f);
        mapContent.pivot = new Vector2(0f, 0.5f);

        // x = chiều rộng thật, y = 0 nghĩa là cao đúng bằng Viewport
        mapContent.sizeDelta = new Vector2(contentWidth, 0f);
        mapContent.anchoredPosition = Vector2.zero;


        // Reset scroll position
        scrollRect.horizontalNormalizedPosition = 0f;
        // START ROOM
        var startRoom = CreateRoomAt(0, RoomType.Start);

        var startUI = startRoom.GetComponentInChildren<RoomUI>();

        if (startUI != null)
        {
            startUI.isStartRoom = true;
            //startUI.SetAsVisited();
        }

        rows.Add(new List<Room> { startRoom });

        if (PlayerMapController.Instance != null)
        {
            PlayerMapController.Instance.currentRoom = startRoom;
        }

        if (MapUIManager.Instance != null)
            MapUIManager.Instance.UpdateRoomHighlights();

        // NORMAL ROWS
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

        // BOSS ROOM
        bossRoom = CreateRoomAt(height - 1, RoomType.Boss);

        rows.Add(new List<Room> { bossRoom });

        var penultimate = rows[rows.Count - 2];

        foreach (var room in penultimate)
            room.ConnectTo(bossRoom);

        // DRAW CONNECTIONS
        DrawAllConnections();

        if (MapUIManager.Instance != null)
        {
            MapUIManager.Instance.StartCoroutine(
                MapUIManager.Instance.UpdateHighlightsNextFrame()
            );
        }

        // Refresh scroll position
        Canvas.ForceUpdateCanvases();

        scrollRect.horizontalNormalizedPosition = 0f;
        UpdateSlider(0f);
    }

    // =====================================
    // ROOM GENERATION
    // =====================================

    private List<Room> CreateRow(int rowIndex, int count, bool allowRandom)
    {
        var list = new List<Room>();

        float availableHeight =
            mapViewport.rect.height - verticalPadding * 2f;

        float spacing = 0f;

        if (count > 1)
        {
            spacing = Random.Range(minRoomSpacing, maxRoomSpacing);

            spacing = Mathf.Min(
                spacing,
                availableHeight / (count - 1)
            );
        }

        float startY = (count - 1) * spacing * 0.5f;

        float x = horizontalPadding + roomWidth * 0.5f
                  + rowIndex * columnSpacing;

        for (int i = 0; i < count; i++)
        {
            float y = startY - i * spacing;

            Vector2 position = new Vector2(x, y);

            RoomType type = allowRandom
                ? GetRandomRoomType(rowIndex)
                : RoomType.Battle;

            Vector2Int gridPosition = new Vector2Int(rowIndex, i);

            list.Add(CreateRoom(position, type, gridPosition));
        }

        return list;
    }

    private Room CreateRoomAt(int rowIndex, RoomType type)
    {
        float x = horizontalPadding + roomWidth * 0.5f
                  + rowIndex * columnSpacing;

        Vector2 position = new Vector2(x, 0f);

        Vector2Int gridPosition = new Vector2Int(rowIndex, 0);

        return CreateRoom(position, type, gridPosition);
    }

    private Room CreateRoom(
        Vector2 position,
        RoomType type,
        Vector2Int gridPosition)
    {
        GameObject go = Instantiate(roomPrefab, mapContent);

        RectTransform rect = go.GetComponent<RectTransform>();

        rect.anchorMin = new Vector2(0f, 0.5f);
        rect.anchorMax = new Vector2(0f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);

        rect.anchoredPosition = position;
        rect.localScale = Vector3.one;

        Room room = go.GetComponent<Room>();

        room.type = type;
        room.gridPos = gridPosition;

        //room.SetCanvas(palette.GetPrefabFor(type));
        room.SetRoomType(type);
        return room;
    }

    // ROOM TYPES
    private RoomType GetRandomRoomType(int rowIndex)
    {
        List<RoomType> options = new List<RoomType>();

        options.Add(RoomType.Battle);

        if (rowIndex >= 3)
            options.Add(RoomType.Event);

        if (rowIndex >= 3 && rowIndex >= lastShopRow + 3)
            options.Add(RoomType.Shop);

        if (rowIndex >= 6 && rowIndex >= lastEliteRow + 5)
            options.Add(RoomType.Elite);

        if (rowIndex >= 4 && rowIndex >= lastRestRow + 4)
            options.Add(RoomType.Rest);

        RoomType chosen = options[Random.Range(0, options.Count)];

        if (chosen == RoomType.Rest)
            lastRestRow = rowIndex;

        if (chosen == RoomType.Shop)
            lastShopRow = rowIndex;

        if (chosen == RoomType.Elite)
            lastEliteRow = rowIndex;

        return chosen;
    }

    // CONNECTION LOGIC
    private void EnsureConnectionsBetween(
        List<Room> prevRow,
        List<Room> currentRow)
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

        int extra = Random.Range(
            0,
            Mathf.Max(prevRow.Count, currentRow.Count)
        );

        for (int i = 0; i < extra; i++)
        {
            var p = prevRow[Random.Range(0, prevRow.Count)];
            var c = currentRow[Random.Range(0, currentRow.Count)];

            p.ConnectTo(c);
        }
    }

    // DRAW UI CONNECTIONS
    private void DrawAllConnections()
    {
        foreach (var row in rows)
        {
            foreach (var room in row)
            {
                foreach (var target in room.outgoing)
                {
                    DrawConnection(room, target);
                }
            }
        }
    }

    private void DrawConnection(Room from, Room to)
    {
        RectTransform fromRect =
            from.GetComponent<RectTransform>();

        RectTransform toRect =
            to.GetComponent<RectTransform>();

        if (fromRect == null || toRect == null)
            return;

        GameObject lineObject = Instantiate(
            connectionPrefab,
            mapContent
        );

        RectTransform lineRect =
            lineObject.GetComponent<RectTransform>();

        lineRect.anchorMin = Vector2.zero;
        lineRect.anchorMax = Vector2.one;
        lineRect.pivot = new Vector2(0.5f, 0.5f);

        lineRect.offsetMin = Vector2.zero;
        lineRect.offsetMax = Vector2.zero;

        // Keep connection behind rooms
        lineObject.transform.SetAsFirstSibling();

        UIBezierConnection line =
            lineObject.GetComponent<UIBezierConnection>();

        if (line == null)
        {
            Debug.LogError("Connection prefab is missing UIBezierConnection!");
            return;
        }

        // Convert room positions to connection local space
        Vector2 start = fromRect.anchoredPosition
                        - new Vector2(mapContent.rect.width * 0.5f, 0f);

        Vector2 end = toRect.anchoredPosition
                      - new Vector2(mapContent.rect.width * 0.5f, 0f);

        float curve = Random.Range(-40f, 40f);

        line.SetPoints(start, end, curve);
    }

    // SLIDER / SCROLL SYNC
    private void OnScrollChanged(Vector2 position)
    {
        UpdateSlider(position.x);
    }

    private void UpdateSlider(float value)
    {
        //if (mapSlider != null)
            //mapSlider.SetValueWithoutNotify(value);
    }

    // CLEAR MAP
    private void ClearMap()
    {
        if (mapContent == null)
            return;

        for (int i = mapContent.childCount - 1; i >= 0; i--)
        {
            GameObject child = mapContent.GetChild(i).gameObject;

            if (Application.isPlaying)
                Destroy(child);
            else
                DestroyImmediate(child);
        }
    }
}


//using System.Collections.Generic;
//using UnityEngine;

//public class MapGenerator : MonoBehaviour
//{
//    [Header("Grid Settings")]
//    public int minHeight = 6;
//    public int maxHeight = 9;
//    public int height;
//    public int maxPerRow = 4;
//    public float ySpacing = 2.5f;
//    public float minXSpacing = 1.5f;
//    public float maxXSpacing = 3f;

//    [Header("Prefabs & Palette")]
//    public GameObject roomPrefab;
//    public RoomPalette palette;
//    public Transform gridParent;
//    public GameObject connectionPrefab;
//    [SerializeField] private PanCamera panCamera;

//    private List<List<Room>> rows = new List<List<Room>>();
//    private Room bossRoom;

//    private int lastRestRow = -999;
//    private int lastShopRow = -999;
//    private int lastEliteRow = -999;

//    private void OnEnable()
//    {
//        if (panCamera != null)
//        {
//            panCamera.enabled = true;
//        }
//    }

//    void OnDisable()
//    {
//        if (panCamera != null)
//        {
//            panCamera.enabled = false;
//        }
//    }

//    void Start()
//    {
//        GenerateMap();
//    }

//    [ContextMenu("Generate Map")]
//    public void GenerateMap()
//    {
//        ClearMap();
//        rows.Clear();
//        lastRestRow = -999;
//        lastShopRow = -999;
//        lastEliteRow = -999;

//        if (MapUIManager.Instance != null)
//            MapUIManager.Instance.battleRoomCount = 0;

//        height = Random.Range(minHeight, maxHeight + 1);

//        // --- ROW 0: START ---
//        var startRoom = CreateRoomAt(0, RoomType.Start);
//        var startRoomUI = startRoom.GetComponentInChildren<RoomUI>();
//        if (startRoomUI != null)
//        {
//            startRoomUI.isStartRoom = true;
//            startRoomUI.SetAsVisited(); 
//        }

//        rows.Add(new List<Room> { startRoom });

//        PlayerMapController.Instance.currentRoom = startRoom;
//        MapUIManager.Instance.UpdateRoomHighlights();

//        // --- ROWS 1 -> height-2 ---
//        for (int r = 1; r < height - 1; r++)
//        {
//            int count = Random.Range(1, maxPerRow + 1);
//            var row = CreateRow(r, count, true);
//            rows.Add(row);

//            if (r == 1)
//            {
//                foreach (var room in row)
//                    startRoom.ConnectTo(room);
//            }
//            else
//            {
//                EnsureConnectionsBetween(rows[r - 1], row);
//            }
//        }

//        // --- BOSS ROW ---
//        bossRoom = CreateRoomAt(height - 1, RoomType.Boss);
//        rows.Add(new List<Room> { bossRoom });

//        var penultimate = rows[rows.Count - 2];
//        foreach (var r in penultimate) r.ConnectTo(bossRoom);

//        MapUIManager.Instance.StartCoroutine(MapUIManager.Instance.UpdateHighlightsNextFrame());

//        DrawAllConnections();
//        SetCameraBounds();
//    }

//    private void SetCameraBounds()
//    {
//        PanCamera cam = Camera.main.GetComponent<PanCamera>();
//        if (cam == null) return;

//        float minY = -1f; 
//        float maxY = (height - 1) * ySpacing + 2f; 

//        cam.minBounds = new Vector2(0, minY);
//        cam.maxBounds = new Vector2(0, maxY);
//    }

//    private RoomType GetRandomRoomType(int rowIndex)
//    {
//        List<RoomType> options = new List<RoomType>();
//        options.Add(RoomType.Battle);

//        if (rowIndex >= 3) options.Add(RoomType.Event);
//        if (rowIndex >= 3 && rowIndex >= lastShopRow +3) options.Add(RoomType.Shop);
//        if (rowIndex >= 6 && rowIndex >= lastEliteRow + 5) options.Add(RoomType.Elite);
//        if (rowIndex >= 4 && rowIndex >= lastRestRow + 4) options.Add(RoomType.Rest);

//        RoomType chosen = options[Random.Range(0, options.Count)];
//        if (chosen == RoomType.Rest) lastRestRow = rowIndex;
//        if (chosen == RoomType.Shop) lastShopRow = rowIndex;
//        if (chosen == RoomType.Elite) lastEliteRow = rowIndex;
//        return chosen;
//    }

//    private List<Room> CreateRow(int rowIndex, int count, bool allowRandom)
//    {
//        var list = new List<Room>();

//        float spacing = Random.Range(minXSpacing, maxXSpacing);

//        float startY = (count - 1) * spacing * 0.5f;
//        float baseX = rowIndex * ySpacing;

//        for (int i = 0; i < count; i++)
//        {
//            float y = startY - i * spacing;

//            float xOffset = Random.Range(0.5f, ySpacing - 0.5f);
//            float x = baseX + xOffset;

//            Vector3 pos = new Vector3(x, y, 0);

//            RoomType type = allowRandom ? GetRandomRoomType(rowIndex) : RoomType.Battle;

//            list.Add(CreateRoom(pos, type));
//        }

//        return list;
//    }

//    //private List<Room> CreateRow(int rowIndex, int count, bool allowRandom)
//    //{
//    //    var list = new List<Room>();
//    //    float spacing = Random.Range(minXSpacing, maxXSpacing);

//    //    float startX = -(count - 1) * spacing * 0.5f;
//    //    float baseY = rowIndex * ySpacing;

//    //    for (int i = 0; i < count; i++)
//    //    {
//    //        float x = startX + i * spacing;

//    //        float yOffset = Random.Range(( 2f), (ySpacing - 2f));

//    //        float y = baseY + yOffset;
//    //        Vector3 pos = new Vector3(x, y, 0);

//    //        RoomType type = allowRandom ? GetRandomRoomType(rowIndex) : RoomType.Battle;
//    //        list.Add(CreateRoom(pos, type));
//    //    }

//    //    return list;
//    //}

//    private Room CreateRoomAt(int rowIndex, RoomType type)
//    {
//        float x = rowIndex * ySpacing;

//        Vector3 pos = new Vector3(x, 0, 0);

//        return CreateRoom(pos, type);
//    }

//    //private Room CreateRoomAt(int rowIndex, RoomType type)
//    //{
//    //    float y = rowIndex * ySpacing;
//    //    Vector3 pos = new Vector3(0, y, 0);
//    //    return CreateRoom(pos, type);
//    //}

//    private Room CreateRoom(Vector3 pos, RoomType type)
//    {
//        var go = Instantiate(roomPrefab, pos, Quaternion.identity, gridParent);
//        var room = go.GetComponent<Room>();
//        room.type = type;
//        room.gridPos = new Vector2Int(
//            (int)(pos.x / ySpacing),
//            0
//        ); room.SetCanvas(palette.GetPrefabFor(type));
//        return room;
//    }

//    private void EnsureConnectionsBetween(List<Room> prevRow, List<Room> currentRow)
//    {
//        foreach (var curr in currentRow)
//        {
//            if (curr.incoming.Count == 0)
//            {
//                var from = prevRow[Random.Range(0, prevRow.Count)];
//                from.ConnectTo(curr);
//            }
//        }

//        foreach (var prev in prevRow)
//        {
//            if (prev.outgoing.Count == 0)
//            {
//                var to = currentRow[Random.Range(0, currentRow.Count)];
//                prev.ConnectTo(to);
//            }
//        }

//        int extra = Random.Range(0, Mathf.Max(prevRow.Count, currentRow.Count));
//        for (int i = 0; i < extra; i++)
//        {
//            var p = prevRow[Random.Range(0, prevRow.Count)];
//            var c = currentRow[Random.Range(0, currentRow.Count)];
//            p.ConnectTo(c);
//        }
//    }

//    private void DrawAllConnections()
//    {
//        foreach (var row in rows)
//        {
//            foreach (var room in row)
//            {
//                foreach (var target in room.outgoing)
//                {
//                    if (room.transform.position.y < target.transform.position.y)
//                    {
//                        DrawConnection(room.GetAnchorPos(), target.GetAnchorPos());
//                    }
//                }
//            }
//        }
//    }

//    private void DrawConnection(Vector3 a, Vector3 b)
//    {
//        var line = Instantiate(connectionPrefab, gridParent);
//        var lr = line.GetComponent<LineRenderer>();
//        if (lr != null)
//        {
//            int points = 20; 
//            lr.positionCount = points;

//            Vector3 mid = (a + b) / 2f;
//            Vector3 dir = (b - a).normalized;
//            Vector3 perp = new Vector3(-dir.y, dir.x, 0); 
//            float curveAmount = Random.Range(-1.5f, 1.5f); 
//            mid += perp * curveAmount;

//            for (int i = 0; i < points; i++)
//            {
//                float t = i / (float)(points - 1);
//                Vector3 p = Mathf.Pow(1 - t, 2) * a +
//                            2 * (1 - t) * t * mid +
//                            Mathf.Pow(t, 2) * b;
//                lr.SetPosition(i, p);
//            }
//        }
//    }

//    private void ClearMap()
//    {
//        for (int i = gridParent.childCount - 1; i >= 0; i--)
//        {
//            DestroyImmediate(gridParent.GetChild(i).gameObject);
//        }
//    }


//}
