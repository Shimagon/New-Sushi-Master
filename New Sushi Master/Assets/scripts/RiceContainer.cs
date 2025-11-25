using UnityEngine;
using Valve.VR.InteractionSystem;

/// <summary>
/// 米釜（お釜）- 掴んだら米を左手に生成する
/// </summary>
[RequireComponent(typeof(Interactable))]
public class RiceContainer : MonoBehaviour
{
    [Header("Rice Settings")]
    [Tooltip("生成する米のPrefab")]
    public GameObject ricePrefab;

    [Tooltip("米の生成位置オフセット（手からの相対位置）")]
    public Vector3 riceSpawnOffset = new Vector3(0f, 0.1f, 0f);

    [Header("Audio")]
    [Tooltip("米を掴んだときの効果音")]
    public AudioClip riceGrabSound;

    [Header("Settings")]
    [Tooltip("連続生成のクールダウン時間（秒）")]
    public float cooldownTime = 0.5f;

    private Interactable interactable;
    private float lastSpawnTime = 0f;

    void Awake()
    {
        interactable = GetComponent<Interactable>();

        // 掴んだときのイベントに登録
        interactable.onAttachedToHand += OnAttachedToHand;
    }

    void OnDestroy()
    {
        if (interactable != null)
        {
            interactable.onAttachedToHand -= OnAttachedToHand;
        }
    }

    private void OnAttachedToHand(Hand hand)
    {
        // クールダウンチェック
        if (Time.time - lastSpawnTime < cooldownTime)
        {
            Debug.Log("米生成のクールダウン中です");
            return;
        }

        // 米のPrefabが設定されているか確認
        if (ricePrefab == null)
        {
            Debug.LogError("米のPrefabが設定されていません！");
            return;
        }

        // 米を生成する位置（手の位置 + オフセット）
        Vector3 spawnPosition = hand.transform.position + hand.transform.TransformDirection(riceSpawnOffset);
        Quaternion spawnRotation = hand.transform.rotation;

        // 米を生成
        GameObject newRice = Instantiate(ricePrefab, spawnPosition, spawnRotation);

        Debug.Log($"米を{hand.name}に生成しました");

        // 効果音を再生
        if (riceGrabSound != null)
        {
            AudioSource.PlayClipAtPoint(riceGrabSound, spawnPosition);
        }

        // 生成した米を手にアタッチ
        hand.AttachObject(newRice, GrabTypes.Grip);

        // クールダウン時間を記録
        lastSpawnTime = Time.time;

        // すぐに釜を手から離す（釜は持ち続けない）
        hand.DetachObject(gameObject);
    }
}
