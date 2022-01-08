# Third Person Drone Controller Tutorial
This tutorial will demonstrate how to develop a Third Person Drone Controller system utilising the new input system in Unity

## 1. Setup your project

Start by creating a new 3D Unity Project

### After creating the project we need to install some required packages
- Go to your `Package Manager` located under the `Window` menu 
- Go to the `Packages:` drop down and select `Unity Registry`
- Then find and install `Cinemachine` and `Input System`

### With the new Input System package installed we will need to change some project settings to be able to use it
- Go to `Edit > Project Settings`
- Find `Player` in the list on left side of the window
- Open the `Other Settings` section
- Scroll down and find `Active Input Handling` (found usually in the middle of the page scroll), set this to `Input System Package (new)` or `Both`
- After changing `Active Input Handling` you will be prompted to restart Unity to apply the new Input System

### We will need to create some assets ready for later
- Create a new Material called `DroneMat`, _we will use this to colour our Drone mesh_
- Create a new script called `ThirdPersonDroneController`, _this will control the Drone's movement and physics behaviour_
- Create a new script called `DroneCMCamCtrl`, _this will control the Drone's third person camera behaviour_
- Create a new script called `DroneInputCtrl`, _this will handle the input used to control the Drone_

### Using the new input system we will need to create an asset to hold our action map with all of the controls
- Create a new `Input Actions` asset in your project files calling this `DroneActionMaps` (this can be found at the bottom of the `Create` menu)
- Open up the asset and create a new Action Map by clicking the `+` button next to `Action Maps`, call this map `Drone`
- With the `Drone` action map open add a new action by clicking the `+` button next to `Actions`, call this action `Movement`, then go to the action's properties and set its `Action Type` to `Value` and `Control Type` to `Vector 2`, add the following bindings by pressing the `+` button next to the action name
  - Add a 2D Vector Composite calling it `Keyboard`, under it's dropdown bind `Up` to `W`, `Down` to `S`, `Left` to `A`, and `Right` to `D`
  - Add a binding setting this to `Left Stick [Gamepad]`
- Create an action called `Look`, in its properties set its `Action Type` to `Value` and `Control Type` to `Vector 2`, add the following bindings
  - Add a binding set to `Delta [Mouse]`, in its properties under Processors add `Invert Vector 2` and unctick `Invert X`, and add a `Scale Vector 2` with **15** set on the X and Y, _this will control our mouse look sensitivity_
  - Add a binding set to `Right Stick [Gamepad]`, in its properties under Processors add `Invert Vector 2` and untick `Invert X`, add `Scale Vector 2` setting the X and Y to **300**, and `Stick Deadzone`
- Create an action called `Ascend`, add the following bindings
  - Add a binding set to `Space [Keyboard]`
  - Add a binding set to `Right Shoulder [Gamepad]`
- Create an action called `Descend`, add the following bindings
  - Add a binding set to `Left Control [Keyboard]`
  - Add a binding set to `Left Shoulder [Gamepad]`
- Save the Input Action asset

