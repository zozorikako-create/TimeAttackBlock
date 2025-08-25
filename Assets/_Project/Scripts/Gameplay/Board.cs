using System.Collections.Generic;
using UnityEngine;

namespace TimeAttackBlock.Gameplay
{
    public class Board : MonoBehaviour
    {
        [Header("Grid Settings")]
        [SerializeField] private Vector2Int gridSize = new Vector2Int(10, 10);
        [SerializeField] private float cellSize = 0.6f;
        [SerializeField] private Vector2 worldOrigin = new Vector2(0, 0);
        [SerializeField] private GameObject gridCellPrefab; // Cell_Grid.prefab

        [Header("Visual Root")]
        [SerializeField] private Transform gridRoot;

        public Vector2Int GridSize => gridSize;
        public float CellSize => cellSize;
        public Vector2 WorldOrigin => worldOrigin;

        private readonly List<GameObject> _spawnedGrid = new List<GameObject>();

        private void Reset()
        {
            gridSize = new Vector2Int(10, 10);
            cellSize = 0.6f;
            worldOrigin = new Vector2(0, 0);
            if (gridRoot == null)
            {
                var go = new GameObject("GridRoot");
                go.transform.SetParent(transform, false);
                gridRoot = go.transform;
            }
        }

        private void Awake()
        {
            if (gridRoot == null)
            {
                var go = new GameObject("GridRoot");
                go.transform.SetParent(transform, false);
                gridRoot = go.transform;
            }
        }

        public Vector3 GridToWorld(int x, int y)
        {
            return new Vector3(worldOrigin.x + x * cellSize, worldOrigin.y + y * cellSize, 0f);
        }

        public Rect GetWorldRect()
        {
            return new Rect(worldOrigin, new Vector2(gridSize.x * cellSize, gridSize.y * cellSize));
        }

        public void CreateGrid()
        {
            ClearGridVisual();
            if (gridCellPrefab == null) return;

            for (int y = 0; y < gridSize.y; y++)
            {
                for (int x = 0; x < gridSize.x; x++)
                {
                    var go = Instantiate(gridCellPrefab, gridRoot);
                    go.name = $"Cell_{x}_{y}";
                    go.transform.position = GridToWorld(x, y);
                    go.transform.localScale = Vector3.one * cellSize;
                    _spawnedGrid.Add(go);
                }
            }
        }

        public void RebuildGrid()
        {
            CreateGrid();
        }

        private void ClearGridVisual()
        {
            for (int i = 0; i < _spawnedGrid.Count; i++)
            {
                if (_spawnedGrid[i] != null) DestroyImmediate(_spawnedGrid[i]);
            }
            _spawnedGrid.Clear();

            // ”O‚Ì‚½‚ßGridRoot’¼‰º‚ÌŽcŠ[‚àÁ‚·
            if (gridRoot != null)
            {
                var tmp = new List<Transform>();
                foreach (Transform c in gridRoot) tmp.Add(c);
                foreach (var c in tmp) DestroyImmediate(c.gameObject);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            gridSize.x = Mathf.Max(1, gridSize.x);
            gridSize.y = Mathf.Max(1, gridSize.y);
            cellSize = Mathf.Max(0.01f, cellSize);
        }
#endif
    }
}