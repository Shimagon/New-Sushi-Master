using UnityEngine;

/// <summary>
/// 完成した寿司の種類を識別するためのコンポーネント
/// このスクリプトを寿司のPrefabにアタッチして、種類を設定します
/// </summary>
public class SushiType : MonoBehaviour
{
    [Header("寿司の種類")]
    [Tooltip("寿司の種類名（Maguro, Tamago, Salmon）")]
    public string sushiTypeName = "Maguro";

    [Header("表示名")]
    [Tooltip("UI表示用の日本語名")]
    public string displayName = "マグロ";

    [Header("スコア設定")]
    [Tooltip("この寿司の得点")]
    public int scoreValue = 100;

    /// <summary>
    /// 寿司の種類を取得
    /// </summary>
    public string GetSushiType()
    {
        return sushiTypeName;
    }

    /// <summary>
    /// 表示名を取得
    /// </summary>
    public string GetDisplayName()
    {
        return displayName;
    }

    /// <summary>
    /// スコアを取得
    /// </summary>
    public int GetScore()
    {
        return scoreValue;
    }

    // エディタで視覚化（デバッグ用）
    void OnDrawGizmos()
    {
        // 種類に応じて色を変える
        switch (sushiTypeName)
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
        Gizmos.DrawWireCube(transform.position, Vector3.one * 0.2f);
    }
}
