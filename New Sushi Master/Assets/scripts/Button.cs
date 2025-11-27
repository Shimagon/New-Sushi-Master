using UnityEngine;
using UnityEngine.SceneManagement;

public class CubeButton : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 左クリック
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == transform) // このCubeがクリックされたら
                {
                    StartGame();
                }
            }
        }
    }

    void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
