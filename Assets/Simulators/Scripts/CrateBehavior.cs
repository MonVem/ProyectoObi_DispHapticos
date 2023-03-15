using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateBehavior : MonoBehaviour
{
    public Vector3 origin;
    private Rigidbody rigidBody;
    public HM_InclinedPlane hm;
    // Start is called before the first frame update
    void Start()
    {
        origin = transform.position;
        rigidBody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "HIP")
        {
            if (hm.CubeCanMove)
            {
                rigidBody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
            }
            else
            {
                rigidBody.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
            }
        }      
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.name == "HIP")
        {
            if (hm.CubeCanMove)
            {
                rigidBody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
            }
            else
            {
                rigidBody.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.name == "HIP")
        {
            rigidBody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
        }
    }

    public void SetToOrigin()
    {
        transform.position = origin;
    }
}
