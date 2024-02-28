using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float runSpeed = 12f;
    [SerializeField] private float jumpPower = 7f;
    [SerializeField] private float gravity = 10f;
    [SerializeField] private float lookSpeed = 2f;
    [SerializeField] private float lookXLimit = 45f;

    private Rigidbody rb;
    private float moveForce = 1.0f;
    private float vert;


    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;

    private float maxStamina = 7.5f;
    private float stamina = 7.5f;


    private float leftRecuperationDelay = 0f;
    private float recuperationDelay = 2;

    // Getter et Setter pour maxStamina
    public float MaxStamina
    {
        get { return maxStamina; }
        set { maxStamina = value; }
    }

    // Getter et Setter pour stamina
    public float Stamina
    {
        get { return stamina; }
        set { stamina = value; }
    } 

    // Getter et Setter pour leftRecuperationDelay
    public float LeftRecuperationDelay
    {
        get { return leftRecuperationDelay; }
        set { leftRecuperationDelay = value; }
    }

    // Getter et Setter pour recuperationDelay
    public float RecuperationDelay
    {
        get { return recuperationDelay; }
        set { recuperationDelay = value; }
    }

    private bool canRun = true;
    private bool canMove = true;

    [SerializeField, StringSelection("Horizontal 1", "Horizontal 2")] private string inputHorizontalName;
    [SerializeField, StringSelection("Vertical 1", "Vertical 2")] private string inputVerticalName;
    [SerializeField, StringSelection("Jump 1", "Jump 2")] private string inputJumpName;
    [SerializeField, StringSelection("Run 1", "Run 2")] private string inputRunName;

    private CharacterController characterController;
    private
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {

        #region Handles Movment
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        vert = Input.GetAxis("Vertical");

        // Press Left Shift to run
        bool isRunning = Input.GetButton(inputRunName);
       
        float curSpeedX = canMove ? (isRunning && canRun ? runSpeed : walkSpeed) * Input.GetAxis(inputVerticalName) : 0;
        float curSpeedY = canMove ? (isRunning && canRun ? runSpeed : walkSpeed) * Input.GetAxis(inputHorizontalName) : 0;
        bool isMoving = curSpeedX != 0 || curSpeedY != 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        #endregion

        #region Handles Jumping
        if (Input.GetButton(inputJumpName) && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        #endregion

        #region Handles Rotation
        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
        #endregion

        if (canRun && isMoving)
        {
            if (isRunning)
            {
                stamina -= Time.deltaTime;
            }
            else
            {
                if (stamina < maxStamina)
                {
                    stamina += Time.deltaTime * 3;
                }
                else
                {
                    stamina = maxStamina;
                    leftRecuperationDelay = 0f;
                }
            }

            if (stamina <= 0f)
            {
                stamina = 0f;
                canRun = false;
                leftRecuperationDelay = recuperationDelay;
            }
        }
        else
        {
            if (leftRecuperationDelay <= 0f)
            {
                leftRecuperationDelay = 0f;
                if (stamina < maxStamina)
                {
                    stamina += Time.deltaTime * 2;
                }
                else
                {
                    stamina = maxStamina;
                    canRun = true;
                }
            }
            else
            {
                leftRecuperationDelay -= Time.deltaTime;
            }
        }

    }

    private void FixedUpdate()
    {
        rb.AddForce(transform.forward * moveForce * vert);
        Debug.Log(transform.forward * moveForce * vert);
    }
}