using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuButton : MonoBehaviour
{
    public string sceneToLoad;  // シーン名
    public bool isQuitButton = false;

    private void OnTriggerEnter(Collider other)
    {
        // プレイヤーの手（コントローラー）にタグをつけて判定
        if (other.CompareTag("PlayerHand"))
        {
            if (isQuitButton)
            {
                QuitGame();
            }
            else
            {
                StartGame();
            }
        }
    }

    private void StartGame()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
