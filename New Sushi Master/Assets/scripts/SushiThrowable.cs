using UnityEngine;
using Valve.VR.InteractionSystem;

/// <summary>
/// 投げられる寿司オブジェクト
/// お客さんに当たったらスコアを追加する
/// </summary>
[RequireComponent(typeof(Interactable))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(VelocityEstimator))]
public class SushiThrowable : MonoBehaviour
{
    [Header("Sushi Settings")]
    [Tooltip("この寿司の種類")]
    public string sushiType = "Maguro";

    [Tooltip("この寿司の得点")]
    public int pointValue = 100;

    [Header("Throw Settings")]
    [Tooltip("投げる力の倍率")]
    public float throwMultiplier = 1.5f;

    [Tooltip("投げた後に自動的に消えるまでの時間（秒）")]
    public float autoDestroyTime = 10f;

    [Header("Effects")]
    [Tooltip("お客さんに当たったときのエフェクト")]
    public GameObject hitEffect;

    [Tooltip("お客さんに当たったときの効果音")]
    public AudioClip hitSound;

    [Tooltip("投げたときの効果音")]
    public AudioClip throwSound;

    private Interactable interactable;
    private Rigidbody rb;
    private VelocityEstimator velocityEstimator;
    private bool hasBeenThrown = false;
    private bool hasHitTarget = false;
    private float throwTime = 0f;

    // アタッチフラグ（SteamVR用）
    private Hand.AttachmentFlags attachmentFlags =
        Hand.AttachmentFlags.ParentToHand |
        Hand.AttachmentFlags.DetachFromOtherHand |
        Hand.AttachmentFlags.TurnOnKinematic;

    void Awake()
    {
        interactable = GetComponent<Interactable>();
        rb = GetComponent<Rigidbody>();
        velocityEstimator = GetComponent<VelocityEstimator>();

        // SushiTypeコンポーネントがあれば、そこから種類を取得
        SushiType sushiTypeComponent = GetComponent<SushiType>();
        if (sushiTypeComponent != null)
        {
            sushiType = sushiTypeComponent.GetSushiType();
            pointValue = sushiTypeComponent.GetScore();
        }

        // Interactableのイベントに登録
        interactable.onAttachedToHand += OnAttachedToHand;
        interactable.onDetachedFromHand += OnDetachedFromHand;

        // Rigidbodyの設定
        if (rb != null)
        {
            rb.maxAngularVelocity = 50.0f;
        }
    }

    void OnDestroy()
    {
        if (interactable != null)
        {
            interactable.onAttachedToHand -= OnAttachedToHand;
            interactable.onDetachedFromHand -= OnDetachedFromHand;
        }
    }

    void Update()
    {
        // 投げられた後、一定時間経過したら自動的に消す
        if (hasBeenThrown)
        {
            throwTime += Time.deltaTime;
            if (throwTime >= autoDestroyTime)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnAttachedToHand(Hand hand)
    {
        Debug.Log($"{sushiType}が{hand.name}に掴まれました");
        hasBeenThrown = false;
        throwTime = 0f;
    }

    private void OnDetachedFromHand(Hand hand)
    {
        Debug.Log($"{sushiType}が{hand.name}から離されました（投げられました）");

        // 投げる処理
        ThrowObject(hand);
    }

    private void ThrowObject(Hand hand)
    {
        hasBeenThrown = true;
        throwTime = 0f;

        // 手の速度を取得して寿司に適用
        if (velocityEstimator != null && rb != null)
        {
            Vector3 velocity = velocityEstimator.GetVelocityEstimate() * throwMultiplier;
            Vector3 angularVelocity = velocityEstimator.GetAngularVelocityEstimate();

            rb.velocity = velocity;
            rb.angularVelocity = angularVelocity;

            Debug.Log($"寿司を投げました！速度: {velocity.magnitude:F2} m/s");
        }

        // 投げた音を再生
        if (throwSound != null)
        {
            AudioSource.PlayClipAtPoint(throwSound, transform.position);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // まだ投げられていない、または既に当たっている場合は無視
        if (!hasBeenThrown || hasHitTarget)
        {
            return;
        }

        GameObject hitObject = collision.gameObject;

        // お客さんに当たったかチェック
        Customer customer = hitObject.GetComponent<Customer>();
        if (customer != null)
        {
            // お客さんに当たった！
            OnHitCustomer(customer, collision.contacts[0].point);
        }
        else if (hitObject.CompareTag("Customer"))
        {
            // タグでもチェック
            OnHitCustomerSimple(hitObject, collision.contacts[0].point);
        }
    }

    /// <summary>
    /// お客さんに当たったときの処理
    /// </summary>
    private void OnHitCustomer(Customer customer, Vector3 hitPoint)
    {
        hasHitTarget = true;

        // お客さんが求めている寿司かチェック
        bool isCorrectSushi = customer.WantsSushi(sushiType);
        int earnedPoints = isCorrectSushi ? pointValue : pointValue / 2;

        // スコアを追加
        ScoreManager.Instance?.AddScore(earnedPoints);

        Debug.Log($"お客さんに{sushiType}が当たった！ {earnedPoints}点獲得！");

        // お客さんに通知
        customer.ReceiveSushi(sushiType, isCorrectSushi);

        // エフェクトと音を再生
        PlayHitEffects(hitPoint);

        // 寿司を消す
        Destroy(gameObject, 0.1f);
    }

    /// <summary>
    /// お客さんに当たったときの処理（シンプル版）
    /// </summary>
    private void OnHitCustomerSimple(GameObject customerObject, Vector3 hitPoint)
    {
        hasHitTarget = true;

        // スコアを追加
        ScoreManager.Instance?.AddScore(pointValue);

        Debug.Log($"お客さんに{sushiType}が当たった！ {pointValue}点獲得！");

        // エフェクトと音を再生
        PlayHitEffects(hitPoint);

        // 寿司を消す
        Destroy(gameObject, 0.1f);
    }

    /// <summary>
    /// ヒットエフェクトと音を再生
    /// </summary>
    private void PlayHitEffects(Vector3 position)
    {
        // エフェクト
        if (hitEffect != null)
        {
            GameObject effect = Instantiate(hitEffect, position, Quaternion.identity);
            Destroy(effect, 3f);
        }

        // 効果音
        if (hitSound != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, position);
        }
    }

    /// <summary>
    /// 投げられたかどうか
    /// </summary>
    public bool HasBeenThrown()
    {
        return hasBeenThrown;
    }

    /// <summary>
    /// 寿司の種類を取得
    /// </summary>
    public string GetSushiType()
    {
        return sushiType;
    }
}
