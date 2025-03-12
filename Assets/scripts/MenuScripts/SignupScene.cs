using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class SignupScene : MonoBehaviour
{
    public TMP_InputField userNameInput;
    public TMP_InputField passwordInput;
    public TMP_Text textComponent;
    public void SignupButton()
    {
        string username = userNameInput.text;
        string password = passwordInput.text;
        int response = Database.Instance.AttemptSignUp(username, password);
        if (response == 1)
        {
            SceneManager.LoadScene("SampleScene");
        }
        else if (response == 0)
        {
            textComponent.text = "This username is already taken";
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