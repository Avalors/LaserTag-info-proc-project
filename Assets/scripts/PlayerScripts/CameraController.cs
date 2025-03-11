using UnityEngine;

public class CameraController : MonoBehaviour
{

    [Header("Camera sensativity")]
    public float sensX;
    public float sensY;
    public Transform player;
    public Transform orientation;

    float xRotation;
    float yRotation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private Alteruna.Avatar _avatar;

    void Start()
    {   
        _avatar = GetComponentInParent<Alteruna.Avatar>();

        if(!_avatar.IsMe){
            return;
        }
        //locks cursor to the middle of the screen and makes it invisible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {

        if(!_avatar.IsMe){
            return;
        }

        //gets mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensX;

        yRotation += mouseX; //vertical camera movement
        xRotation -= mouseY; //horizontal camera movement

        //prevents rotation beyond reasonable limits
        xRotation = Mathf.Clamp(xRotation,-90f,90f); 
        // applying rotation for cam and orientation

        //rotate camera on the x-y axis
        transform.rotation = Quaternion.Euler(xRotation,yRotation,0);

        //rotate your player on the y axis (required due to unlimited horizontal rotation)
        player.rotation = Quaternion.Euler(0,yRotation,0);
        orientation.rotation = Quaternion.Euler(0,yRotation,0);

    }
}
