using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class EnterCodeScreen : MonoBehaviour
{
    public TMP_InputField inputField;


    public void JoinLobby()
    {
        string lobbyCode = inputField.text;
        SceneManager.LoadScene("LobbyScreen");
    }

}