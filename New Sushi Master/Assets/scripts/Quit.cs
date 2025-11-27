using UnityEngine;

public class QuitButton : MonoBehaviour
{
    void Update()
    {
        // 左クリックが押されたとき
        if (Input.GetMouseButtonDown(0))
        {
            if (Camera.main == null) return; // カメラがない場合は処理しない

            // マウス位置からRayを飛ばしてクリック判定
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == transform) // このCubeがクリックされた場合
                {
                    QuitGame();
                }
            }
        }
    }

    void QuitGame()
    {
        Debug.Log("ゲームを終了します");

#if UNITY_EDITOR
        // Unityエディターの場合はプレイ停止
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // ビルドしたゲームを終了
        Application.Quit();
#endif
    }
}

