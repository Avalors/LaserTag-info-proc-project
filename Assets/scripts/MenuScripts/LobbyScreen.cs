using UnityEngine;
using UnityEngine.SceneManagement;
public class LobbyScreen : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
