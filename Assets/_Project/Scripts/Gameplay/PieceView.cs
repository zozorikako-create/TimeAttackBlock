using System.Collections.Generic;
using UnityEngine;

namespace TimeAttackBlock.Gameplay
{
    /// <summary>
    /// �Z��Prefab����ׂĎ�D/�v���r���[�̌����ڂ����B
    /// �E�Z����z�u�����E�v�������S���킹�i���[�J�����_����ɒ��S�j
    /// �E�u�e�{�{�́v��2���C���[�ŗ��̊���\��
    /// </summary>
    public class PieceView : MonoBehaviour
    {
        [Header("Cell Visual")]
        [Tooltip("1�}�X�̎l�p�v���n�u�iSpriteRenderer�t���j")]
        public GameObject cellPrefab;

        [Tooltip("�Z���Ԃ̌��ԁicellSize �ɑ΂��銄���j")]
        [Range(0f, 0.3f)] public float cellGapRatio = 0.10f;

        [Tooltip("�����ڂ̐F�i�{�́j")]
        public Color faceTint = new Color(1f, 0.92f, 0.2f, 1f);

        [Tooltip("�v���r���[�p�̔�����")]
        [Range(0.05f, 1f)] public float previewAlpha = 0.9f;

        [Header("Shadow")]
        public bool useShadow = true;
        [Tooltip("�e�̃��[�J���I�t�Z�b�g�icellSize��1�Ƃ����Ƃ��̑��Ηʁj")]
        public Vector2 shadowOffset = new Vector2(0.06f, -0.06f);
        [Tooltip("�e�̐F")]
        public Color shadowTint = new Color(0.95f, 0.7f, 0.05f, 0.9f);

        private readonly List<Transform> _cells = new();
        private readonly List<Transform> _shadows = new();
        private float _lastCellSize = 1f;

        /// <summary>�ŐV�̃��[���h���E�iBuild��ɍX�V�j</summary>
        public Bounds LastWorldBounds { get; private set; }

        /// <summary>�݊����̂��߁iPieceSpawner ����Ăт₷���悤�Ɂj</summary>
        public Bounds GetWorldBounds() => LastWorldBounds;

        public void SetPreviewMode(bool on)
        {
            // ����̓������ŕ\��
            previewAlpha = on ? Mathf.Clamp(previewAlpha, 0.05f, 1f) : 1f;
        }

        /// <summary>
        /// �����ڍ\�z�icells �� (0,0) ���_���K���ς݂̐������W�j
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

            float gap = cellSize * cellGapRatio;              // �Z���Ԋu
            float visual = Mathf.Max(0.001f, cellSize - gap); // ���ۂ̎l�p���
            float half = visual * 0.5f;

            // �������񍶉����_�ŕ��ׂ�i���S���킹�͌�i�j
            for (int i = 0; i < cells.Count; i++)
            {
                Vector2Int p = cells[i];

                // �e
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

                // �{��
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

            // �]�����v�[���͔�\��
            for (int i = cells.Count; i < _cells.Count; i++)
            {
                _cells[i].gameObject.SetActive(false);
                if (_shadows[i]) _shadows[i].gameObject.SetActive(false);
            }

            // ===== ���E�����S���킹 =====
            var bounds = CalcWorldBounds();                    // ���݂̋��E
            Vector3 worldCenter = bounds.center;
            Vector3 localCenter = transform.InverseTransformPoint(worldCenter);
            ShiftChildren(-localCenter);                       // ���_�𒆐S��

            // ��蒼���ĕۑ�
            LastWorldBounds = CalcWorldBounds();
        }

        // �����݊��iBuild ���ĂԃR�[�h�������Ă������悤�Ɂj
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

        // ===== �������� =====
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
                if (sr) sr.sortingOrder += 1; // �e���O
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
            return sr.sprite.bounds.size.x; // �����X�v���C�g�z��
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