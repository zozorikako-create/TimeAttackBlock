using System.Collections.Generic;
using UnityEngine;

namespace TimeAttackBlock.Gameplay
{
    /// <summary>
    /// ブロック（複数セルの集合）。見た目は Cell.prefab を並べて構築する。
    /// - BuildVisual(Vector2Int[] pattern, float cellSize) で生成
    /// - SetPreviewMode(bool) でプレビュー用に Collider2D を無効化
    /// - GetWorldBounds() で現在の見た目のワールド境界を取得
    /// 互換用に SetShape/SetCellSize/BuildVisual() も実装（呼び出しは上記に委譲）
    /// </summary>
    public class Block : MonoBehaviour
    {
        [Header("Visual")]
        [Tooltip("各セルの見た目に使うプレハブ（SpriteRenderer付き想定）")]
        public GameObject cellPrefab;
        [Tooltip("セルの色（SpriteRendererがあれば適用）")]
        public Color color = Color.white;
        [Tooltip("セルをぶら下げるルート。未設定でも動的に作成されます")]
        public Transform visualRoot;

        private readonly List<Transform> _spawnedCells = new List<Transform>();
        private Vector2Int[] _pattern = new Vector2Int[0];
        private float _cellSize = 1f;
        private bool _isPreview = false;

        private void EnsureRoot()
        {
            if (visualRoot) return;
            var go = new GameObject("VisualRoot");
            go.transform.SetParent(transform, false);
            visualRoot = go.transform;
        }

        /// <summary>プレビュー用に当たり判定を切る/戻す</summary>
        public void SetPreviewMode(bool isPreview)
        {
            _isPreview = isPreview;
            var cols = GetComponentsInChildren<Collider2D>(true);
            foreach (var c in cols) c.enabled = !_isPreview;
        }

        /// <summary>
        /// 指定パターンとセルサイズで見た目を構築。親のスケールは常に等倍のまま。
        /// </summary>
        public void BuildVisual(Vector2Int[] pattern, float cellSize)
        {
            _pattern = pattern ?? new Vector2Int[0];
            _cellSize = Mathf.Max(0.001f, cellSize);
            Rebuild();
        }

        /// <summary>現在のワールド境界（レンダラー合成）</summary>
        public Bounds GetWorldBounds()
        {
            var rends = GetComponentsInChildren<Renderer>(true);
            if (rends.Length == 0) return new Bounds(transform.position, Vector3.zero);
            var b = rends[0].bounds;
            for (int i = 1; i < rends.Length; i++) b.Encapsulate(rends[i].bounds);
            return b;
        }

        // ===== 互換API（旧コードからも呼べるように） =====
        public void SetShape(IEnumerable<Vector2Int> cells)
        {
            _pattern = cells != null ? new List<Vector2Int>(cells).ToArray() : new Vector2Int[0];
        }
        public void SetCellSize(float size) { _cellSize = Mathf.Max(0.001f, size); }
        public void BuildVisual() { Rebuild(); }
        // ================================================

        private void Rebuild()
        {
            EnsureRoot();

            // 親スケールは常に1（スケールの累積で歪まないように）
            transform.localScale = Vector3.one;

            // 既存セルを削除
            for (int i = _spawnedCells.Count - 1; i >= 0; i--)
            {
                if (_spawnedCells[i]) DestroyImmediate(_spawnedCells[i].gameObject);
            }
            _spawnedCells.Clear();

            if (cellPrefab == null || _pattern == null || _pattern.Length == 0) return;

            // まずセルを相対座標で並べる
            foreach (var p in _pattern)
            {
                var cell = Instantiate(cellPrefab, visualRoot);
                cell.transform.localPosition = new Vector3(p.x * _cellSize, p.y * _cellSize, 0f);
                cell.transform.localRotation = Quaternion.identity;
                cell.transform.localScale = Vector3.one * _cellSize;

                var sr = cell.GetComponent<SpriteRenderer>();
                if (sr) sr.color = color;

                _spawnedCells.Add(cell.transform);
            }

            // 図形の中心が(0,0)に来るようにオフセット（見た目が中央揃えになる）
            var lb = GetLocalBoundsFromPattern();
            Vector3 center = lb.center;
            foreach (var t in _spawnedCells)
            {
                t.localPosition -= center;
            }
        }

        /// <summary>パターンからローカル境界を計算（点群→サイズ補正）</summary>
        private Bounds GetLocalBoundsFromPattern()
        {
            if (_pattern == null || _pattern.Length == 0)
                return new Bounds(Vector3.zero, Vector3.zero);

            var p0 = new Vector3(_pattern[0].x * _cellSize, _pattern[0].y * _cellSize, 0f);
            var b = new Bounds(p0, Vector3.zero);

            for (int i = 1; i < _pattern.Length; i++)
            {
                var p = new Vector3(_pattern[i].x * _cellSize, _pattern[i].y * _cellSize, 0f);
                b.Encapsulate(p);
            }

            // 点群境界にセルのサイズを少し反映して視覚サイズに近づける
            var size = b.size;
            if (size.x < _cellSize) size.x = _cellSize;
            if (size.y < _cellSize) size.y = _cellSize;
            b.size = size;

            return b;
        }
    }
}