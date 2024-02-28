using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private float horizontalInput;
    private float verticalInput;
    private float steerAngle;
    private bool isBreaking;
    private float isJumping;
    private float isChangingCamera;

    [SerializeField, StringSelection("Jump 1", "Jump 2")] private string inputJumpName;
    [SerializeField, StringSelection("Camera 1", "Camera 2")] private string inputCameraName;

    private List<string> pressedKeys;

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

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass += new Vector3(0, -1f, 0);
        pressedKeys = new List<string>();
    }

    private bool GetAxisDown(string input)
    {

        if (Input.GetAxis(input) != 0)
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
        GetInput();
        HandleCamera();
        HandleJump();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
        rb.AddForce(transform.forward * motorForce * verticalInput);
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal 2");
        verticalInput = Input.GetAxis("Vertical 2");
        isJumping = Input.GetAxis("Jump 1");
        isChangingCamera = Input.GetAxis("Camera 1");
        isBreaking = Input.GetKey(KeyCode.Space);
    }

    private void HandleCamera()
    {
        if (GetAxisDown(inputCameraName))
        {
            Debug.Log("camera");
        }
    }

    private void HandleJump()
    {
        if (GetAxisDown(inputJumpName))
        {
            Debug.Log("jumping");
        }
    }

    private void HandleSteering()
    {
        steerAngle = maxSteeringAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = steerAngle;
        frontRightWheelCollider.steerAngle = steerAngle;
    }

    private void HandleMotor()
    {
        frontLeftWheelCollider.motorTorque = verticalInput * motorForce * 5;
        frontRightWheelCollider.motorTorque = verticalInput * motorForce * 5;

        brakeForce = isBreaking ? 3000f : 0f;
        frontLeftWheelCollider.brakeTorque = brakeForce;
        frontRightWheelCollider.brakeTorque = brakeForce;
        rearLeftWheelCollider.brakeTorque = brakeForce;
        rearRightWheelCollider.brakeTorque = brakeForce;
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