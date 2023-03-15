using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HIP_Sphere : HapticInteractionPoint
{
    private TextMeshProUGUI coords;

void Start(){
    base.Start();
    coords = GameObject.Find("Text Coord").GetComponent<TextMeshProUGUI>();
    coords.text = "Prueba";

    }
// Update is called once per frame
void Update()
{
    base.Update();

    coords.text = position.ToString();
    // update position
    if (isTouching) {
        IHIP.transform.position = HIPCollidingPosition;
        transform.position = position;
    } else {
        IHIP.transform.position = position;
        transform.position = position;
    }
    // update damping factors

    Kv = (Kv > 1.0f * _hapticManager.GetHapticDeviceInfo(hapticDeviceNumber, 6)) ? 1.0f * _hapticManager.GetHapticDeviceInfo(hapticDeviceNumber, 6) : Kv;
    Kvr = (Kvr > 1.0f * _hapticManager.GetHapticDeviceInfo(hapticDeviceNumber, 7)) ? 1.0f * _hapticManager.GetHapticDeviceInfo(hapticDeviceNumber, 7) : Kvr;
    Kvg = (Kvr > 1.0f * _hapticManager.GetHapticDeviceInfo(hapticDeviceNumber, 8)) ? 1.0f * _hapticManager.GetHapticDeviceInfo(hapticDeviceNumber, 8) : Kvg;
}


void OnCollisionEnter(Collision collision)
{
        Debug.Log("Enter");
    if (collision.gameObject.name == "WoodFloor")
    {
        // HIP is touching an object
        isTouching = true;

        // set colliding object mass
        //collision.rigidbody.mass = 1;

        // update IHIP position according to colliding position
        HIPCollidingPosition = collision.contacts[0].point + (radius * collision.contacts[0].normal);

        // update collision point
        objectCollidingPosition = position + (collision.contacts[0].normal * Mathf.Abs(collision.contacts[0].separation));
        objectCollidingNormal = collision.contacts[0].normal;
        
    }
}

void OnCollisionStay(Collision collision){
}

void OnCollisionExit(Collision collision)
    {
        Debug.Log("Exit");
        isTouching = false;
}
}
