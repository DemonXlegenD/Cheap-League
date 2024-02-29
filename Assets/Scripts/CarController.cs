using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    private float horizontalInput;
    private float verticalInput;
    private float steerAngle;
    private bool isBreaking;
    private float isBoosting;
    private Rigidbody rb;
    private float timeInAir = 0f;
    private float flickDelay = 2f;
    private bool canFlick = true;
    private float rollLeft = 0f;
    private float rollRight = 0f;
    private float roll = 0f;

    [SerializeField, StringSelection("Jump 1", "Jump 2")] private string inputJumpName;
    [SerializeField, StringSelection("Camera 1", "Camera 2")] private string inputCameraName;

    private List<InputAction> pressedKeys;

    public WheelCollider frontLeftWheelCollider;
    public WheelCollider frontRightWheelCollider;
    public WheelCollider rearLeftWheelCollider;
    public WheelCollider rearRightWheelCollider;
    public Transform frontLeftWheelTransform;
    public Transform frontRightWheelTransform;
    public Transform rearLeftWheelTransform;
    public Transform rearRightWheelTransform;

    [SerializeField] private float maxSteeringAngle = 30f;
    [SerializeField] private float motorForce = 50f;
    [SerializeField] private float brakeForce = 0f;
    [SerializeField] private float jumpForce = 0f;
    [SerializeField] private float yawpitchRotationSpeed = 20f;
    [SerializeField] private float rollRotationSpeed = 20f;
    [SerializeField, Range(0, 1)] private float flickDeadzone = 0.2f;
    [SerializeField, Range(0, 1)] private float aerialDeadzone = 0.2f;


    [SerializeField] private Controls playerControls;

    private void Awake()
    {
        playerControls = new Controls();
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass += new Vector3(0, -1f, 0);
        pressedKeys = new List<InputAction>();
    }

    private void OnEnable()
    {
        playerControls.Gameplay.Move.Enable();
    }

    private void OnDisable()
    {
        playerControls.Gameplay.Move.Disable();
    }

    private bool GetPressedKey(InputAction input)
    {
        if (input.ReadValue<float>() != 0)
        {
            if (pressedKeys.FindIndex(key => key == input) != -1)
                return false;
            pressedKeys.Add(input);
            return true;
        }
        else
            pressedKeys.Remove(input);
        return false;
    }

    private void FixedUpdate()
    {
        //GetInput();
        //HandleJump();
        //HandleMotor();
        //HandleSteering();

        if (!IsGrounded())
        {
            timeInAir += Time.deltaTime;
            AerialCarControl();
            if (timeInAir > flickDelay)
            {
                canFlick = false;
            }
        } else
        {
            timeInAir = 0;
            canFlick = true;
        }

        if (isBoosting > 0)
        {
            rb.AddForce(transform.forward * rb.mass * 50 * isBoosting);
        }

        UpdateWheels();
        if (IsGrounded())
        {
            rb.AddForce(transform.forward * motorForce * verticalInput);
        }
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, 1.5f);
    }

    /*    public void GetInput()
        {
            Debug.Log(playerControls.Gameplay.Move.ReadValue<Vector2>());
            horizontalInput = playerControls.Gameplay.Move.ReadValue<Vector2>().x;
            verticalInput = playerControls.Gameplay.Move.ReadValue<Vector2>().y;
            isJumping = Input.GetAxis("Jump 1");
            isBreaking = Input.GetKey(KeyCode.Space);
        }*/

    public void HandleRollLeft(InputAction.CallbackContext context)
    {
        rollLeft = context.action.ReadValue<float>();
    }

    public void HandleRollRight(InputAction.CallbackContext context)
    {
        rollRight = context.action.ReadValue<float>();
    }

    public void HandleRoll(InputAction.CallbackContext context)
    {
        roll = context.action.ReadValue<float>();
    }


    public void HandleAerial(InputAction.CallbackContext context)
    {
        if (!IsGrounded()) {
            horizontalInput = context.action.ReadValue<Vector2>().x;
            verticalInput = context.action.ReadValue<Vector2>().y;
        }
    }

    private void AerialCarControl()
    {
        Debug.Log(verticalInput);
        Debug.Log(horizontalInput);

        if (rollLeft > 0)
        {
            transform.Rotate(Vector3.back, rollRotationSpeed * 2 * rollLeft);
        }

        if (rollRight > 0)
        {
            transform.Rotate(Vector3.forward, rollRotationSpeed * 2 * rollRight);
        }

        if (roll > 0)
        {
            transform.Rotate(Vector3.back, rollRotationSpeed * 4 * horizontalInput);
        }
        else
        {
            if (horizontalInput > aerialDeadzone || horizontalInput < -aerialDeadzone)
            {
                transform.Rotate(Vector3.up, yawpitchRotationSpeed * 2 * horizontalInput);
            }
        }

        if (verticalInput > aerialDeadzone || verticalInput < -aerialDeadzone)
        {
            transform.Rotate(Vector3.right, yawpitchRotationSpeed * 2 * verticalInput);
        }
    }

    public void HandleJump(InputAction.CallbackContext context)
    {
        if (GetPressedKey(context.action))
        {
            if (!IsGrounded())
            {
                if (canFlick && timeInAir < flickDelay)
                {
                    canFlick = false;
                    if (verticalInput > flickDeadzone || verticalInput < -flickDeadzone || horizontalInput > flickDeadzone || horizontalInput < -flickDeadzone)
                    {
                        if (verticalInput > flickDeadzone)
                        {
                            Debug.Log("front flip");
                            rb.AddForce(Vector3.right * rb.mass);
                        }
                        else if (verticalInput < -flickDeadzone)
                        {
                            rb.AddForce(Vector3.left * rb.mass);
                        }
                        else if (horizontalInput > flickDeadzone)
                        {
                            rb.AddForce(Vector3.up * rb.mass);
                        }
                        else if (horizontalInput < -flickDeadzone)
                        {
                            rb.AddForce(Vector3.down * rb.mass);
                        }
                        rb.velocity = new Vector3(rb.velocity.x, Physics.gravity.x, rb.velocity.z);
                    }
                    else
                    {
                        rb.AddForce(transform.up * rb.mass * jumpForce);
                    }
                }
            }
            else
            {
                rb.AddForce(transform.up * rb.mass * jumpForce);
            }
        }
    }

    public void HandleBoost(InputAction.CallbackContext context)
    {
        isBoosting = context.action.ReadValue<float>();
    }

    public void HandleSteering(InputAction.CallbackContext context)
    {
        if (IsGrounded())
        {
            horizontalInput = context.ReadValue<Vector2>().x;
            steerAngle = maxSteeringAngle * horizontalInput;
            frontLeftWheelCollider.steerAngle = steerAngle;
            frontRightWheelCollider.steerAngle = steerAngle;
        }
    }

    public void HandleMotor(InputAction.CallbackContext context)
    {
        if (IsGrounded())
        {
            verticalInput = context.ReadValue<Vector2>().y;
            frontLeftWheelCollider.motorTorque = verticalInput * motorForce * 5;
            frontRightWheelCollider.motorTorque = verticalInput * motorForce * 5;

            brakeForce = isBreaking ? 3000f : 0f;
            frontLeftWheelCollider.brakeTorque = brakeForce;
            frontRightWheelCollider.brakeTorque = brakeForce;
            rearLeftWheelCollider.brakeTorque = brakeForce;
            rearRightWheelCollider.brakeTorque = brakeForce;
        }
    }

    private void UpdateWheels()
    {
        UpdateWheelPos(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateWheelPos(frontRightWheelCollider, frontRightWheelTransform);
        UpdateWheelPos(rearLeftWheelCollider, rearLeftWheelTransform);
        UpdateWheelPos(rearRightWheelCollider, rearRightWheelTransform);
    }

    private void UpdateWheelPos(WheelCollider wheelCollider, Transform trans)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        trans.rotation = rot;
        trans.position = pos;
    }

}