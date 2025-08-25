using System.Collections.Generic;
using UnityEngine;

namespace TimeAttackBlock.Gameplay
{
    /// <summary>
    /// セルPrefabを並べて手札/プレビューの見た目を作る。
    /// ・セルを配置→境界計測→中心合わせ（ローカル原点が常に中心）
    /// ・「影＋本体」の2レイヤーで立体感を表現
    /// </summary>
    public class PieceView : MonoBehaviour
    {
        [Header("Cell Visual")]
        [Tooltip("1マスの四角プレハブ（SpriteRenderer付き）")]
        public GameObject cellPrefab;

        [Tooltip("セル間の隙間（cellSize に対する割合）")]
        [Range(0f, 0.3f)] public float cellGapRatio = 0.10f;

        [Tooltip("見た目の色（本体）")]
        public Color faceTint = new Color(1f, 0.92f, 0.2f, 1f);

        [Tooltip("プレビュー用の半透明")]
        [Range(0.05f, 1f)] public float previewAlpha = 0.9f;

        [Header("Shadow")]
        public bool useShadow = true;
        [Tooltip("影のローカルオフセット（cellSizeを1としたときの相対量）")]
        public Vector2 shadowOffset = new Vector2(0.06f, -0.06f);
        [Tooltip("影の色")]
        public Color shadowTint = new Color(0.95f, 0.7f, 0.05f, 0.9f);

        private readonly List<Transform> _cells = new();
        private readonly List<Transform> _shadows = new();
        private float _lastCellSize = 1f;

        /// <summary>最新のワールド境界（Build後に更新）</summary>
        public Bounds LastWorldBounds { get; private set; }

        /// <summary>互換性のため（PieceSpawner から呼びやすいように）</summary>
        public Bounds GetWorldBounds() => LastWorldBounds;

        public void SetPreviewMode(bool on)
        {
            // 今回はαだけで表現
            previewAlpha = on ? Mathf.Clamp(previewAlpha, 0.05f, 1f) : 1f;
        }

        /// <summary>
        /// 見た目構築（cells は (0,0) 原点正規化済みの整数座標）
        /// </summary>
        public void BuildFromCells(IReadOnlyList<Vector2Int> cells, float cellSize, GameObject cellPrefabOverride = null)
        {
            if (cellPrefabOverride) cellPrefab = cellPrefabOverride;
            if (!cellPrefab)
            {
                Debug.LogError("[PieceView] Cell Prefab not assigned.");
                return;
            }
            _lastCellSize = cellSize;

            EnsurePool(cells.Count);

            float gap = cellSize * cellGapRatio;              // セル間隔
            float visual = Mathf.Max(0.001f, cellSize - gap); // 実際の四角一辺
            float half = visual * 0.5f;

            // いったん左下原点で並べる（中心合わせは後段）
            for (int i = 0; i < cells.Count; i++)
            {
                Vector2Int p = cells[i];

                // 影
                if (useShadow)
                {
                    var tS = _shadows[i];
                    tS.gameObject.SetActive(true);
                    Vector3 posS = new Vector3(p.x * cellSize + half + shadowOffset.x * cellSize,
                                               p.y * cellSize + half + shadowOffset.y * cellSize, 0f);
                    tS.localPosition = posS;
                    tS.localRotation = Quaternion.identity;
                    tS.localScale = Vector3.one * (visual / GetSpriteUnitSize());
                    var srS = tS.GetComponent<SpriteRenderer>();
                    if (srS)
                    {
                        var c = shadowTint; c.a *= previewAlpha;
                        srS.color = c;
                    }
                }
                else _shadows[i].gameObject.SetActive(false);

                // 本体
                var t = _cells[i];
                t.gameObject.SetActive(true);
                Vector3 pos = new Vector3(p.x * cellSize + half, p.y * cellSize + half, 0f);
                t.localPosition = pos;
                t.localRotation = Quaternion.identity;
                t.localScale = Vector3.one * (visual / GetSpriteUnitSize());
                var sr = t.GetComponent<SpriteRenderer>();
                if (sr)
                {
                    var c = faceTint; c.a *= previewAlpha;
                    sr.color = c;
                }
            }

            // 余ったプールは非表示
            for (int i = cells.Count; i < _cells.Count; i++)
            {
                _cells[i].gameObject.SetActive(false);
                if (_shadows[i]) _shadows[i].gameObject.SetActive(false);
            }

            // ===== 境界→中心合わせ =====
            var bounds = CalcWorldBounds();                    // 現在の境界
            Vector3 worldCenter = bounds.center;
            Vector3 localCenter = transform.InverseTransformPoint(worldCenter);
            ShiftChildren(-localCenter);                       // 原点を中心に

            // 取り直して保存
            LastWorldBounds = CalcWorldBounds();
        }

