using UnityEngine;

/// <summary>
/// シンプルな魚ネタ元（VRなしでテスト可能）
/// マウスクリックで魚ネタを生成する
/// </summary>
public class SimpleFishSource : MonoBehaviour
{
    [Header("Fish Settings")]
    [Tooltip("生成する魚ネタのPrefab")]
    public GameObject fishPrefab;

    [Tooltip("魚ネタの生成位置オフセット")]
    public Vector3 fishSpawnOffset = new Vector3(0f, 1f, 0f);

    [Header("Audio")]
    [Tooltip("魚ネタを掴んだときの効果音")]
    public AudioClip fishGrabSound;

    [Header("Settings")]
    [Tooltip("連続生成のクールダウン時間（秒）")]
    public float cooldownTime = 0.5f;

    [Header("Visual Feedback")]
    [Tooltip("この元が生成する魚の種類（表示用）")]
    public string fishTypeName = "Maguro";

    [Header("Debug")]
    [Tooltip("デバッグモード")]
    public bool debugMode = true;

    private float lastSpawnTime = 0f;

    void Start()
    {
        // オブジェクト名に魚の種類を追加
        name = $"SimpleFishSource ({fishTypeName})";

        if (debugMode)
        {
            Debug.Log($"SimpleFishSource ({fishTypeName}) が起動しました。クリックして魚ネタを生成できます。");
        }
    }

    void OnMouseDown()
    {
        // クールダウンチェック
        if (Time.time - lastSpawnTime < cooldownTime)
        {
            if (debugMode)
            {
                Debug.Log("魚ネタ生成のクールダウン中です");
            }
            return;
        }

        // 魚ネタのPrefabが設定されているか確認
        if (fishPrefab == null)
        {
            Debug.LogError("魚ネタのPrefabが設定されていません！");
            return;
        }

        // 魚ネタを生成する位置
        Vector3 spawnPosition = transform.position + fishSpawnOffset;
        Quaternion spawnRotation = Quaternion.identity;

        // 魚ネタを生成
        GameObject newFish = Instantiate(fishPrefab, spawnPosition, spawnRotation);

        if (debugMode)
        {
            Debug.Log($"{fishTypeName}ネタを生成しました at {spawnPosition}");
        }

        // 効果音を再生
        if (fishGrabSound != null)
        {
            AudioSource.PlayClipAtPoint(fishGrabSound, spawnPosition);
        }

        // クールダウン時間を記録
        lastSpawnTime = Time.time;
    }

    // エディタでの視覚化
    void OnDrawGizmos()
    {
        // 魚の種類に応じて色を変える
        switch (fishTypeName)
        {
            case "Maguro":
                Gizmos.color = Color.red;
                break;
            case "Tamago":
                Gizmos.color = Color.yellow;
                break;
            case "Salmon":
                Gizmos.color = new Color(1f, 0.5f, 0.3f);
                break;
            default:
                Gizmos.color = Color.white;
                break;
        }

        Vector3 spawnPos = transform.position + fishSpawnOffset;
        Gizmos.DrawWireCube(spawnPos, Vector3.one * 0.3f);
        Gizmos.DrawLine(transform.position, spawnPos);
    }
}
