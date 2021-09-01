using UnityEngine;
using UnityEngine.SceneManagement;



public class GameOverScreen : MonoBehaviour
{
    public void RestartButton()
    {
        SceneManager.LoadScene("PerlinWalk");
    }

    public void ExitButton()
    {
        Application.Quit();
        Debug.Log("Game Closed!");
    }
}
