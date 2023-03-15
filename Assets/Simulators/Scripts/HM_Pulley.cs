using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class HM_Pulley : HapticManager
{
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
    }

    void HapticThread()
    {
        while (hapticThreadIsRunning)
        {
            for (int i = 0; i < hapticDevicesDetected; i++)
            {
                UpdatePositionOrientationButtonStates(i);

                if (button0[i])
                {
                    Vector3 gravity = new Vector3(0, -9.8f, 0);
                    gravity = myHIP[i].mass * gravity;
                    HapticPluginImport.SetHapticsForce(hapticPlugin, i, gravity);
                }

                HapticPluginImport.UpdateHapticDevices(hapticPlugin, i);
            }
        }
    }
}
