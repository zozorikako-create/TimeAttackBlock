using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using TimeAttackBlock;
using TimeAttackBlock.Gameplay;

public class PieceSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Board board;
    [SerializeField] private BlockShapeDatabase shapeDB;
    [SerializeField] private GameObject cellPrefab; // PieceView が動的に生成時に使う1マスPrefab

    [Header("Auto Layout (portrait)")]
    [SerializeField] private Camera cam;
    [Range(0.05f, 0.35f)] public float handAreaRatio = 0.15f;
    [Range(0.60f, 0.98f)] public float handWidthRatio = 0.90f;
    [Range(0.00f, 0.20f)] public float horizontalGapRatio = 0.06f;
    [Range(0f, 0.5f)] public float handYOffsetRatio = 0.10f; // 下帯の中心を少し上へ

    [Header("Slot Padding / Fit")]
    [Range(0.80f, 1.00f)] public float slotInnerPadding = 0.92f;
    public bool fitToSlot = false; // 盤面とセルサイズを揃えたいならOFF推奨

    [Header("Visual Scale")]
    [Range(0.5f, 1.2f)] public float handCellScale = 0.94f;

    private readonly List<PieceView> current = new();
    private readonly List<int> bag = new();
    private int bagIndex;

    private void Awake()
    {
        if (!cam) cam = Camera.main;
    }

    private void Start()
    {
        if (!ValidateRefs()) return;
        RebuildBag();
        SpawnHand();
    }

    private void Update()
    {
        if (!ValidateRefs()) return;
        if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Space)) SpawnHand();
    }

    public void SpawnHand()
    {
        foreach (var v in current) if (v) Destroy(v.gameObject);
        current.Clear();

        if (bagIndex >= bag.Count) RebuildBag();
        var (centers, slotW, slotH) = CalcSlots();

        float baseCell = Mathf.Max(0.01f, board.CellSize * handCellScale);

        for (int i = 0; i < 3; i++)
        {
            if (bagIndex >= bag.Count) RebuildBag();
            int idx = bag[bagIndex++];

            var shape = (shapeDB && shapeDB.shapes != null && idx < shapeDB.shapes.Count)
                ? shapeDB.shapes[idx] : null;
            if (shape == null || shape.cells == null || shape.cells.Count == 0) { i--; continue; }

            // ランダム回転 → 正規化
            int k = Random.Range(0, 4);
            var cells = RotateAndNormalize(shape.cells, k);

            // 5マス一直線は縦固定
            if (IsFiveLine(cells, out var w, out var h))
            {
                int safety = 4;
                while (w != 1 && safety-- > 0)
                {
                    cells = RotateAndNormalize(cells, 1);
                    IsFiveLine(cells, out w, out h);
                }
            }

            // セルサイズ（fitToSlotがONのときのみスロットに合わせて縮小）
            float cellSize = baseCell;
            if (fitToSlot)
            {
                GetSizeInCells(cells, out int cw, out int ch);
                float maxByW = (slotW * slotInnerPadding) / Mathf.Max(1, cw);
                float maxByH = (slotH * slotInnerPadding) / Mathf.Max(1, ch);
                cellSize = Mathf.Min(baseCell, Mathf.Min(maxByW, maxByH));
            }

            var go = new GameObject($"Hand_{i}_{shape.id}_r{k}");
            go.transform.SetParent(transform, false);
            go.transform.position = centers[i];

            var view = go.AddComponent<PieceView>();
            view.cellPrefab = cellPrefab;
            view.SetPreviewMode(true);
            view.BuildFromCells(cells, cellSize);

            // 中心合わせ（PieceView側でセンタ済みだが保険で再取得）
            var b = view.GetWorldBounds();
            go.transform.position += (centers[i] - b.center);

            current.Add(view);
        }
    }

    private (Vector3[] centers, float slotW, float slotH) CalcSlots()
    {
        float worldH = cam.orthographicSize * 2f;
        float worldW = worldH * cam.aspect;

        float bandH = worldH * handAreaRatio;
        float centerY = -worldH * 0.5f + bandH * 0.5f + handYOffsetRatio * bandH;

        float usableW = worldW * handWidthRatio;
        float gapW = worldW * horizontalGapRatio;

        float slotW = (usableW - gapW * 2f) / 3f;
        float slotH = bandH;

        float left = -usableW * 0.5f;
        float x1 = left + slotW * 0.5f;
        float x2 = x1 + slotW + gapW;
        float x3 = x2 + slotW + gapW;

        return (new[]
        {
            new Vector3(x1, centerY, 0f),
            new Vector3(x2, centerY, 0f),
            new Vector3(x3, centerY, 0f)
        }, slotW, slotH);
    }

    private void RebuildBag()
    {
        bag.Clear();
        int n = (shapeDB && shapeDB.shapes != null) ? shapeDB.shapes.Count : 0;
        for (int i = 0; i < n; i++) bag.Add(i);
        for (int i = bag.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (bag[i], bag[j]) = (bag[j], bag[i]);
        }
        bagIndex = 0;
    }

    private bool ValidateRefs()
    {
        if (!board) { Debug.LogWarning("[PieceSpawner] Board not assigned."); return false; }
        if (!shapeDB) { Debug.LogWarning("[PieceSpawner] Shape DB not assigned."); return false; }
        if (!cellPrefab) { Debug.LogWarning("[PieceSpawner] Cell Prefab not assigned."); return false; }
        if (!cam) cam = Camera.main;
        return true;
    }

    // ==== helpers ====
    private static List<Vector2Int> RotateAndNormalize(List<Vector2Int> src, int k)
    {
        k = ((k % 4) + 4) % 4;
        var cs = new List<Vector2Int>(src);
        for (int r = 0; r < k; r++)
            for (int i = 0; i < cs.Count; i++)
                cs[i] = new Vector2Int(-cs[i].y, cs[i].x);

        int minX = int.MaxValue, minY = int.MaxValue;
        for (int i = 0; i < cs.Count; i++)
        {
            if (cs[i].x < minX) minX = cs[i].x;
            if (cs[i].y < minY) minY = cs[i].y;
        }
        for (int i = 0; i < cs.Count; i++)
            cs[i] = new Vector2Int(cs[i].x - minX, cs[i].y - minY);

        return cs;
    }

    private static void GetSizeInCells(List<Vector2Int> cells, out int width, out int height)
    {
        int maxX = 0, maxY = 0;
        for (int i = 0; i < cells.Count; i++)
        {
            if (cells[i].x > maxX) maxX = cells[i].x;
            if (cells[i].y > maxY) maxY = cells[i].y;
        }
        width = maxX + 1;
        height = maxY + 1;
    }

    private static bool IsFiveLine(List<Vector2Int> cs, out int w, out int h)
    {
        int minX = int.MaxValue, maxX = int.MinValue;
        int minY = int.MaxValue, maxY = int.MinValue;
        foreach (var c in cs)
        {
            if (c.x < minX) minX = c.x; if (c.x > maxX) maxX = c.x;
            if (c.y < minY) minY = c.y; if (c.y > maxY) maxY = c.y;
        }
        w = (maxX - minX + 1);
        h = (maxY - minY + 1);
        return cs.Count == 5 && (w == 5 && h == 1 || w == 1 && h == 5);
    }
}