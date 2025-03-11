using UnityEngine;
using UnityEngine.SceneManagement;
public class HostOrJoin : MonoBehaviour
{
    public void JoinButton()
    {
        SceneManager.LoadScene("EnterCodeScreen");
    }

    public void HostButton()
    {
        SceneManager.LoadScene("LobbyScreen");
    }

}