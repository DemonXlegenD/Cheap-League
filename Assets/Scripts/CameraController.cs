using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform ball;
    [SerializeField] private GameObject arrowPointer;

    [SerializeField] private Vector3 offset;
    [SerializeField] private float translateSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField, Range(60, 110)] private int fov;

    private bool cameraLocked = false;
    private List<InputAction> pressedKeys;

    private Controls playerControls;

    private void Awake()
    {
        playerControls = new Controls();
    }

    private void OnEnable()
    {
        playerControls.Gameplay.Camera.Enable();
    }

    private void OnDisable()
    {
        playerControls.Gameplay.Camera.Disable();
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

    private void Start()
    {
        pressedKeys = new List<InputAction>();
        GetComponent<Camera>().fieldOfView = fov;
        if (!ball)
        {
            ball = GameObject.FindGameObjectWithTag("Ball").transform;
        }
    }

    private void FixedUpdate()
    {
        //HandleCamera();
        HandleTranslation();
        HandleRotation();
    }

    private Vector3 GetTargetPos()
    {
        return target.position + target.GetComponent<Rigidbody>().centerOfMass;
    }

    public void HandleCamera(InputAction.CallbackContext context)
    {
        if (GetPressedKey(context.action))
        {
            if (cameraLocked == true)
            {
                cameraLocked = false;
            }
            else
            {
                cameraLocked = true;
            }
        }
    }

    private void TranslateCamera(Vector3 targetPosition)
    {
        RaycastHit hit;
        Vector3 raycastOrigin = targetPosition;
        if (Physics.Raycast(transform.position, targetPosition - transform.position, out hit, Vector3.Distance(transform.position, targetPosition)))
        {
            transform.position = Vector3.Lerp(transform.position, hit.point, translateSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, translateSpeed * Time.deltaTime);
        }
    }

    private void HandleTranslation()
    {
        Vector3 targetPosition;
        Vector3 targetDirection;
        if (cameraLocked)
        {
            arrowPointer.SetActive(false);
            targetDirection = GetTargetPos() - ball.position + new Vector3(0, 5, 0);
            targetPosition = GetTargetPos() + new Vector3(0, offset.y, 0) - (targetDirection.normalized * offset.z);
            //transform.position = Vector3.Lerp(transform.position, targetPosition, translateSpeed * Time.deltaTime);
            //TranslateCamera(targetPosition);
            transform.position = Vector3.Lerp(transform.position, targetPosition, translateSpeed * Time.deltaTime);
            //transform.LookAt(target.position);
        } else
        {
            arrowPointer.SetActive(true);
            targetDirection = ball.position - GetTargetPos();
            arrowPointer.transform.position = GetTargetPos() + (targetDirection.normalized * 2.5f);
            arrowPointer.transform.rotation = Quaternion.LookRotation(targetDirection) * Quaternion.Euler(90, 0, 0);

            //targetPosition = target.TransformPoint(new Vector3(0, 0, offset.z)) + new Vector3(0, offset.y, 0);
            targetPosition = GetTargetPos() + new Vector3(0, offset.y, 0) + (target.forward * offset.z);
            //TranslateCamera(targetPosition);
            transform.position = Vector3.Lerp(transform.position, targetPosition, translateSpeed * Time.deltaTime);
        }

        //var finalPos = Physics.Raycast(transform.position, targetPosition.normalized, targetPosition.magnitude);

    }
    private void HandleRotation()
    {
        var direction = target.forward;
        if (cameraLocked)
        {
            direction = ball.position - transform.position;
        }

        var rotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }
}