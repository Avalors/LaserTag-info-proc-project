using UnityEngine;
using UnityEngine.SceneManagement;
public class LoginOrSignup : MonoBehaviour
{
    public void LoginButton()
    {
        SceneManager.LoadScene("LoginScene");
    }
    public void SignupButton()
    {
        SceneManager.LoadScene("SignupScene");
    }
    public void BackButton()
    {
        SceneManager.LoadScene("StartMenu");
    }
}
