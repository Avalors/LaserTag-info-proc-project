using UnityEngine;
using Alteruna;

public class MultiplayerUIManager : MonoBehaviour
{
    public GameObject roomPanelMenu;
    public GameObject HUD;

    public void HideRoomMenujoin(Multiplayer multiplayer, Room room, User ID){
        roomPanelMenu.SetActive(false);
        Debug.Log("Room menu hidden after joining/creating a room.");
    }

    public void ShowHudjoin(Multiplayer multiplayer, Room room, User ID){
        HUD.SetActive(true);
        Debug.Log("Room menu hidden after joining/creating a room.");
    }

    
}
