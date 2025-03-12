using UnityEngine;
using Alteruna;
using System.Collections;


public class MultiplayerUIManager : MonoBehaviour
{
    public GameObject roomPanelMenu;
    public GameObject HUD;
    public GameObject WaitMenu;

    public Multiplayer multiplayer;


    private PlayerMovement _player;

    void Start()
    {
        TryFindPlayerMove();
    }

    void Update()
    {
        if(_player == null){
            TryFindPlayerMove();
        }
    }

    private void TryFindPlayerMove(){
        _player = LocalPlayerManager.Instance?.playerMovement; //returns null if not found
    }

    public void OnPlayerJoin(Multiplayer multiplayer, Room room, User user){
        UpdateUI(room);
        StartCoroutine(WaitForPlayerThenFreeMovement(room));
    }

    public void OnPlayerLeft(Multiplayer multiplayer, User user){
        UpdateUI(multiplayer.CurrentRoom);
        FreePlayerMovement(multiplayer.CurrentRoom);
    }

    public void OnOtherUserJoined(Multiplayer multiplayer, User user){
        UpdateUI(multiplayer.CurrentRoom);
        FreePlayerMovement(multiplayer.CurrentRoom);
    }

    private void UpdateUI(Room room){
        if(room == null){
            return;
        }

        roomPanelMenu.SetActive(false);

        int playerCount = room.GetUserCount();
        int maxPlayerCount = room.MaxUsers;


        if(playerCount < maxPlayerCount){
            WaitMenu.SetActive(true);
        }
        else{
            WaitMenu.SetActive(false);
            HUD.SetActive(true);
        }
    }

    private IEnumerator WaitForPlayerThenFreeMovement(Room room)
    {
        // Wait until _player is assigned
        while (_player == null)
        {
            yield return null; // Waits for the next frame before checking again
        }

        // Now that _player is assigned, execute FreePlayerMovement
        FreePlayerMovement(room);
    }

    private void FreePlayerMovement(Room room){
        if(room == null){
            return;
        }

        if(_player == null){
            return;
        }

        Rigidbody rb = _player.GetComponent<Rigidbody>();


        int playerCount = room.GetUserCount();
        int maxPlayerCount = room.MaxUsers;


        if(playerCount < maxPlayerCount){
            rb.constraints |= RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
        }
        else{
            rb.constraints &= ~(RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ);
        }

    }

    
}
