using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform cameraPosition;

    private Alteruna.Avatar _avatar;


    void Start()
    {
        _avatar = GetComponentInParent<Alteruna.Avatar>();
        if(!_avatar.IsMe){
            return;
        }
        transform.position = cameraPosition.position;
    }

    // Update is called once per frame
    private void Update()
    {
        if(!_avatar.IsMe){
            return;
        }
        transform.position = cameraPosition.position;
    }
}
