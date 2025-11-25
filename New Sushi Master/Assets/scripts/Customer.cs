using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// お客さんのクラス
/// 特定の寿司を要求し、正しい寿司が来たら喜ぶ
/// </summary>
public class Customer : MonoBehaviour
{
    [Header("Customer Settings")]
    [Tooltip("要求する寿司の種類")]
    public string requestedSushiType = "Maguro";

    [Tooltip("お客さんの名前")]
    public string customerName = "Customer";

    [Tooltip("要求を表示するまでの待機時間（秒）")]
    public float requestDelay = 2f;

    [Header("UI References")]
    [Tooltip("要求を表示するテキスト（ワールドスペースUI）")]
    public Text requestText;

    [Tooltip("要求を表示するキャンバス")]
    public GameObject requestCanvas;

    [Tooltip("吹き出しアイコン（オプション）")]
    public Image requestIcon;

    [Header("Feedback")]
    [Tooltip("正しい寿司を受け取ったときのエフェクト")]
    public GameObject happyEffect;

    [Tooltip("間違った寿司を受け取ったときのエフェクト")]
    public GameObject sadEffect;

    [Tooltip("正しい寿司を受け取ったときの効果音")]
    public AudioClip happySound;

    [Tooltip("間違った寿司を受け取ったときの効果音")]
    public AudioClip sadSound;

    [Header("Animation")]
    [Tooltip("Animatorコンポーネント（オプション）")]
    public Animator animator;

    [Tooltip("喜ぶアニメーションのトリガー名")]
    public string happyAnimationTrigger = "Happy";

    [Tooltip("悲しむアニメーションのトリガー名")]
    public string sadAnimationTrigger = "Sad";

    [Header("Behavior")]
    [Tooltip("寿司を受け取った後に消えるまでの時間（秒）")]
    public float disappearDelay = 3f;

    [Tooltip("次のお客さんを生成する場合のPrefab")]
    public GameObject nextCustomerPrefab;

    private bool hasReceivedSushi = false;
    private bool isRequestShown = false;

    void Start()
    {
        // 最初は要求を非表示
        if (requestCanvas != null)
        {
            requestCanvas.SetActive(false);
        }

        // 少し待ってから要求を表示
        StartCoroutine(ShowRequestAfterDelay());
    }

    /// <summary>
    /// 遅延して要求を表示
    /// </summary>
    private IEnumerator ShowRequestAfterDelay()
    {
        yield return new WaitForSeconds(requestDelay);

        ShowRequest();
    }

    /// <summary>
    /// 要求を表示
    /// </summary>
    private void ShowRequest()
    {
        if (isRequestShown) return;

        isRequestShown = true;

        if (requestCanvas != null)
        {
            requestCanvas.SetActive(true);
        }

        if (requestText != null)
        {
            requestText.text = $"{requestedSushiType}が欲しい！";
        }

        Debug.Log($"{customerName}が{requestedSushiType}を要求しています");
    }

    /// <summary>
    /// この寿司が欲しいかチェック
    /// </summary>
    public bool WantsSushi(string sushiType)
    {
        return sushiType == requestedSushiType;
    }

    /// <summary>
    /// 寿司を受け取る
    /// </summary>
    public void ReceiveSushi(string sushiType, bool isCorrect)
    {
        if (hasReceivedSushi) return;

        hasReceivedSushi = true;

        Debug.Log($"{customerName}が{sushiType}を受け取りました（正解: {isCorrect}）");

        // 要求表示を非表示
        if (requestCanvas != null)
        {
            requestCanvas.SetActive(false);
        }

        if (isCorrect)
        {
            OnReceiveCorrectSushi();
        }
        else
        {
            OnReceiveWrongSushi();
        }

        // 少し待ってから消える
        StartCoroutine(DisappearAfterDelay());
    }

    /// <summary>
    /// 正しい寿司を受け取ったときの処理
    /// </summary>
    private void OnReceiveCorrectSushi()
    {
        Debug.Log($"{customerName}は喜んでいます！");

        // エフェクト
        if (happyEffect != null)
        {
            GameObject effect = Instantiate(happyEffect, transform.position + Vector3.up * 2f, Quaternion.identity);
            Destroy(effect, 3f);
        }

        // 効果音
        if (happySound != null)
        {
            AudioSource.PlayClipAtPoint(happySound, transform.position);
        }

        // アニメーション
        if (animator != null && !string.IsNullOrEmpty(happyAnimationTrigger))
        {
            animator.SetTrigger(happyAnimationTrigger);
        }
    }

    /// <summary>
    /// 間違った寿司を受け取ったときの処理
    /// </summary>
    private void OnReceiveWrongSushi()
    {
        Debug.Log($"{customerName}は不満そうです...");

        // エフェクト
        if (sadEffect != null)
        {
            GameObject effect = Instantiate(sadEffect, transform.position + Vector3.up * 2f, Quaternion.identity);
            Destroy(effect, 3f);
        }

        // 効果音
        if (sadSound != null)
        {
            AudioSource.PlayClipAtPoint(sadSound, transform.position);
        }

        // アニメーション
        if (animator != null && !string.IsNullOrEmpty(sadAnimationTrigger))
        {
            animator.SetTrigger(sadAnimationTrigger);
        }
    }

    /// <summary>
    /// 遅延して消える
    /// </summary>
    private IEnumerator DisappearAfterDelay()
    {
        yield return new WaitForSeconds(disappearDelay);

        // 次のお客さんを生成（オプション）
        if (nextCustomerPrefab != null)
        {
            Instantiate(nextCustomerPrefab, transform.position, transform.rotation);
        }

        // 自分を消す
        Destroy(gameObject);
    }

    /// <summary>
    /// 要求している寿司の種類を取得
    /// </summary>
    public string GetRequestedSushiType()
    {
        return requestedSushiType;
    }

    /// <summary>
    /// 寿司を受け取ったかどうか
    /// </summary>
    public bool HasReceivedSushi()
    {
        return hasReceivedSushi;
    }

    // エディタで要求を視覚化（デバッグ用）
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * 2f, 0.3f);
    }
}
