using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collisionTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Test");
    }

    private void OnCollisionStay(Collision collision)
    {
            
    }

    private void OnCollisionExit(Collision collision)
    {
            
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
