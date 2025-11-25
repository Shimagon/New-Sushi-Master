using UnityEngine;
using Valve.VR.InteractionSystem;

/// <summary>
/// 左手で持つ米オブジェクト
/// 魚（ネタ）と衝突したときに寿司を生成する
/// </summary>
[RequireComponent(typeof(Interactable))]
[RequireComponent(typeof(Rigidbody))]
public class GrabbableRice : MonoBehaviour
{
    [Header("Sushi Settings")]
    [Tooltip("マグロ寿司のPrefab")]
    public GameObject maguroSushiPrefab;

    [Tooltip("たまご寿司のPrefab")]
    public GameObject tamagoSushiPrefab;

    [Tooltip("サーモン寿司のPrefab")]
    public GameObject salmonSushiPrefab;

    [Tooltip("寿司の生成位置オフセット")]
    public Vector3 sushiSpawnOffset = Vector3.zero;

    [Tooltip("寿司の回転（Eulerアングル）")]
    public Vector3 sushiRotation = new Vector3(-90f, 0f, 0f);

    [Header("VR Hand Settings")]
    [Tooltip("このオブジェクトをアタッチする手（左手を推奨）")]
    public Hand preferredHand;

    [Header("Audio")]
    [Tooltip("寿司生成時の効果音")]
    public AudioClip sushiMakeSound;

    [Header("Effects")]
    [Tooltip("寿司生成時のエフェクト（CFXR Magic Poofなど）")]
    public GameObject sushiMakeEffect;

    [Header("Debug")]
    [Tooltip("デバッグモード：手で持たなくても寿司を作れる")]
    public bool allowWithoutHand = false;

    private Interactable interactable;
    private Rigidbody rb;
    private bool isHeldByHand = false;
    private Hand currentHand;
    private bool hasCreatedSushi = false; // 寿司を作ったかどうか

    void Awake()
    {
        interactable = GetComponent<Interactable>();
        rb = GetComponent<Rigidbody>();

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
        Debug.Log($"米が{hand.name}に掴まれました");
    }

    private void OnDetachedFromHand(Hand hand)
    {
        isHeldByHand = false;
        currentHand = null;
        Debug.Log($"米が{hand.name}から離されました");
    }

    void OnCollisionEnter(Collision collision)
    {
        // 既に寿司を作っていたら無視（重複防止）
        if (hasCreatedSushi)
        {
            return;
        }

        // 手で持っているときのみ寿司を作る（デバッグモードではスキップ）
        if (!isHeldByHand && !allowWithoutHand)
        {
            return;
        }

        // 衝突相手が魚（ネタ）かチェック
        GameObject otherObject = collision.gameObject;

        // 魚のスクリプトがあるかチェック
        GrabbableFish fish = otherObject.GetComponent<GrabbableFish>();

        if (fish != null)
        {
            // GrabbableFishがある場合
            // デバッグモードなら手で持ってなくてもOK
            if (allowWithoutHand || fish.IsHeldByHand())
            {
                hasCreatedSushi = true; // フラグを立てる
                MakeSushi(fish, collision.contacts[0].point);
            }
        }
        else if (otherObject.CompareTag("Fish"))
        {
            // タグだけでもチェック（後方互換性）
            hasCreatedSushi = true; // フラグを立てる
            MakeSushiSimple(otherObject, collision.contacts[0].point);
        }
    }

    /// <summary>
    /// 寿司を生成する（GrabbableFish使用）
    /// </summary>
    void MakeSushi(GrabbableFish fish, Vector3 collisionPoint)
    {
        // 魚の種類に応じた寿司Prefabを取得
        GameObject sushiPrefab = GetSushiPrefabForFish(fish.gameObject);

        if (sushiPrefab == null)
        {
            Debug.LogWarning("寿司のPrefabが設定されていません！魚の種類: " + fish.GetFishType());
            return;
        }

        // 寿司の生成位置を計算（衝突点 + オフセット）
        Vector3 spawnPosition = collisionPoint + sushiSpawnOffset;
        Quaternion spawnRotation = Quaternion.Euler(sushiRotation);

        // 寿司を生成
        GameObject newSushi = Instantiate(sushiPrefab, spawnPosition, spawnRotation);

        // エフェクトを再生（寿司の位置に）
        if (sushiMakeEffect != null)
        {
            GameObject effect = Instantiate(sushiMakeEffect, newSushi.transform.position, Quaternion.identity);
            effect.transform.localScale = Vector3.one * 2f; // エフェクトを2倍に拡大

            // エフェクトを3秒後に自動削除
            Destroy(effect, 3f);
        }

        // 効果音を再生
        if (sushiMakeSound != null)
        {
            AudioSource.PlayClipAtPoint(sushiMakeSound, spawnPosition);
        }

        // 寿司に投げられるコンポーネントがあれば、右手にアタッチ
        SushiThrowable throwable = newSushi.GetComponent<SushiThrowable>();
        if (throwable != null && currentHand != null)
        {
            // 右手を取得（現在は左手なので、反対の手を探す）
            Hand otherHand = GetOtherHand(currentHand);
            if (otherHand != null)
            {
                // 右手に寿司を自動アタッチ
                otherHand.AttachObject(newSushi, GrabTypes.Grip);
                Debug.Log("寿司を右手にアタッチしました");
            }
        }

        // 米と魚を破棄
        Destroy(gameObject);
        Destroy(fish.gameObject);
    }

