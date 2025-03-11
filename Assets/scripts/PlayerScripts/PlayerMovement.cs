using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;



    [Header("Ground check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    private Alteruna.Avatar _avatar;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _avatar = GetComponentInParent<Alteruna.Avatar>();

        if(!_avatar.IsMe){
            return;
        }
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!_avatar.IsMe){
            return;
        }

        GroundCheck();
        MyInput();
        SpeedControl();
        DragHandling();
    }


    //ensures physics runs at a fixed frequency rate ensuring reliable movement
    private void FixedUpdate()
    {
        if(!_avatar.IsMe){
            return;
        }
        
        MovePlayer();
    }

    private void MyInput(){
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");


        // jump input handling

        if(Input.GetKey(jumpKey) && readyToJump && grounded){
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }


    private void MovePlayer(){

        //calculates the movement direction

        // moves player in the direction of looking
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if(grounded){
            //applies force in direction of moveDirection
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if(!grounded){
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }

    }

    private void GroundCheck(){
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
    }

    private void DragHandling(){

        //resistance behaves like air resistance
        if(grounded){
            rb.linearDamping = groundDrag;
        }
        else{
            rb.linearDamping = 0;
        }
    }


    private void SpeedControl(){
        Vector3 flatVel = new Vector3(rb.linearVelocity.x,0f,rb.linearVelocity.z);

        //manually limit velocity
        if(flatVel.magnitude > moveSpeed){
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    private void Jump(){
        
        //reset y velocity before jumping
        rb.linearVelocity = new Vector3(rb.linearVelocity.x,0f,rb.linearVelocity.z);

        //generates impulse force upwards
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump(){
        readyToJump = true;
    }
}
