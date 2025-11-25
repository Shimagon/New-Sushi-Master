using UnityEngine;

/// <summary>
/// シンプルな米釜（VRなしでテスト可能）
/// マウスクリックで米を生成する
/// </summary>
public class SimpleRiceContainer : MonoBehaviour
{
    [Header("Rice Settings")]
    [Tooltip("生成する米のPrefab")]
    public GameObject ricePrefab;

    [Tooltip("米の生成位置オフセット")]
    public Vector3 riceSpawnOffset = new Vector3(0f, 1f, 0f);

    [Header("Audio")]
    [Tooltip("米を掴んだときの効果音")]
    public AudioClip riceGrabSound;

    [Header("Settings")]
    [Tooltip("連続生成のクールダウン時間（秒）")]
    public float cooldownTime = 0.5f;

    [Header("Debug")]
    [Tooltip("デバッグモード")]
    public bool debugMode = true;

    private float lastSpawnTime = 0f;

    void Start()
    {
        if (debugMode)
        {
            Debug.Log($"SimpleRiceContainer が起動しました。クリックして米を生成できます。");
        }
    }

    void OnMouseDown()
    {
        // クールダウンチェック
        if (Time.time - lastSpawnTime < cooldownTime)
        {
            if (debugMode)
            {
                Debug.Log("米生成のクールダウン中です");
            }
            return;
        }

        // 米のPrefabが設定されているか確認
        if (ricePrefab == null)
        {
            Debug.LogError("米のPrefabが設定されていません！");
            return;
        }

        // 米を生成する位置
        Vector3 spawnPosition = transform.position + riceSpawnOffset;
        Quaternion spawnRotation = Quaternion.identity;

        // 米を生成
        GameObject newRice = Instantiate(ricePrefab, spawnPosition, spawnRotation);

        if (debugMode)
        {
            Debug.Log($"米を生成しました at {spawnPosition}");
        }

        // 効果音を再生
        if (riceGrabSound != null)
        {
            AudioSource.PlayClipAtPoint(riceGrabSound, spawnPosition);
        }

        // クールダウン時間を記録
        lastSpawnTime = Time.time;
    }

    // エディタでの視覚化
    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Vector3 spawnPos = transform.position + riceSpawnOffset;
        Gizmos.DrawWireCube(spawnPos, Vector3.one * 0.3f);
        Gizmos.DrawLine(transform.position, spawnPos);
    }
}
