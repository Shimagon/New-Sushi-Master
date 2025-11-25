using UnityEngine;
using Valve.VR.InteractionSystem;

/// <summary>
/// 右手で掴む魚（ネタ）オブジェクト
/// 米と衝突したときに寿司を生成する
/// </summary>
[RequireComponent(typeof(Interactable))]
[RequireComponent(typeof(Rigidbody))]
public class GrabbableFish : MonoBehaviour
{
    [Header("Fish Settings")]
    [Tooltip("この魚の種類（マグロ、サーモンなど）")]
    public string fishType = "Maguro";

    [Header("VR Hand Settings")]
    [Tooltip("このオブジェクトをアタッチする手（右手を推奨）")]
    public Hand preferredHand;

    [Header("Visual Feedback")]
    [Tooltip("手で持ったときの色")]
    public Color heldColor = Color.yellow;

    [Tooltip("通常時の色")]
    public Color normalColor = Color.white;

    private Interactable interactable;
    private Rigidbody rb;
    private bool isHeldByHand = false;
    private Hand currentHand;
    private Renderer[] renderers;
    private Color[] originalColors;

    void Awake()
    {
        interactable = GetComponent<Interactable>();
        rb = GetComponent<Rigidbody>();

        // FishTypeコンポーネントがあれば、そこから種類を取得
        FishType fishTypeComponent = GetComponent<FishType>();
        if (fishTypeComponent != null)
        {
            fishType = fishTypeComponent.GetFishType();
            Debug.Log($"FishTypeコンポーネントから魚の種類を取得: {fishType}");
        }

        // レンダラーを取得（色変更用）
        renderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_Color"))
            {
                originalColors[i] = renderers[i].material.color;
            }
        }

        // Interactableのイベントに登録
        interactable.onAttachedToHand += OnAttachedToHand;
        interactable.onDetachedFromHand += OnDetachedFromHand;
    }

    void OnDestroy()
    {
        if (interactable != null)
        {
            interactable.onAttachedToHand -= OnAttachedToHand;
            interactable.onDetachedFromHand -= OnDetachedFromHand;
        }
    }

    private void OnAttachedToHand(Hand hand)
    {
        isHeldByHand = true;
        currentHand = hand;
        Debug.Log($"{fishType}が{hand.name}に掴まれました");

        // 色を変更（視覚的フィードバック）
        SetColor(heldColor);
    }

    private void OnDetachedFromHand(Hand hand)
    {
        isHeldByHand = false;
        currentHand = null;
        Debug.Log($"{fishType}が{hand.name}から離されました");

        // 色を元に戻す
        ResetColor();
    }

    void OnCollisionEnter(Collision collision)
    {
        // 手で持っているときのみチェック
        if (!isHeldByHand)
        {
            return;
        }

        // 衝突相手が米かチェック
        GameObject otherObject = collision.gameObject;

        // GrabbableRiceスクリプトがあるかチェック
        GrabbableRice rice = otherObject.GetComponent<GrabbableRice>();

        if (rice != null && rice.IsHeldByHand())
        {
            // 米側で寿司生成処理を行うので、ここでは何もしない
            Debug.Log($"{fishType}が米と衝突しました");
        }
    }

    /// <summary>
    /// 色を変更
    /// </summary>
    private void SetColor(Color color)
    {
        foreach (Renderer rend in renderers)
        {
            if (rend.material.HasProperty("_Color"))
            {
                rend.material.color = color;
            }
        }
    }

    /// <summary>
    /// 色を元に戻す
    /// </summary>
    private void ResetColor()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_Color"))
            {
                renderers[i].material.color = originalColors[i];
            }
        }
    }

    /// <summary>
    /// 現在手で持っているかどうか
    /// </summary>
    public bool IsHeldByHand()
    {
        return isHeldByHand;
    }

    /// <summary>
    /// 現在持っている手を取得
    /// </summary>
    public Hand GetCurrentHand()
    {
        return currentHand;
    }

    /// <summary>
    /// 魚の種類を取得
    /// </summary>
    public string GetFishType()
    {
        return fishType;
    }
}
