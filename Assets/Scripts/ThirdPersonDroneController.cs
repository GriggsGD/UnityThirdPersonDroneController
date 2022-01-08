using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonDroneController : MonoBehaviour
{
    Rigidbody rb;
    DroneInputCtrl dInput;

    [Header("Flight Properties")]
    [SerializeField] float horizontalSpeed = 5;
    [SerializeField] float verticalSpeed = 5;

    [Header("Smoothing Properties")]
    [SerializeField] float turnSmoothTime = 0.3f;
    [SerializeField] float speedSmoothTime = 0.3f;

    float targetHoriSpeed;
    float targetVertSpeed;
    Transform camT;
    float turnSmoothVelocity;
    float speedHoriSmoothVelocity;
    float speedVertSmoothVelocity;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        dInput = GetComponent<DroneInputCtrl>();
        camT = Camera.main.transform;
    }
    private void FixedUpdate()
    {
        MoveDrone();
        RotateDrone();
    }
    void RotateDrone()
    {
        if (dInput.MovementDir() != Vector2.zero)
        {
            float targetRotation = Mathf.Atan2(dInput.MovementDir().x, dInput.MovementDir().y) * Mathf.Rad2Deg + camT.eulerAngles.y;
            rb.rotation = Quaternion.Euler(Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime));
        }
    }
    void MoveDrone()
    {
        float inputHoriSpeed = horizontalSpeed * dInput.MovementDir().magnitude;
        targetHoriSpeed = Mathf.SmoothDamp(targetHoriSpeed, inputHoriSpeed, ref speedHoriSmoothVelocity, speedSmoothTime);

        float inputVertSpeed;
        if (dInput.Ascend()) inputVertSpeed = 1 * verticalSpeed;
        else if (dInput.Descend()) inputVertSpeed = -1 * verticalSpeed;
        else inputVertSpeed = 0;
        targetVertSpeed = Mathf.SmoothDamp(targetVertSpeed, inputVertSpeed, ref speedVertSmoothVelocity, speedSmoothTime);

        rb.velocity = new Vector3(transform.forward.x * targetHoriSpeed, targetVertSpeed, transform.forward.z * targetHoriSpeed);
    }
}
