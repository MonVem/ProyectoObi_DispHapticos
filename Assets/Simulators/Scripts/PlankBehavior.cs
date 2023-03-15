using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.UI.Image;

public class PlankBehavior : MonoBehaviour
{
    public Quaternion origin;
    public Vector3 originElevator;
    public HM_InclinedPlane hm;
    public float step;
    public GameObject a;
    public float sizeA;
    // Start is called before the first frame update
    void Start()
    {
        origin = transform.rotation;
        originElevator = a.transform.position;
    }

    public void SetToOrigin()
    {
        transform.rotation = origin;
        a.transform.position = originElevator;
        hm._inclination = 0;
    }

    public void AddInclination()
    {
        hm._inclination += step*100;
        if (hm._inclination <= 25)
        {
            var rot = transform.rotation;
            rot.z += step;
            transform.rotation = rot;
            var pos = a.transform.position;
            pos.y += (float)0.0773333333333333;
            a.transform.position = pos;

        }
    }

    public void ReduceInclination()
    {
        hm._inclination -= step*100;
        if (hm._inclination > 0) {
            var rot = transform.rotation;
            rot.z -= step;
            transform.rotation = rot;
            var pos = a.transform.position;
            pos.y -= (float)0.0773333333333333;
            a.transform.position = pos;
        }
    }
}
