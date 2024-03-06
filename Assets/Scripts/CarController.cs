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
    private float isHandbreaking;
    private bool isFlicking;
    private Rigidbody rb;
    private float timeInAir = 0f;
    private float flickDelay = 2f;
    private bool canFlick = true;
    private float rollLeft = 0f;
    private float rollRight = 0f;
    private float roll = 0f;

    public float boostAmount = 100f;
    private float leftRecuperationDelay = 0f;
    private float recuperationDelay = 2;

    private List<InputAction> pressedKeys;

    [SerializeField] private GameObject ball;

    public Collider CarCollider;
    public GameObject BoostGameObject;
    public WheelCollider frontLeftWheelCollider;
    public WheelCollider frontRightWheelCollider;
    public WheelCollider rearLeftWheelCollider;
    public WheelCollider rearRightWheelCollider;
    public Transform frontLeftWheelTransform;
    public Transform frontRightWheelTransform;
    public Transform rearLeftWheelTransform;
    public Transform rearRightWheelTransform;

    private Vector3 fLWOffset;
    private Vector3 fRWOffset;
    private Vector3 rLWOffset;
    private Vector3 rRWOffset;
    private Quaternion fLWRotation;
    private Quaternion fRWRotation;
    private Quaternion rLWRotation;
    private Quaternion rRWRotation;

    private Vector3 dirr = Vector3.zero;
    private Vector3 dirr2 = Vector3.zero;

    [SerializeField] private float maxSteeringAngle = 30f;
    [SerializeField] private float motorForce = 50f;
    [SerializeField] private float brakeForce = 0f;
    [SerializeField] private float jumpForce = 100f;
    [SerializeField] private float flickForce = 10;
    [SerializeField] private float boostForce = 30f;
    [SerializeField] private float yawpitchRotationSpeed = 2f;
    [SerializeField] private float rollRotationSpeed = 2f;
    [SerializeField, Range(0, 1)] private float flickDeadzone = 0.2f;
    [SerializeField, Range(0, 1)] private float aerialDeadzone = 0.2f;


    [SerializeField] private AudioSource EngineSound;
    [SerializeField] private AudioSource JumpSound;
    [SerializeField] private AudioSource FlickSound;

    private void Awake()
    {
        fLWRotation = frontLeftWheelTransform.rotation;
        fRWRotation = frontRightWheelTransform.rotation;
        rLWRotation = rearLeftWheelTransform.rotation;
        rRWRotation = rearRightWheelTransform.rotation;

        fLWOffset = frontLeftWheelTransform.position;
        fRWOffset = frontRightWheelTransform.position;
        rLWOffset = rearLeftWheelTransform.position;
        rRWOffset = rearRightWheelTransform.position;
    }
    void Start()
    {
        CarCollider.contactOffset = 0.1f;
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass += new Vector3(0, -1f, 0);
        pressedKeys = new List<InputAction>();

        Physics.IgnoreCollision(ball.GetComponent<Collider>(), frontLeftWheelCollider.GetComponent<Collider>());
        Physics.IgnoreCollision(ball.GetComponent<Collider>(), frontRightWheelCollider.GetComponent<Collider>());
        Physics.IgnoreCollision(ball.GetComponent<Collider>(), rearLeftWheelCollider.GetComponent<Collider>());
        Physics.IgnoreCollision(ball.GetComponent<Collider>(), rearRightWheelCollider.GetComponent<Collider>());
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
        Debug.DrawRay(rb.position, dirr * 50, Color.cyan);
        Debug.DrawRay(rb.position, dirr2 * 50, Color.red);

        if (!IsGrounded())
        {
            timeInAir += Time.deltaTime;
            AerialCarControl();
            if (timeInAir > flickDelay)
            {
                canFlick = false;
            }
            rb.AddForce(Vector3.up * Physics.gravity.y * rb.mass);
        } else
        {
            timeInAir = 0;
            canFlick = true;
            //rb.AddForce(transform.up * Physics.gravity.y * rb.mass * 1.2f);
        }

        HandleBoostAmount();
        if (isBoosting > 0)
        {
            if (boostAmount > 0)
            {
                rb.AddForce(-BoostGameObject.transform.right * rb.mass * boostForce * isBoosting);
            } else
            {
                BoostGameObject.GetComponent<ParticleSystem>().Stop();
            }
        }


        UpdateWheels();
        if (IsGrounded())
        {
            rb.AddForce(transform.forward * motorForce * verticalInput);
        }
        EngineSound.pitch = Mathf.Lerp(EngineSound.pitch, 1 + (rb.velocity.magnitude / 75) + ((rb.velocity.magnitude / 25) % 1.4f), 10 * Time.deltaTime);

    }

    public void HandleBoostAmount()
    {
        if (isBoosting > 0)
        {
            boostAmount -= Time.deltaTime * 20;
        }
        else
        {
            if (boostAmount < 100)
            {
                boostAmount += Time.deltaTime * 20;
            }
            else
            {
                boostAmount = 100;
                leftRecuperationDelay = 0f;
            }
        }

        if (boostAmount <= 0f)
        {
            boostAmount = 0f;
            leftRecuperationDelay = recuperationDelay;
        }
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -transform.up, 1.5f);
    }

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

    private IEnumerator Flick(Vector3 flickPosition, Vector3 flickDirection)
    {
        isFlicking = true;
        rb.constraints = RigidbodyConstraints.FreezePositionY;
        rb.angularDrag = 0.05f;

        //flickPosition = new Vector3(flickPosition.x, 0, flickDirection.z);
        //flickDirection = new Vector3(flickDirection.x, 0, flickDirection.z);

        rb.angularVelocity = Vector3.zero;
        rb.AddForce(flickPosition * rb.mass * flickForce * 1.5f, ForceMode.Impulse);
        rb.AddTorque(flickDirection * rb.mass * flickForce, ForceMode.Impulse);

        yield return new WaitForSeconds(0.75f);

        rb.constraints = RigidbodyConstraints.None;
        rb.angularDrag = 5f;
        isFlicking = false;
    }

    private void OldAerialCarControl()
    {

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
                transform.Rotate(Vector3.up, yawpitchRotationSpeed * 3 * horizontalInput);
            }
        }

        if (verticalInput > aerialDeadzone || verticalInput < -aerialDeadzone)
        {
            transform.Rotate(Vector3.right, yawpitchRotationSpeed * 3 * verticalInput);
        }
    }

    private void AerialCarControl()
    {
        if (!isFlicking)
        {
            if (rollLeft > 0)
            {
                rb.AddRelativeTorque(Vector3.back * rollRotationSpeed * rb.mass * rollLeft * 20);
                //transform.Rotate(Vector3.back, rollRotationSpeed * 2 * rollLeft);
            }

            if (rollRight > 0)
            {
                rb.AddRelativeTorque(Vector3.forward * rollRotationSpeed * rb.mass * rollRight * 20);

                //transform.Rotate(Vector3.forward, rollRotationSpeed * 2 * rollRight);
            }

            if (roll > 0)
            {
                rb.AddRelativeTorque(Vector3.back * rollRotationSpeed * rb.mass * horizontalInput * 30);

                //transform.Rotate(Vector3.back, rollRotationSpeed * 4 * horizontalInput);
            }
            else
            {
                if (horizontalInput > aerialDeadzone || horizontalInput < -aerialDeadzone)
                {
                    rb.AddRelativeTorque(Vector3.up * rollRotationSpeed * rb.mass * horizontalInput * 20);

                    //transform.Rotate(Vector3.up, yawpitchRotationSpeed * 2 * horizontalInput);
                }
            }
        }

        if (verticalInput > aerialDeadzone || verticalInput < -aerialDeadzone)
        {
            rb.AddRelativeTorque(Vector3.right * rollRotationSpeed * rb.mass * verticalInput * 25);

            //transform.Rotate(Vector3.right, yawpitchRotationSpeed * 2 * verticalInput);
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
                        Vector3 flickPosition = Vector3.zero;
                        Vector3 flickDirection = Vector3.zero;
                        if (verticalInput > flickDeadzone)
                        {
                            flickPosition += transform.forward * verticalInput;
                            flickDirection += transform.right;
                        }
                        if (verticalInput < -flickDeadzone)
                        {
                            flickPosition += transform.forward * verticalInput;
                            flickDirection -= transform.right;
                        }
                        if (horizontalInput > flickDeadzone)
                        {
                            flickPosition += transform.right * horizontalInput;
                            flickDirection -= transform.forward;
                        }
                        if (horizontalInput < -flickDeadzone)
                        {
                            flickPosition += transform.right * horizontalInput;
                            flickDirection += transform.forward;
                        }

                        StartCoroutine(Flick(flickPosition.normalized, flickDirection.normalized));

                        dirr = flickDirection;
                        dirr2 = flickPosition;

                        FlickSound.Play();
                        //rb.velocity = new Vector3(rb.velocity.x, Physics.gravity.x, rb.velocity.z);
                    }
                    else
                    {
                        rb.AddForce(transform.up * rb.mass * jumpForce);
                        JumpSound.Play();
                    }
                }
            }
            else
            {
                rb.AddForce(transform.up * rb.mass * jumpForce);
                JumpSound.Play();
            }
        }
    }

    public void HandleBoost(InputAction.CallbackContext context)
    {
        isBoosting = context.action.ReadValue<float>();

        if (isBoosting > 0 && boostAmount > 0)
        {
            BoostGameObject.GetComponent<ParticleSystem>().Play();
        }
        else
        {
            BoostGameObject.GetComponent<ParticleSystem>().Stop();
        }
    }

    public void HandleHandbrake(InputAction.CallbackContext context)
    {
        isHandbreaking = context.action.ReadValue<float>();

        float newStiffness = 2f;
        if (isHandbreaking > 0) {
            newStiffness = 0.5f;
        }

        WheelFrictionCurve friction = frontLeftWheelCollider.GetComponent<WheelCollider>().forwardFriction;
        friction.stiffness = newStiffness;
        frontLeftWheelCollider.GetComponent<WheelCollider>().forwardFriction = friction;
        frontRightWheelCollider.GetComponent<WheelCollider>().sidewaysFriction = friction;
        rearLeftWheelCollider.GetComponent<WheelCollider>().sidewaysFriction = friction;
        rearRightWheelCollider.GetComponent<WheelCollider>().sidewaysFriction = friction;
    }

    public void HandleSteering(InputAction.CallbackContext context)
    {
        horizontalInput = context.ReadValue<Vector2>().x;
        steerAngle = maxSteeringAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = steerAngle;
        frontRightWheelCollider.steerAngle = steerAngle;
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
        UpdateWheelPos(frontLeftWheelCollider, frontLeftWheelTransform, fLWRotation, fLWOffset);
        UpdateWheelPos(frontRightWheelCollider, frontRightWheelTransform, fRWRotation, fRWOffset);
        UpdateWheelPos(rearLeftWheelCollider, rearLeftWheelTransform, rLWRotation, rLWOffset);
        UpdateWheelPos(rearRightWheelCollider, rearRightWheelTransform, rRWRotation, rRWOffset);
    }

    private void UpdateWheelPos(WheelCollider wheelCollider, Transform trans, Quaternion rotate, Vector3 offset)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        trans.rotation = rot * rotate;
        trans.position = pos;// + offset;
    }

}