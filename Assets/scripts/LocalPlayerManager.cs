using Unity.VisualScripting;
using UnityEngine;
using Alteruna;

public class LocalPlayerManager : MonoBehaviour
{
    public static LocalPlayerManager Instance {get; private set;}

    [Header("local avatar attributes")]
    public PlayerShoot playerShoot;
    public PlayerMovement playerMovement;
    public Alteruna.Avatar localAvatar;

    [Header("Opponent avatar attributes")]

    public PlayerShoot otherPlayerShoot;
    public PlayerMovement otherPlayerMovement;
    public Alteruna.Avatar otherAvatar;

    //search boolean flags

    private bool myplayerFound = false;

    private bool OtherplayerFound = false;


    private void Awake()
    {
        if(Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else{
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InvokeRepeating(nameof(FindLocalPlayer), 0.5f, 0.5f);
        InvokeRepeating(nameof(FindOtherPlayer), 0.5f, 0.5f);
    }


    private void FindLocalPlayer(){

        if(myplayerFound == true){
            return;
        }

        Alteruna.Avatar[] avatars = FindObjectsByType<Alteruna.Avatar>(FindObjectsSortMode.None);

        foreach(var avatar in avatars){
            if(avatar.IsMe){
                localAvatar = avatar;

                Transform playerTransform = avatar.transform.Find("Player");

                if(playerTransform != null){
                    playerMovement = playerTransform.GetComponent<PlayerMovement>();
                    playerShoot = playerTransform.GetComponentInChildren<PlayerShoot>();

                    if(playerMovement != null && playerShoot != null){
                        Debug.Log("✅ Found local player's Avatar, Movement, and Shoot script!");
                        myplayerFound = true;
                        CancelInvoke(nameof(FindLocalPlayer));
                        return;
                    }
                    else{
                        Debug.LogWarning("⚠ PlayerShoot or PlayerMovement not found in local player's Avatar!");
                    }
                }
                else{
                    Debug.LogWarning("⚠ 'Player' object not found inside local Avatar!");
                }
            }
        }

    }

    private void FindOtherPlayer(){
        if(OtherplayerFound == true){
            return;
        }

        Alteruna.Avatar[] avatars = FindObjectsByType<Alteruna.Avatar>(FindObjectsSortMode.None);


        foreach(var avatar in avatars){
            if(!avatar.IsMe){
                otherAvatar = avatar;

                Transform playerTransform = avatar.transform.Find("Player");

                if(playerTransform != null){
                    otherPlayerMovement = playerTransform.GetComponent<PlayerMovement>();
                    otherPlayerShoot = playerTransform.GetComponentInChildren<PlayerShoot>();

                    if(otherPlayerMovement != null && otherPlayerShoot != null){
                        Debug.Log("✅ Found Other player's Avatar, Movement, and Shoot script!");
                        myplayerFound = true;
                        CancelInvoke(nameof(FindLocalPlayer));
                        return;
                    }
                    else{
                        Debug.LogWarning("⚠ PlayerShoot or PlayerMovement not found in Other player's Avatar!");
                    }
                }
                else{
                    Debug.LogWarning("⚠ 'Player' object not found inside local Avatar!");
                }
            }
        }

    }
}


