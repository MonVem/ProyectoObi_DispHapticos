using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

public class HM_Sphere : HapticManager
{
    [Header("Specific Simulation Parameters")]
    private InputField mukTextValue;
    private InputField musTextValue;
    private Slider mukSlider;
    private Slider musSlider;
    private InputField gravityTextValue;
    private Slider gravitySlider;

    [ShowOnly] public float mu_s = 0;
    [ShowOnly] public float mu_k = 0;
    [ShowOnly] public float _inclination = 0;
    [ShowOnly] public bool CubeCanMove = false;
    public PhysicMaterial ramp;
    protected Vector3 staticForce;
    protected Vector3 dynamicForce;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();

        // setting the haptic thread
        hapticThreadIsRunning = true;
        hapticThread = new Thread(HapticThread);
        // set priority of haptic thread
        hapticThread.Priority = System.Threading.ThreadPriority.Highest;
        // starting the haptic thread
        hapticThread.Start();
    }

    // Update is called once per frame
    void Update()
    {
        CloseOnEscKeyPress();
        ramp.staticFriction = mu_s;
        ramp.dynamicFriction = mu_k;
    }

    void HapticThread()
    {
        while (hapticThreadIsRunning)
        {
            for (int i = 0; i < hapticDevicesDetected; i++)
            {
                UpdatePositionOrientationButtonStates(i);
                if (myHIP[i].HipIsColliding() && myHIP[i].CollidingObjectNormal().z == 0 && mu_s <= 0.0 && mu_k <= 0)
                {
                    Vector3 appliedForce = myHIP[i].CollidingObjectPosition() - myHIP[i].GetPosition();
                    ApplyDepthForce(i, myHIP[i].CollidingObjectPosition());
                    fa = appliedForce.magnitude;
                    fx = appliedForce.x;
                    fy = appliedForce.y;
                    CubeCanMove = false;
                }
                else if (myHIP[i].HipIsColliding() && myHIP[i].CollidingObjectNormal().z == 0 && mu_s > 0 && mu_k > 0)
                {
                    float stacticForceX = mu_s * myHIP[i].mass * gravity * Mathf.Sin(_inclination * Mathf.Deg2Rad);
                    float stacticForceY = myHIP[i].mass * gravity * Mathf.Cos(_inclination * Mathf.Deg2Rad);
                    staticForce = new Vector3(stacticForceX, stacticForceY, 0.0f);

                    float dynamicForceX = mu_k * myHIP[i].mass * gravity * Mathf.Sin(_inclination * Mathf.Deg2Rad);
                    float dynamicForceY = myHIP[i].mass * gravity * Mathf.Cos(_inclination * Mathf.Deg2Rad);
                    dynamicForce = new Vector3(dynamicForceX, dynamicForceY, 0.0f);

                    Vector3 appliedForce = myHIP[i].CollidingObjectPosition() - myHIP[i].GetPosition();
                    fa = appliedForce.magnitude;
                    fx = appliedForce.x;
                    fy = appliedForce.y;

                    if (appliedForce.magnitude <= staticForce.magnitude)
                    {
                        CubeCanMove = false;
                        ApplyFrictionForceStatic(i, myHIP[i].CollidingObjectPosition());
                    }
                    else
                    {
                        CubeCanMove = false;
                        fa = dynamicForce.magnitude;
                        fx = dynamicForce.x;
                        fy = dynamicForce.y;
                        ApplyFrictionForceDynamic(i, myHIP[i].CollidingObjectPosition());
                    }
                }
                else if (myHIP[i].HipIsColliding() && myHIP[i].CollidingObjectNormal().z != 0)
                {
                    Vector3 appliedForce = myHIP[i].CollidingObjectPosition() - myHIP[i].GetPosition();
                    ApplyDepthForce(i, myHIP[i].CollidingObjectPosition());
                    fa = appliedForce.magnitude;
                    fx = appliedForce.x;
                    fy = appliedForce.y;
                }
                else
                {
                    fa = 0.0f;
                    fx = 0.0f;
                    fy = 0.0f;
                }

                //if (button0[i])
                //{
                //    Vector3 g = new Vector3(0, gravity, 0);
                //    Vector3 force = myHIP[i].mass * g;
                //    fa = Mathf.Sqrt(Mathf.Pow(force.x,2) + Mathf.Pow(force.y, 2) + Mathf.Pow(force.z, 2));
                //    fx = force.x;
                //    fy = force.y;
                //    HapticPluginImport.SetHapticsForce(hapticPlugin, i, force);
                //}


                HapticPluginImport.UpdateHapticDevices(hapticPlugin, i);
            }
        }
    }

    public void UpdateMuKText()
    {
        mukTextValue.text = mukSlider.value.ToString("0.00");
    }

    public void UpdateMuKSlider()
    {
        mukSlider.value = mu_k;
    }

    public void UpdateMuSText()
    {
        musTextValue.text = musSlider.value.ToString("0.00");
    }
    public void UpdateMuSSlider()
    {
        musSlider.value = mu_s;
    }
    public void UpdateGravityText()
    {
        gravityTextValue.text = gravitySlider.value.ToString("0.00");
    }
    public void UpdateGravitySlider()
    {
        gravitySlider.value = gravity;
    }

    protected void ApplyFrictionForceStatic(int hapDevNum, Vector3 desiredPosition)
    {
        // Force calculated according to the contact point minus the current position

        // compute linear force    
        Vector3 direction = desiredPosition - position[hapDevNum];
        Vector3 forceField = myHIP[hapDevNum].Kp * direction * (1 - mu_s);
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

    protected void ApplyFrictionForceDynamic(int hapDevNum, Vector3 desiredPosition)
    {
        // Force calculated according to the contact point minus the current position

        // compute linear force    
        Vector3 direction = (desiredPosition - position[hapDevNum]);
        Vector3 forceField = myHIP[hapDevNum].Kp * direction * (1 - (mu_k / mu_s));
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
}