        // 旧名互換（Build を呼ぶコードがあっても動くように）
        public void Build(Vector2Int[] cells, float cellSize)
            => BuildFromCells(cells, cellSize, null);

        public void Clear()
        {
            for (int i = 0; i < _cells.Count; i++)
            {
                if (_cells[i]) _cells[i].gameObject.SetActive(false);
                if (_shadows[i]) _shadows[i].gameObject.SetActive(false);
            }
        }

        // ===== 内部処理 =====
        private void EnsurePool(int count)
        {
            while (_cells.Count < count)
            {
                var goShadow = new GameObject($"shadow_{_shadows.Count:D2}");
                goShadow.transform.SetParent(transform, false);
                var srS = goShadow.AddComponent<SpriteRenderer>();
                CopySpriteRenderer(cellPrefab, srS, orderDelta: -1);
                _shadows.Add(goShadow.transform);

                var go = Instantiate(cellPrefab, transform);
                go.name = $"cell_{_cells.Count:D2}";
                var sr = go.GetComponent<SpriteRenderer>();
                if (sr) sr.sortingOrder += 1; // 影より前
                _cells.Add(go.transform);
            }
        }

        private void CopySpriteRenderer(GameObject srcPrefab, SpriteRenderer dst, int orderDelta)
        {
            var src = srcPrefab.GetComponent<SpriteRenderer>();
            if (!src) return;
            dst.sprite = src.sprite;
            dst.sortingLayerID = src.sortingLayerID;
            dst.sortingLayerName = src.sortingLayerName;
            dst.sortingOrder = src.sortingOrder + orderDelta;
            dst.drawMode = src.drawMode;
            dst.maskInteraction = src.maskInteraction;
        }

        private float GetSpriteUnitSize()
        {
            var sr = cellPrefab.GetComponent<SpriteRenderer>();
            if (!sr || !sr.sprite) return 1f;
            return sr.sprite.bounds.size.x; // 正方スプライト想定
        }

        private Bounds CalcWorldBounds()
        {
            bool has = false;
            Bounds b = new Bounds(transform.position, Vector3.zero);

            void Enc(Transform t)
            {
                var sr = t.GetComponent<SpriteRenderer>();
                if (!sr || !t.gameObject.activeInHierarchy) return;
                if (!has) { b = sr.bounds; has = true; }
                else b.Encapsulate(sr.bounds);
            }

            for (int i = 0; i < _cells.Count; i++)
            {
                if (_shadows[i]) Enc(_shadows[i]);
                if (_cells[i]) Enc(_cells[i]);
            }
            if (!has) b = new Bounds(transform.position, new Vector3(_lastCellSize, _lastCellSize, 0.01f));
            return b;
        }

        private void ShiftChildren(Vector3 deltaLocal)
        {
            for (int i = 0; i < _cells.Count; i++)
            {
                if (_shadows[i]) _shadows[i].localPosition += deltaLocal;
                if (_cells[i]) _cells[i].localPosition += deltaLocal;
            }
        }
    }
}