![Action map window](https://user-images.githubusercontent.com/79928221/148583441-119d95ed-a245-45cd-8d1f-8f5be60bfd68.png)

### After setting up our input action asset select it in the project files, go to the inspector and tick `Generate C# Class`, this will generate a script with our input actions to use in other scripts, _if you make any changes to the Input Action asset remember to save it to update the script_

## 2. Setup your scene

With all the packages imported create a new scene

In the scene we will need to add some game objects and components for our drone
- Add an empty game object called `Drone`, add a `Box Collider` setting its size to **.75,.1,.75**, add a `Rigidbody` unticking `Use Gravity`, connect our 3 scripts `ThirdPersonDroneController`, `DroneCMCamCtrl` and `DroneInputCtrl`, and create a new tag `Drone` and set it to this object. _This will be the parent/root for our Drone parts._

![Drone Root Inspector](https://user-images.githubusercontent.com/79928221/148587113-64a1df47-4e90-48c7-86d3-40a431015efe.png)
- Add the following as children of the `Drone` object
  - Add a Cube mesh called `GFX`, make sure its positioned at **0,0,0** to keep it centered on the `Drone` object, scale the cube by **.75,.1,.75**, and remove the cube's box collider. _This cube will act as our drone visual_
  - Add an empty game object called `CameraRoot`, _this will act as a target for a Cinemachine virtual camera and will be used to rotate the camera view_

- Add a Cinemachine Virtual Camera found under the `Cinemachine` menu, rename this object to `DroneCMvcam`, then go to it's inspector properties and do the following
  - Set the `Follow` target to the `CameraRoot` object found under the `Drone` root object
  - Go to the `Body` section, open the top dropdown and select 3rd Person Follow
  - Set `Shoulder Offset` to **0,0,0**. _The drone doesn't have shoulders so we dont need a shoulder offset for our camera focus point_
  - Set `Vertical Arm Length` to **.2**, _this will move the camera focus slightly above the drone_
  - Set `Camera Side` to **.5**, _this will center camera view on the Drone horizontally_
  - Set `Camera Distance` to **3**, _as explained by its name this sets the camera distance from our Drone_
  - In the `Ignore Tag` dropdown select `Drone` which we set to our `Drone` root object, _this will ignore collisions with the `Drone` object_
  - Go to the `Aim` section, open the dropdown and select `Do Nothing`

The Cinemachine Inspector properties should look like below\
![Cinemachine Inspector](https://user-images.githubusercontent.com/79928221/148571790-22e9d3a7-d24e-413c-9bce-9920f4e9a00d.png)

## 3. Develop the drone functionality

### Starting off we need to code our input script `DroneInputCtrl`
- First we need to declare a variable to reference our Input Actions script and initiate it in an Awake() method...
```
DroneActionMaps dActions;

void Awake()
  {
    dActions = new DroneActionMaps();
  }
```
- Next using some OnEnable() and OnDisable() methods we need to enable or disable the drone input action map when the script is activated or deactivated...
```
private void OnEnable()
  {
    dActions.Drone.Enable();
  }
private void OnDisable()
  {
    dActions.Drone.Disable();
  }
```
- Now we can create some public methods to be called by our other scripts returning the input values
```
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
```

### Next we can code our Camera control behaviour in the `DroneCMCamCtrl` script

#### To begin with we need to declare some variables and initiate them in an Awake() method...
```
[SerializeField] GameObject cinemachineCamTarget; //Reference for the CameraRoot game object
[SerializeField] float topClamp = 70; //Camera maximum look up angle
[SerializeField] float bottomClamp = -30; //Camera minimum look down angle
public bool lockCamera = false; //Locks camera movement

DroneInputCtrl dInput; //References the DroneInputCtrl behaviour script to get input

float cameraYaw;
float cameraPitch;
const float inputThreshold = 0.01f;
private void Awake()
  {
    dInput = GetComponent<DroneInputCtrl>();
  }
```

#### Next lets create a method to rotate the camera based on the input...
```
void RotateCamera()
    {
        if (dInput.LookDir().sqrMagnitude >= inputThreshold && !lockCamera) //Checks input is above threshold and camera is not locked
        {
            cameraYaw += dInput.LookDir().x * Time.deltaTime;
            cameraPitch += dInput.LookDir().y * Time.deltaTime;
        }
        cameraYaw = ClampAngle(cameraYaw, float.MinValue, float.MaxValue); //Clamps the yaw to max angle values using the ClampAngle method
        cameraPitch = ClampAngle(cameraPitch, bottomClamp, topClamp); //Clamps the Pitch to max angle values using the ClampAngle method

        cinemachineCamTarget.transform.rotation = Quaternion.Euler(cameraPitch, cameraYaw, 0); //Applies the look rotation to the CameraRoot object
    }
    static float ClampAngle(float angle, float min, float max) //Used to clamp values between -360 and 360 for the angles
    {
        if (angle < -360f) angle += 360f;
        if (angle > 360f) angle -= 360f;
        return Mathf.Clamp(angle, min, max);
    }
```
#### Finally add an `Update()` method to execute the `RotateCamera()` method...
```
void Update()
    {
        RotateCamera();
    }
```

### Now lets code the Drone movement in the `ThirdPersonDroneController` script

#### First we need to declare some variables to reference our Drone's components, initiating them in an Awake() method, and properties to control aspects of our Drone's flight movement behaviour...
```
    Rigidbody rb; //Reference for the Drone's rigidbody
    DroneInputCtrl dInput; //Reference for the Drone's input script

    [Header("Flight Properties")]
    [SerializeField] float horizontalSpeed = 5; //Controls the horizontal flight speed of the Drone
    [SerializeField] float verticalSpeed = 5; //Controls the vertical flight speed of the Drone

    [Header("Smoothing Properties")]
    [SerializeField] float turnSmoothTime = 0.3f; //Controls the time it takes to get to a full speed turn and full stop
    [SerializeField] float speedSmoothTime = 0.3f; //Controls the time it takes to reach full speed and full stop

    float targetHoriSpeed; //Holds the desired horizontal speed for our Drone to reach
    float targetVertSpeed; //Holds the desired vertical speed for our Drone to reach
    Transform camT; //Holds the position of the scene camera
    float turnSmoothVelocity; //Reference for the turn speed smoothing velocity
    float speedHoriSmoothVelocity; //Reference for the horizontal speed smoothing velocity
    float speedVertSmoothVelocity; //Reference for the vertical speed smoothing velocity
    void Awake()
    {
        rb = GetComponent<Rigidbody>(); //Finds and initiates the Drone's rigidbody
        dInput = GetComponent<DroneInputCtrl>(); //Finds and initiates the Drone's input script
        camT = Camera.main.transform; //Finds and stores the scenes main camera transform
    }
```

#### Next we need to develop a method to rotate our Drone based on our camera's position...
```
void RotateDrone()
    {
        if (dInput.MovementDir() != Vector2.zero) //Check's if there is player input
        {
            float targetRotation = Mathf.Atan2(dInput.MovementDir().x, dInput.MovementDir().y) * Mathf.Rad2Deg + camT.eulerAngles.y; //Sets a target rotation for our drone depending on the input direction and the camera's position from the Drone
            rb.rotation = Quaternion.Euler(Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime)); //Applies a smoothed rotation to our Drone using the target rotation value
        }
    }
```

#### Now we need to develop a method to control the Drone's movement...
```
void MoveDrone()
    {
        float inputHoriSpeed = horizontalSpeed * dInput.MovementDir().magnitude; //Multiplies the input with the set horizontal speed, no input = 0, input = 1
        targetHoriSpeed = Mathf.SmoothDamp(targetHoriSpeed, inputHoriSpeed, ref speedHoriSmoothVelocity, speedSmoothTime); //Smooths the target speed to prevent instant full speed starts and stops

        float inputVertSpeed; //New variable for holding the vertical input direction multiplied by the set vertical speed
        inputVertSpeed = dInput.Ascend() ? 1 * verticalSpeed : 0; //Checks if the Ascend input is being pressed, if so multiplying the vertical speed by 1 to raise the drone, if false setting to 0 
        inputVertSpeed = dInput.Descend() ? (-1 * verticalSpeed) + inputVertSpeed : 0 + inputVertSpeed; //Checks if the Descend input is being pressed, if so multiplying the vertical speed by -1 to lower the drone, adding the previous value set by the Ascend input, if false setting to 0 again adding the previous value set by the Ascend input
        targetVertSpeed = Mathf.SmoothDamp(targetVertSpeed, inputVertSpeed, ref speedVertSmoothVelocity, speedSmoothTime); //Sets the target vertical speed from the calculated vertical input with smoothing 

        rb.velocity = new Vector3(transform.forward.x * targetHoriSpeed, targetVertSpeed, transform.forward.z * targetHoriSpeed); //Sets the target vertical speed to the Drone's rigidbody velocity
    }
```
#### With the Drone movement methods complete we need to call them in a FixedUpdate() method
```
private void FixedUpdate()
    {
        MoveDrone();
        RotateDrone();
    }
```
## 4. Setup the scripts on the Drone

