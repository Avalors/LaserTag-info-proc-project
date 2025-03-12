using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class LoginScene : MonoBehaviour
{
    public TMP_InputField userNameInput;
    public TMP_InputField passwordInput;
    public TMP_Text textComponent;
    public void LoginButton()
    {
        string username = userNameInput.text;
        string password = passwordInput.text;
        int response = Database.Instance.AttemptLogIn(username, password);
        if (response == 1)
        {
            SceneManager.LoadScene("SampleScene");
        }
        else if (response == 0)
        {
            textComponent.text = "Invalid username or password";
        }
        else
        {
            textComponent.text = "Failed to connect to database";
        }
    }
    public void BackButton()
    {
        SceneManager.LoadScene("LoginOrSignup");
    }
}
