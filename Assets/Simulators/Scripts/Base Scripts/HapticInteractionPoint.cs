using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HapticInteractionPoint : MonoBehaviour
{
    [Header("Haptic Device Linker")]
    // Haptic Manager
    public string hapticDeviceName;
    protected GameObject hapticManagerObject;
    // Haptic device information from the haptic manager
    protected HapticManager _hapticManager;
    // Haptic device parent and IHIP game objects
    protected GameObject parentObject;
    protected GameObject IHIP;
    [Header("Haptic Device Information")]
    // Haptic device number
    public int hapticDeviceNumber;
    // Haptic device visual representation
    [ShowOnly] public float mass;
    protected Vector3 position;
    protected Quaternion orientation;
    protected bool[] button;
    [ShowOnly] public bool statusButton0;
    [ShowOnly] public bool statusButton1;
    [ShowOnly] public bool statusButton2;
    [ShowOnly] public bool statusButton3;
    protected Rigidbody rigidBody;
    protected Material material;
    protected float radius;

    protected bool isTouching = false;

    [Header("Stiffness Fator")]
    // stiffness coefficient
    public float Kp = 10; // [N/m]

    [Header("Damping Factors")]
    // damping term
    public float Kv = 20; // [N/m]
    public float Kvr = 10;
    public double Kvg = 10;

    protected Vector3 objectCollidingPosition;
    protected Vector3 objectCollidingNormal;
    protected Vector3 HIPCollidingPosition;

    protected void Awake()
    {
        position = Vector3.zero;
        orientation = Quaternion.identity;
        button = new bool[4];
        rigidBody = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    protected void Start()
    {
        hapticManagerObject = GameObject.Find("Haptic Manager");
        _hapticManager = hapticManagerObject.gameObject.GetComponent<HapticManager>();
        parentObject = GameObject.Find(hapticDeviceName);
        for (int i = 0; i < parentObject.transform.childCount; i++)
        {
            if (parentObject.transform.GetChild(i).gameObject.name == "IHIP")
            {
                IHIP = parentObject.transform.GetChild(i).gameObject;
                break;
            }
        }
        material = IHIP.GetComponent<Renderer>().material;
        radius = (IHIP.GetComponent<Renderer>().bounds.extents.magnitude) / 2;
    }

    protected void Update()
    {
        radius = (IHIP.GetComponent<Renderer>().bounds.extents.magnitude) / 2;

        hapticDeviceNumber = (hapticDeviceNumber > -1 && hapticDeviceNumber < _hapticManager.GetHapticDevicesFound()) ? hapticDeviceNumber : _hapticManager.GetHapticDevicesFound() - 1;

        // get haptic device variables
        position = _hapticManager.GetPosition(hapticDeviceNumber);
        orientation = _hapticManager.GetOrientation(hapticDeviceNumber);
        for (int i = 0; i < 4; i++)
        {
            button[i] = _hapticManager.GetButtonState(hapticDeviceNumber, i);
        }

        // update haptic device mass
        rigidBody.mass = (rigidBody.mass > 0) ? rigidBody.mass : 0.0f;
        mass = rigidBody.mass;

        // update positions of HIP and IHIP
        IHIP.transform.position = position;
        IHIP.transform.rotation = orientation;
        transform.position = position;
        transform.rotation = orientation;

        UpdateButtonsStatus();
    }
    
    protected void UpdateButtonsStatus()
    {
        statusButton0 = button[0] && !button[1] && !button[2] && !button[3]; // Only button 0 is pressed
        statusButton1 = !button[0] && button[1] && !button[2] && !button[3]; // Only button 1 is pressed
        statusButton2 = !button[0] && !button[1] && button[2] && !button[3]; // Only button 2 is pressed
        statusButton3 = !button[0] && !button[1] && !button[2] && button[3]; // Only button 3 is pressed
    }

    public void SetMass(float m)
    {
        rigidBody.mass = (m > 0) ? m : 0.0f;
    }

    public bool HipIsColliding()
    {
        return isTouching;
    }

    public Vector3 CollidingObjectPosition()
    {
        return objectCollidingPosition;
    }

    public Vector3 CollidingObjectNormal()
    {
        return objectCollidingNormal;
    }

    public Vector3 GetPosition()
    {
        return position;
    }
}
