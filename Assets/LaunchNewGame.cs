using UnityEngine;
using UnityEngine.SceneManagement;

public class LaunchNewGame : MonoBehaviour
{
    public void StartNewGame()
    {
        SceneManager.LoadScene("TakeOffFromIsland");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene("TakeOffFromIsland");
        }
    }
}