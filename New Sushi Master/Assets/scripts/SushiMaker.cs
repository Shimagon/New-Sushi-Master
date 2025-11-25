using UnityEngine;

public class SushiMaker : MonoBehaviour
{
    // Inspectorから設定するために公開します
    // 各寿司の種類に対応するPrefabを設定
    public GameObject maguroSushiPrefab;    // マグロ寿司
    public GameObject tamagoSushiPrefab;    // たまご寿司
    public GameObject salmonSushiPrefab;    // サーモン寿司

    // Unityの物理衝突を検知する関数 (Is TriggerがOFFのとき)
    void OnCollisionEnter(Collision collision)
    {
        // 衝突した相手のゲームオブジェクトを取得
        GameObject otherObject = collision.gameObject;

        // 衝突相手のタグが「Fish」であることを確認
        if (otherObject.CompareTag("Fish"))
        {
            // 米と魚がぶつかったら寿司を生成し、元を破棄する
            MakeSushi(otherObject);
        }
    }

    void MakeSushi(GameObject fishObject)
    {
        Vector3 spawnPosition = transform.position;

        // 🍣 ここで回転を上書きします 🍣
        // X軸周りに-90度回転させて水平になるようにします。
        // (プレハブ編集で設定したのと同じ回転をコードで強制します)
        Quaternion desiredRotation = Quaternion.Euler(-90f, 0f, 0f);

        // 魚の種類に応じて生成する寿司を決定
        GameObject sushiPrefabToUse = GetSushiPrefabForFish(fishObject);

        if (sushiPrefabToUse != null)
        {
            // 生成時に desiredRotation を指定
            GameObject newSushi = Instantiate(sushiPrefabToUse, spawnPosition, desiredRotation);

            // 元のオブジェクトを破棄
            Destroy(gameObject);
            Destroy(fishObject);
        }
        else
        {
            Debug.LogWarning("寿司のPrefabが設定されていません: " + fishObject.name);
        }
    }

    // 魚の種類に応じた寿司のPrefabを返す
    GameObject GetSushiPrefabForFish(GameObject fishObject)
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
}