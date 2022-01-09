using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneCMCamCtrl : MonoBehaviour
{
    [SerializeField] GameObject cinemachineCamTarget;
    [SerializeField] float topClamp = 70;
    [SerializeField] float bottomClamp = -30;
    public bool lockCamera = false;

    DroneInputCtrl dInput;

    float cameraYaw;
    float cameraPitch;
    const float inputThreshold = 0.01f;
    private void Awake()
    {
        dInput = GetComponent<DroneInputCtrl>();
    }
    void Update()
    {
        RotateCamera();
    }
    void RotateCamera()
    {
        if (dInput.LookDir().sqrMagnitude >= inputThreshold && !lockCamera)
        {
            cameraYaw += dInput.LookDir().x * Time.deltaTime;
            cameraPitch += dInput.LookDir().y * Time.deltaTime;
        }
        cameraYaw = ClampAngle(cameraYaw, float.MinValue, float.MaxValue);
        cameraPitch = ClampAngle(cameraPitch, bottomClamp, topClamp);

        cinemachineCamTarget.transform.rotation = Quaternion.Euler(cameraPitch, cameraYaw, 0);
    }
    static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f) angle += 360f;
        if (angle > 360f) angle -= 360f;
        return Mathf.Clamp(angle, min, max);
    }
}
