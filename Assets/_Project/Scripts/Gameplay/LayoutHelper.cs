using UnityEngine;

/// <summary>
/// Portrait向けレイアウト調整（カメラ/ボード/手札スロット）
/// ※ 手札が低い/高いは HandZoneHeight と HandYOffset をいじるのが一番簡単！
/// </summary>
public class LayoutHelper : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform boardRoot;
    [SerializeField] private Transform handSlot1;
    [SerializeField] private Transform handSlot2;
    [SerializeField] private Transform handSlot3;

    [Header("Tuning (world units)")]
    [Tooltip("上部HUDに確保する高さ（ワールド単位）")]
    [SerializeField] private float hudReserveHeight = 2.2f;

    [Tooltip("下部の手札ゾーンの高さ（ワールド単位）")]
    [SerializeField] private float handZoneHeight = 3.1f; // ←既定値を少し大きめに

    [Tooltip("上下マージン（ボードの上下に少し空き）")]
    [SerializeField] private float verticalMargin = 0.4f;

    [Tooltip("左右マージン（ボードの左右に少し空き）")]
    [SerializeField] private float horizontalMargin = 0.4f;

    [Header("Board Size")]
    [Tooltip("ボードは正方形想定。1辺のセル数（10で10x10）。見た目の計算用")]
    [SerializeField] private int boardCells = 10;

    [Tooltip("1セルの基本サイズ（boardRootに対する基準）。0.6〜1.0で調整")]
    [SerializeField] private float baseCellSize = 0.7f;

    [Header("Hand Slots")]
    [Tooltip("手札の左右寄せ比率（0=中央寄り、1=端寄せ）")]
    [Range(0.5f, 0.9f)]
    [SerializeField] private float handHorizontalFactor = 0.72f;

    [Tooltip("手札の上下位置オフセット（下端からの持ち上げ量）")]
    [SerializeField] private float handYOffset = 1.0f; // ←既定値を少し上げる

    private void Reset()
    {
        mainCamera = Camera.main;
    }

    private void Start()
    {
        ApplyLayout();
    }

    private void OnValidate()
    {
        if (Application.isPlaying) ApplyLayout();
    }

    public void ApplyLayout()
    {
        if (!mainCamera || !boardRoot || !handSlot1 || !handSlot2 || !handSlot3)
        {
            Debug.LogWarning("[LayoutHelper] Missing references.");
            return;
        }

        mainCamera.orthographic = true;

        // 半分高さ（画面全高≒14）。端末比で横が変化→ボード側のスケールで吸収
        float targetOrthoHalfHeight = 7.0f;
        mainCamera.orthographicSize = targetOrthoHalfHeight;

        float camHalfH = mainCamera.orthographicSize;
        float camHalfW = camHalfH * mainCamera.aspect;

        // 中央の利用可能領域（HUD＋手札を除いた残り）
        float availableH = (camHalfH * 2f) - hudReserveHeight - handZoneHeight - (verticalMargin * 2f);
        if (availableH < 1f) availableH = 1f;

        float availableW = (camHalfW * 2f) - (horizontalMargin * 2f);
        if (availableW < 1f) availableW = 1f;

        // ボード（正方形）を最大で収まるサイズに
        float targetBoardWorldSize = Mathf.Min(availableH, availableW);
        float desiredWorldPerCell = targetBoardWorldSize / Mathf.Max(1, boardCells);
        float scale = desiredWorldPerCell / Mathf.Max(0.01f, baseCellSize);
        boardRoot.localScale = Vector3.one * scale;

        // ボード位置：中央領域の真ん中
        float centerY = (camHalfH - hudReserveHeight) - (availableH / 2f);
        boardRoot.position = new Vector3(0f, centerY, 0f);

        // 手札スロット位置：下ゾーン内
        float handY = -camHalfH + (handZoneHeight / 2f) + handYOffset;
        float xA = -camHalfW * handHorizontalFactor;
        float xB = 0f;
        float xC = +camHalfW * handHorizontalFactor;
        handSlot1.position = new Vector3(xA, handY, 0f);
        handSlot2.position = new Vector3(xB, handY, 0f);
        handSlot3.position = new Vector3(xC, handY, 0f);
    }
}