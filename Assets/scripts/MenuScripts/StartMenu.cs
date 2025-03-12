using UnityEngine;
using UnityEngine.SceneManagement;
public class StartMenu : MonoBehaviour
{
    public void PlayButton()
    {
        
        SceneManager.LoadScene("LoginOrSignup");
    }
    public void QuitButton()
    {
        Application.Quit();
    }
}
