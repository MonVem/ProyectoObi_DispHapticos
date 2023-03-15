using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class HapticManager : MonoBehaviour
{
    // Plugin import
    protected IntPtr hapticPlugin;
    // Haptic thread
    protected Thread hapticThread;
    // A flag to indicate if the haptic simulation currently running
    protected bool hapticThreadIsRunning;

    [Header("Number of Haptic Devices")]
    [ShowOnly] public int permitedHapticsDevices;
    // Haptic devices objects in the scene
    public GameObject[] hapticCursors;
    // Maximum haptic devices in the scene
    protected const int maxNumberOfHaptics = 16;
    protected HapticInteractionPoint[] myHIP = new HapticInteractionPoint[maxNumberOfHaptics];   

    [Header("Haptic Devices Parameters")]
    // Number of haptic devices detected
    [ShowOnly] public int hapticDevicesDetected;
    public float workspace = 100.0f;
    // Position [m] of each haptic device
    protected Vector3[] position = new Vector3[16];
    protected Quaternion[] orientation = new Quaternion[16];
    // State of haptic device buttons
    protected bool[] button0 = new bool[16];
    protected bool[] button1 = new bool[16];
    protected bool[] button2 = new bool[16];
    protected bool[] button3 = new bool[16];

    [Header("Simulation Parameters")]
    public float gravity = -9.8f;
    [ShowOnly] public float fa;
    [ShowOnly] public float fx;
    [ShowOnly] public float fy;

    // Start is called before the first frame update
    protected void Start()
    {
        // inizialization of Haptic Plugin
        Debug.Log("Starting Haptic Devices");
        // check if haptic devices libraries were loaded
        hapticPlugin = HapticPluginImport.CreateHapticDevices();
        hapticDevicesDetected = HapticPluginImport.GetHapticsDetected(hapticPlugin);
        permitedHapticsDevices = maxNumberOfHaptics;
        if (hapticDevicesDetected > 0 && hapticDevicesDetected <= maxNumberOfHaptics)
        {
            Debug.Log("Haptic Devices Found: " + HapticPluginImport.GetHapticsDetected(hapticPlugin).ToString());
            for (int i = 0; i < hapticDevicesDetected; i++)
            {
                myHIP[i] = hapticCursors[i].gameObject.GetComponent<HapticInteractionPoint>();
            }
        }
        else if (hapticDevicesDetected > maxNumberOfHaptics)
        {
            Debug.Log($"Haptic Devices exceed the maximum value of {maxNumberOfHaptics} devices permited in the scene");
            Application.Quit();
        }
        else
        {
            Debug.Log("Haptic Devices cannot be found");
            Application.Quit();
        }
    }

    protected void CloseOnEscKeyPress()
    {
        // Exit application
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    // OnDestroy is called when closing application
    protected void OnDestroy()
    {
        // close haptic thread
        EndHapticThread();
        // delete haptic plugin
        HapticPluginImport.DeleteHapticDevices(hapticPlugin);
        Debug.Log("Application ended correctly");
    }

    public int GetHapticDevicesFound()
    {
        return hapticDevicesDetected;
    }

    public Vector3 GetPosition(int numHapDev)
    {
        return position[numHapDev];
    }

    public Quaternion GetOrientation(int numHapDev)
    {
        return orientation[numHapDev];
    }

    protected void UpdatePositionOrientationButtonStates(int i)
    {
        // get haptic positions and convert them into scene positions
        position[i] = workspace * HapticPluginImport.GetHapticsPositions(hapticPlugin, i);
        orientation[i] = HapticPluginImport.GetHapticsOrientations(hapticPlugin, i);

        // get haptic buttons
        button0[i] = HapticPluginImport.GetHapticsButtons(hapticPlugin, i, 1);
        button1[i] = HapticPluginImport.GetHapticsButtons(hapticPlugin, i, 2);
        button2[i] = HapticPluginImport.GetHapticsButtons(hapticPlugin, i, 3);
        button3[i] = HapticPluginImport.GetHapticsButtons(hapticPlugin, i, 4);
    }

    public bool GetButtonState(int numHapDev, int button)
    {
        bool temp;
        switch (button)
        {
            case 1:
                temp = button1[numHapDev];
                break;
            case 2:
                temp = button2[numHapDev];
                break;
            case 3:
                temp = button3[numHapDev];
                break;
            default:
                temp = button0[numHapDev];
                break;
        }
        return temp;
    }

    public float GetHapticDeviceInfo(int numHapDev, int parameter)
    {
        // Haptic info variables
        // 0 - m_maxLinearForce
        // 1 - m_maxAngularTorque
        // 2 - m_maxGripperForce 
        // 3 - m_maxLinearStiffness
        // 4 - m_maxAngularStiffness
        // 5 - m_maxGripperLinearStiffness;
        // 6 - m_maxLinearDamping
        // 7 - m_maxAngularDamping
        // 8 - m_maxGripperAngularDamping

        float temp;
        switch (parameter)
        {
            case 1:
                temp = (float)HapticPluginImport.GetHapticsDeviceInfo(hapticPlugin, numHapDev, 1);
                break;
            case 2:
                temp = (float)HapticPluginImport.GetHapticsDeviceInfo(hapticPlugin, numHapDev, 2);
                break;
            case 3:
                temp = (float)HapticPluginImport.GetHapticsDeviceInfo(hapticPlugin, numHapDev, 3);
                break;
            case 4:
                temp = (float)HapticPluginImport.GetHapticsDeviceInfo(hapticPlugin, numHapDev, 4);
                break;
            case 5:
                temp = (float)HapticPluginImport.GetHapticsDeviceInfo(hapticPlugin, numHapDev, 5);
                break;
            case 6:
                temp = (float)HapticPluginImport.GetHapticsDeviceInfo(hapticPlugin, numHapDev, 6);
                break;
            case 7:
                temp = (float)HapticPluginImport.GetHapticsDeviceInfo(hapticPlugin, numHapDev, 7);
                break;
            case 8:
                temp = (float)HapticPluginImport.GetHapticsDeviceInfo(hapticPlugin, numHapDev, 8);
                break;
            default:
                temp = (float)HapticPluginImport.GetHapticsDeviceInfo(hapticPlugin, numHapDev, 0);
                break;
        }
        return temp;
    }

    
    protected void ApplyDepthForce(int hapDevNum, Vector3 desiredPosition)
    {
        // Force calculated according to the contact point minus the current position

        // compute linear force    
        Vector3 direction = desiredPosition - position[hapDevNum];
        Vector3 forceField = myHIP[hapDevNum].Kp * direction;
        HapticPluginImport.SetHapticsForce(hapticPlugin, hapDevNum, forceField);

        // compute linear damping force
        Vector3 linearVelocity = HapticPluginImport.GetHapticsLinearVelocity(hapticPlugin, hapDevNum);
        Vector3 forceDamping = -myHIP[hapDevNum].Kv * linearVelocity;
        // sent force to haptic device
        HapticPluginImport.SetHapticsForce(hapticPlugin, hapDevNum, forceDamping);

        // compute angular damping force
        Vector3 angularVelocity = HapticPluginImport.GetHapticsAngularVelocity(hapticPlugin, hapDevNum);
        Vector3 torqueDamping = -myHIP[hapDevNum].Kvr * angularVelocity;
        // sent torque to haptic device
        HapticPluginImport.SetHapticsTorque(hapticPlugin, hapDevNum, torqueDamping);

        // compute gripper angular damping force
        double gripperForce = -myHIP[hapDevNum].Kvg * HapticPluginImport.GetHapticsGripperAngularVelocity(hapticPlugin, hapDevNum);
        // sent gripper force to haptic device
        HapticPluginImport.SetHapticsGripperForce(hapticPlugin, hapDevNum, gripperForce);
    }

    // Closes the thread that was created
    protected void EndHapticThread()
    {
        hapticThreadIsRunning = false;
        Thread.Sleep(100);

        // variables for checking if thread hangs
        bool isHung = false; // could possibely be hung during shutdown
        int timepassed = 0;  // how much time has passed in milliseconds
        int maxwait = 10000; // 10 seconds
        Debug.Log("Shutting down Haptic Thread");
        try
        {
            // loop until haptic thread is finished
            while (hapticThread.IsAlive && timepassed <= maxwait)
            {
                Thread.Sleep(10);
                timepassed += 10;
            }

            if (timepassed >= maxwait)
            {
                isHung = true;
            }
            // Unity tries to end all threads associated or attached
            // to the parent threading model, if this happens, the 
            // created one is already stopped; therefore, if we try to 
            // abort a thread that is stopped, it will throw a mono error.
            if (isHung)
            {
                Debug.Log("Haptic Thread is hung, checking IsLive State");
                if (hapticThread.IsAlive)
                {
                    Debug.Log("Haptic Thread object IsLive, forcing Abort mode");
                    hapticThread.Abort();
                }
            }
            Debug.Log("Shutdown of Haptic Thread completed.");
        }
        catch (Exception e)
        {
            // lets let the user know the error, Unity will end normally
            Debug.Log("ERROR during OnApplicationQuit: " + e.ToString());
        }
    }
}
