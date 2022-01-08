using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneInputCtrl : MonoBehaviour
{
    DroneActionMaps dActions;

    void Awake()
    {
        dActions = new DroneActionMaps();
    }

    private void OnEnable()
    {
        dActions.Drone.Enable();
    }
    private void OnDisable()
    {
        dActions.Drone.Disable();
    }

    public Vector2 MovementDir()
    {
        return dActions.Drone.Movement.ReadValue<Vector2>();
    }
    public Vector2 LookDir()
    {
        return dActions.Drone.Look.ReadValue<Vector2>();
    }
    public bool Ascend()
    {
        return dActions.Drone.Ascend.ReadValue<float>() > .5f ? true : false;
    }
    public bool Descend()
    {
        return dActions.Drone.Descend.ReadValue<float>() > .5f ? true : false;
    }
}
