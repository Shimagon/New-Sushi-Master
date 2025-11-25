using UnityEngine;

public class RiceController : MonoBehaviour
{
    // Inspectorから設定できるように、寿司のPrefabを格納する変数
    public GameObject sushiPrefab;

    // 衝突を検知したときに呼ばれるUnityの標準メソッド
    void OnCollisionEnter(Collision collision)
    {
        // 衝突した相手（魚）のGameObjectを取得
        GameObject otherObject = collision.gameObject;

        // 衝突相手のタグが「Fish」であることを確認
        if (otherObject.CompareTag("Fish"))
        {
            // 寿司を生成するメソッドを呼び出す
            MakeSushi(otherObject);
        }
    }

    void MakeSushi(GameObject fish)
    {
        // 1. 寿司の生成位置を決定 (ここでは米の位置を使用)
        Vector3 spawnPosition = transform.position;
        Quaternion spawnRotation = Quaternion.identity; // 回転はなし

        // 2. 寿司オブジェクトの生成
        // Instantiate(生成したいPrefab, 位置, 回転)
        GameObject newSushi = Instantiate(sushiPrefab, spawnPosition, spawnRotation);

        // 3. 元のオブジェクト（米と魚）を破棄
        Destroy(gameObject); // このスクリプトがアタッチされている米自身
        Destroy(fish);       // 衝突した相手の魚
    }
}