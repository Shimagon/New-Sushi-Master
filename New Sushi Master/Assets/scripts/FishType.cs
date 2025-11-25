using UnityEngine;

/// <summary>
/// 魚ネタの種類を識別するためのコンポーネント
/// このスクリプトを魚ネタのGameObjectにアタッチして、種類を設定します
/// </summary>
public class FishType : MonoBehaviour
{
    [Header("魚の種類")]
    [Tooltip("魚の種類名（Maguro, Tamago, Salmon）")]
    public string fishTypeName = "Maguro";

    [Header("表示名")]
    [Tooltip("UI表示用の日本語名")]
    public string displayName = "マグロ";

    /// <summary>
    /// 魚の種類を取得
    /// </summary>
    public string GetFishType()
    {
        return fishTypeName;
    }

    /// <summary>
    /// 表示名を取得
    /// </summary>
    public string GetDisplayName()
    {
        return displayName;
    }

    // エディタで視覚化（デバッグ用）
    void OnDrawGizmos()
    {
        // 種類に応じて色を変える
        switch (fishTypeName)
        {
            case "Maguro":
                Gizmos.color = Color.red;
                break;
            case "Tamago":
                Gizmos.color = Color.yellow;
                break;
            case "Salmon":
                Gizmos.color = new Color(1f, 0.5f, 0.3f); // オレンジっぽい色
                break;
            default:
                Gizmos.color = Color.white;
                break;
        }
        Gizmos.DrawWireSphere(transform.position, 0.1f);
    }
}
