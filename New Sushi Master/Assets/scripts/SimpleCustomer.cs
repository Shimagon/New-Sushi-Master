using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// シンプルなお客さん（テスト用）
/// VRなしでも動作確認できる
/// </summary>
public class SimpleCustomer : MonoBehaviour
{
    [Header("Customer Settings")]
    [Tooltip("要求する寿司の種類")]
    public string requestedSushiType = "Maguro";

    [Header("UI (Optional)")]
    [Tooltip("要求を表示するテキスト")]
    public Text requestText;

    [Header("Debug")]
    [Tooltip("デバッグモード（コンソールにログを出す）")]
    public bool debugMode = true;

    void Start()
    {
        // 要求を表示
        if (requestText != null)
        {
            requestText.text = $"{requestedSushiType}が欲しい！";
        }

        if (debugMode)
        {
            Debug.Log($"お客さんが{requestedSushiType}を要求しています");
        }

        // 頭上に要求を表示（Gizmo用）
        name = $"Customer ({requestedSushiType})";
    }

    void OnCollisionEnter(Collision collision)
    {
        // 寿司が当たったかチェック
        SushiThrowable sushi = collision.gameObject.GetComponent<SushiThrowable>();
        if (sushi != null)
        {
            string sushiType = sushi.GetSushiType();
            bool isCorrect = (sushiType == requestedSushiType);

            if (debugMode)
            {
                Debug.Log($"お客さんに{sushiType}が当たった！ 正解: {isCorrect}");
            }

            // スコアを追加
            if (ScoreManager.Instance != null)
            {
                int points = isCorrect ? 100 : 50;
                ScoreManager.Instance.AddScore(points);
            }

            // お客さんを消す
            Destroy(gameObject, 1f);

            // エフェクト（オプション）
            ShowFeedback(isCorrect);
        }
    }

    void ShowFeedback(bool isCorrect)
    {
        if (isCorrect)
        {
            Debug.Log("お客さんは喜んでいます！");
        }
        else
        {
            Debug.Log("お客さんは不満そうです...");
        }
    }

    // エディタでの視覚化
    void OnDrawGizmos()
    {
        // 種類に応じて色を変える
        switch (requestedSushiType)
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

        // 頭上にアイコン表示
        Gizmos.DrawWireSphere(transform.position + Vector3.up * 2f, 0.3f);
    }
}
