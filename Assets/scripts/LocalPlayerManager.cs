using Unity.VisualScripting;
using UnityEngine;
using Alteruna;

public class LocalPlayerManager : MonoBehaviour
{
    public static LocalPlayerManager Instance {get; private set;}

    public PlayerShoot playerShoot;
    public PlayerMovement playerMovement;
    public Alteruna.Avatar localAvatar;

    private bool playerFound = false;


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
    }


    private void FindLocalPlayer(){

        if(playerFound == true){
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
                        playerFound = true;
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
}
