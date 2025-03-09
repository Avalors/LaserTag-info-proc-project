using UnityEngine;
using Alteruna;
using NUnit.Framework;
using UnityEngine.InputSystem.LowLevel;

public class PlayerShoot : AttributesSync
{
    [SynchronizableField] public int deaths = 0;
    [SynchronizableField] public int health = 100;
    public int maxHealth = 100;

    [SerializeField] private int damage = 10;

    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private int playerSelfLayer;

    public Alteruna.Avatar avatar;
    public Transform maincamera;

    private Vector3 spawnPoint;


    private void Start()
    {
        if(!avatar.IsMe){
            return;
        }

        avatar.gameObject.layer = playerSelfLayer;
        
        spawnPoint = GetComponentInParent<Rigidbody>().transform.position;
    }

    private void Update()
    {
        if(!avatar.IsMe){
            return;
        }

        if(Input.GetKeyDown(KeyCode.Mouse0)){
            Shoot();
        }
    }


    void Shoot(){
        if(Physics.Raycast(maincamera.position, maincamera.forward, out RaycastHit hit, Mathf.Infinity, playerLayer)){
            PlayerShoot playerShoot = hit.transform.GetComponentInChildren<PlayerShoot>();
            playerShoot.Hit(damage);
        }

        
    }

    public void Hit(int damageTaken){
        health -= damageTaken;
        
        if(health <= 0){
            InvokeRemoteMethod("Die", avatar.Possessor);
        }
    }

    [SynchronizableMethod]
    void Die(){
        deaths++;
        health = 100;

        Transform playerTransform = GetComponentInParent<Rigidbody>().transform;
        playerTransform.position = spawnPoint;

    }
}
