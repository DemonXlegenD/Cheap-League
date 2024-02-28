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

    private Rigidbody rb;
    public float maxSteeringAngle = 30f;
    public float motorForce = 50f;
    public float brakeForce = 0f;
    public float jumpForce = 0f;
    public float yawpitchRotationSpeed = 20f;
    public float rollRotationSpeed = 20f;

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
            AerialCarControl();
        }

        if (isBoosting > 0)
        {
            rb.AddForce(transform.forward * rb.mass * 50 * isBoosting);
        }

        UpdateWheels();
        rb.AddForce(transform.forward * motorForce * verticalInput);
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

    public void HandleAerial(InputAction.CallbackContext context)
    {
        if (!IsGrounded()) {
            horizontalInput = context.action.ReadValue<Vector2>().x;
            verticalInput = context.action.ReadValue<Vector2>().y;
            Debug.Log(context.action.ReadValue<Vector2>());
        }
    }

    private void AerialCarControl()
    {
        if (verticalInput > 0)
        {
            this.transform.Rotate(Vector3.right, yawpitchRotationSpeed * rb.mass * Time.deltaTime);
        }
        else if (verticalInput < 0)
        {
            this.transform.Rotate(Vector3.left, yawpitchRotationSpeed * rb.mass * Time.deltaTime);
        }
        else if (horizontalInput > 0)
        {
            this.transform.Rotate(Vector3.up, yawpitchRotationSpeed * rb.mass * Time.deltaTime);
        }
        else if (horizontalInput < 0)
        {
            this.transform.Rotate(Vector3.down, yawpitchRotationSpeed * rb.mass * Time.deltaTime);
        }
    }

    public void HandleJump(InputAction.CallbackContext context)
    {
        if (GetPressedKey(context.action))
        {
            if (IsGrounded())
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