using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public void PlayButton()
    {
        SceneManager.LoadScene("HostOrJoin");
    }
    public void QuitButton()
    {
        Application.Quit();
    }

}
