using UnityEngine;

/// <summary>
/// TimeAttackBlock - Board
/// - Creates a visible 10x10 grid using a Cell prefab
/// - Bottom-left is (0,0) at this object's Transform.position
/// - Cell size is in world units (default 1x1)
/// - Provides GridToWorld/WorldToGrid helpers
/// Unity 2022 LTS / 2D (Built-in RP)
/// </summary>
public class Board : MonoBehaviour
{
    [Header("Grid Size")]
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;

    [Header("Cell")]
    [SerializeField] private GameObject cellPrefab;         // Assign CellPrefab.prefab
    [SerializeField] private Vector2 cellSize = Vector2.one; // 1x1 world unit
    [SerializeField] private float spacing = 0f;             // gap between cells (world units)
    [SerializeField] private string sortingLayerName = "Board";
    [SerializeField] private int sortingOrderBase = 0;

    [Header("Runtime (auto)")]
    [SerializeField] private Transform cellsRoot; // parent container for all cells

    private GameObject[,] cells;

    public int Width => width;
    public int Height => height;

    private void Reset()
    {
        width = 10;
        height = 10;
        cellSize = Vector2.one;
        spacing = 0f;
        sortingLayerName = "Board";
    }

    private void Awake()
    {
        EnsureCellsRoot();
    }

    private void Start()
    {
        // Generate only if empty at start
        if (cells == null || cells.Length == 0)
        {
            Regenerate();
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        width = Mathf.Clamp(width, 1, 100);
        height = Mathf.Clamp(height, 1, 100);
        if (!Application.isPlaying)
        {
            // Regenerate grid in editor when values change
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (this == null) return;
                Regenerate();
            };
        }
    }
#endif

    // Public entry to rebuild grid
    public void Regenerate()
    {
        ClearGrid();
        CreateGrid();
    }

    // Destroys previous grid objects and allocates the array
    public void ClearGrid()
    {
        EnsureCellsRoot();

        // Destroy all children of cellsRoot
        for (int i = cellsRoot.childCount - 1; i >= 0; i--)
        {
            var child = cellsRoot.GetChild(i);
#if UNITY_EDITOR
            if (!Application.isPlaying)
                DestroyImmediate(child.gameObject);
            else
                Destroy(child.gameObject);
#else
            Destroy(child.gameObject);
#endif
        }

        cells = new GameObject[width, height];
    }

    // The main creation loop: (x,y) from (0,0) bottom-left
    public void CreateGrid()
    {
        if (cellPrefab == null)
        {
            Debug.LogError("[Board] Cell Prefab is not assigned.");
            return;
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2 worldPos = GridToWorld(x, y);
                var go = Instantiate(cellPrefab, worldPos, Quaternion.identity, cellsRoot);
                go.name = $"Cell_{x}_{y}";

                // Enforce uniform size and sorting
                var sr = go.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sortingLayerName = sortingLayerName;
                    sr.sortingOrder = sortingOrderBase;

                    // Scale the sprite so its bounds match cellSize exactly
                    if (sr.sprite != null)
                    {
                        var b = sr.sprite.bounds.size; // in local space (units)
                        Vector3 scale = go.transform.localScale;
                        if (b.x != 0f && b.y != 0f)
                        {
                            scale.x = cellSize.x / b.x;
                            scale.y = cellSize.y / b.y;
                            go.transform.localScale = scale;
                        }
                    }
                }

                cells[x, y] = go;
            }
        }
    }

    // Convert grid (x,y) to world position (center of the cell)
    public Vector2 GridToWorld(int x, int y)
    {
        Vector2 origin = transform.position;
        float stepX = cellSize.x + spacing;
        float stepY = cellSize.y + spacing;

        // +0.5 cell offsets place the pivot at each cell's center
        return new Vector2(
            origin.x + x * stepX + (cellSize.x * 0.5f),
            origin.y + y * stepY + (cellSize.y * 0.5f)
        );
    }

    // Convert world position to grid index; returns false if outside grid
    public bool WorldToGrid(Vector2 world, out int x, out int y)
    {
        Vector2 origin = transform.position;
        float stepX = cellSize.x + spacing;
        float stepY = cellSize.y + spacing;

        float localX = world.x - origin.x;
        float localY = world.y - origin.y;

        x = Mathf.FloorToInt(localX / stepX);
        y = Mathf.FloorToInt(localY / stepY);

        return (x >= 0 && x < width && y >= 0 && y < height);
    }

    private void EnsureCellsRoot()
    {
        if (cellsRoot == null)
        {
            var root = new GameObject("CellsRoot");
            root.transform.SetParent(transform, false);
            cellsRoot = root.transform;
        }
    }

#if UNITY_EDITOR
    // Nice editor preview of the grid boxes
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.35f); // semi-transparent green
        for (int y = 0; y < Mathf.Max(1, height); y++)
        {
            for (int x = 0; x < Mathf.Max(1, width); x++)
            {
                Vector2 c = GridToWorld(x, y);
                Gizmos.DrawWireCube(c, new Vector3(cellSize.x, cellSize.y, 0f));
            }
        }
    }
#endif
}