    /// <summary>
    /// 寿司を生成する（シンプル版）
    /// </summary>
    void MakeSushiSimple(GameObject fishObject, Vector3 collisionPoint)
    {
        // 魚の種類に応じた寿司Prefabを取得
        GameObject sushiPrefab = GetSushiPrefabForFish(fishObject);

        if (sushiPrefab == null)
        {
            Debug.LogError("寿司のPrefabが設定されていません！");
            return;
        }

        Vector3 spawnPosition = collisionPoint + sushiSpawnOffset;
        Quaternion spawnRotation = Quaternion.Euler(sushiRotation);

        GameObject newSushi = Instantiate(sushiPrefab, spawnPosition, spawnRotation);

        if (sushiMakeSound != null)
        {
            AudioSource.PlayClipAtPoint(sushiMakeSound, spawnPosition);
        }

        if (sushiMakeEffect != null)
        {
            GameObject effect = Instantiate(sushiMakeEffect, newSushi.transform.position, Quaternion.identity);
            effect.transform.localScale = Vector3.one * 2f;

            // エフェクトを3秒後に自動削除
            Destroy(effect, 3f);
        }

        Destroy(gameObject);
        Destroy(fishObject);
    }

    /// <summary>
    /// 魚の種類に応じた寿司のPrefabを取得
    /// </summary>
    private GameObject GetSushiPrefabForFish(GameObject fishObject)
    {
        // 魚オブジェクトにFishTypeコンポーネントがあれば、それを使用
        FishType fishType = fishObject.GetComponent<FishType>();
        if (fishType != null)
        {
            switch (fishType.fishTypeName)
            {
                case "Maguro":
                    return maguroSushiPrefab;
                case "Tamago":
                    return tamagoSushiPrefab;
                case "Salmon":
                    return salmonSushiPrefab;
                default:
                    Debug.LogWarning("不明な魚の種類: " + fishType.fishTypeName);
                    return maguroSushiPrefab; // デフォルトはマグロ
            }
        }

        // GrabbableFishコンポーネントから種類を取得
        GrabbableFish grabbableFish = fishObject.GetComponent<GrabbableFish>();
        if (grabbableFish != null)
        {
            string fishTypeName = grabbableFish.GetFishType();
            if (fishTypeName == "Tamago")
                return tamagoSushiPrefab;
            else if (fishTypeName == "Salmon")
                return salmonSushiPrefab;
            else
                return maguroSushiPrefab;
        }

        // FishTypeコンポーネントがない場合は名前で判定
        string fishName = fishObject.name.ToLower();
        if (fishName.Contains("maguro") || fishName.Contains("tuna"))
        {
            return maguroSushiPrefab;
        }
        else if (fishName.Contains("tamago") || fishName.Contains("egg"))
        {
            return tamagoSushiPrefab;
        }
        else if (fishName.Contains("salmon") || fishName.Contains("sake"))
        {
            return salmonSushiPrefab;
        }

        // デフォルトはマグロ
        Debug.LogWarning("魚の種類を判定できませんでした。マグロとして扱います: " + fishObject.name);
        return maguroSushiPrefab;
    }

    /// <summary>
    /// 反対の手を取得
    /// </summary>
    private Hand GetOtherHand(Hand hand)
    {
        if (hand == null) return null;

        // Playerオブジェクトから両手を取得
        Player player = hand.GetComponentInParent<Player>();
        if (player != null)
        {
            if (player.leftHand == hand)
                return player.rightHand;
            else if (player.rightHand == hand)
                return player.leftHand;
        }

        return null;
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
}
