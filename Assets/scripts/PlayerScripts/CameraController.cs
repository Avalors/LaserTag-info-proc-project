using UnityEngine;

public class CameraController : MonoBehaviour
{

    [Header("Camera sensativity")]
    public float sensX;
    public float sensY;
    public Transform player;
    public Transform orientation;

    private float Last_Update = 0;

    float xRotation;
    float yRotation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private Alteruna.Avatar _avatar;

    private FPGAController _fpga;

    void Start()
    {   
        _fpga = FindAnyObjectByType<FPGAController>();

        if(_fpga != null){
            Debug.Log("FPGA controller script found!!");
        }

        _avatar = GetComponentInParent<Alteruna.Avatar>();

        if(!_avatar.IsMe){
            return;
        }
        //locks cursor to the middle of the screen and makes it invisible
        /*Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;*/
    }

    // Update is called once per frame
    void Update()
    {

        if(!_avatar.IsMe){
            return;
        }

        Last_Update += Time.deltaTime;

        if(Last_Update > 0.1f){
            Last_Update = 0f;
            _fpga.UpdateReadings();

            int ShootingData = _fpga.shooting_data;

            if(ShootingData == 1){
                _fpga.ClearButton();
            }

            int accel_y = _fpga.accelerometer_x;
            int accel_x = _fpga.accelerometer_y;

            if (accel_y < 50 && accel_y > -50)
            {
                accel_y = 0;
            }
            if (accel_x < 50 && accel_x > -50)
            {
                accel_x = 0;
            }
            accel_x = accel_x / 4;

            //gets mouse input
            float mouseX = accel_y * Time.deltaTime; 
            float mouseY = accel_x * Time.deltaTime; 

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
}
