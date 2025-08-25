using System.Collections.Generic;
using UnityEngine;

namespace TimeAttackBlock.Gameplay
{
    /// <summary>
    /// �u���b�N�i�����Z���̏W���j�B�����ڂ� Cell.prefab ����ׂč\�z����B
    /// - BuildVisual(Vector2Int[] pattern, float cellSize) �Ő���
    /// - SetPreviewMode(bool) �Ńv���r���[�p�� Collider2D �𖳌���
    /// - GetWorldBounds() �Ō��݂̌����ڂ̃��[���h���E���擾
    /// �݊��p�� SetShape/SetCellSize/BuildVisual() �������i�Ăяo���͏�L�ɈϏ��j
    /// </summary>
    public class Block : MonoBehaviour
    {
        [Header("Visual")]
        [Tooltip("�e�Z���̌����ڂɎg���v���n�u�iSpriteRenderer�t���z��j")]
        public GameObject cellPrefab;
        [Tooltip("�Z���̐F�iSpriteRenderer������ΓK�p�j")]
        public Color color = Color.white;
        [Tooltip("�Z�����Ԃ牺���郋�[�g�B���ݒ�ł����I�ɍ쐬����܂�")]
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

        /// <summary>�v���r���[�p�ɓ����蔻���؂�/�߂�</summary>
        public void SetPreviewMode(bool isPreview)
        {
            _isPreview = isPreview;
            var cols = GetComponentsInChildren<Collider2D>(true);
            foreach (var c in cols) c.enabled = !_isPreview;
        }

        /// <summary>
        /// �w��p�^�[���ƃZ���T�C�Y�Ō����ڂ��\�z�B�e�̃X�P�[���͏�ɓ��{�̂܂܁B
        /// </summary>
        public void BuildVisual(Vector2Int[] pattern, float cellSize)
        {
            _pattern = pattern ?? new Vector2Int[0];
            _cellSize = Mathf.Max(0.001f, cellSize);
            Rebuild();
        }

        /// <summary>���݂̃��[���h���E�i�����_���[�����j</summary>
        public Bounds GetWorldBounds()
        {
            var rends = GetComponentsInChildren<Renderer>(true);
            if (rends.Length == 0) return new Bounds(transform.position, Vector3.zero);
            var b = rends[0].bounds;
            for (int i = 1; i < rends.Length; i++) b.Encapsulate(rends[i].bounds);
            return b;
        }

        // ===== �݊�API�i���R�[�h������Ăׂ�悤�Ɂj =====
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

            // �e�X�P�[���͏��1�i�X�P�[���̗ݐςŘc�܂Ȃ��悤�Ɂj
            transform.localScale = Vector3.one;

            // �����Z�����폜
            for (int i = _spawnedCells.Count - 1; i >= 0; i--)
            {
                if (_spawnedCells[i]) DestroyImmediate(_spawnedCells[i].gameObject);
            }
            _spawnedCells.Clear();

            if (cellPrefab == null || _pattern == null || _pattern.Length == 0) return;

            // �܂��Z���𑊑΍��W�ŕ��ׂ�
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

            // �}�`�̒��S��(0,0)�ɗ���悤�ɃI�t�Z�b�g�i�����ڂ����������ɂȂ�j
            var lb = GetLocalBoundsFromPattern();
            Vector3 center = lb.center;
            foreach (var t in _spawnedCells)
            {
                t.localPosition -= center;
            }
        }

        /// <summary>�p�^�[�����烍�[�J�����E���v�Z�i�_�Q���T�C�Y�␳�j</summary>
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

            // �_�Q���E�ɃZ���̃T�C�Y���������f���Ď��o�T�C�Y�ɋ߂Â���
            var size = b.size;
            if (size.x < _cellSize) size.x = _cellSize;
            if (size.y < _cellSize) size.y = _cellSize;
            b.size = size;

            return b;
        }
    }
}