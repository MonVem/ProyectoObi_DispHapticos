using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HIP_InclinedPlane : HapticInteractionPoint
{
    private TextMeshProUGUI xPos;
    private TextMeshProUGUI yPos;
    private TextMeshProUGUI zPos;
    private TextMeshProUGUI massValue;
    private Slider massSlider;
    private InputField massTextValue;

    void Start()
    {
        base.Start();
        GetPositionLabels();
        UpdatePositionLabels();

    }
    // Update is called once per frame
    void Update()
    {
        base.Update();
        GetPositionLabels();
        UpdatePositionLabels();

        // update position
        if (isTouching)
        {
            IHIP.transform.position = HIPCollidingPosition;
            transform.position = position;
        }
        else
        {
            IHIP.transform.position = position;
            transform.position = position;
        }
        // update damping factors

        Kv = (Kv > 1.0f * _hapticManager.GetHapticDeviceInfo(hapticDeviceNumber, 6)) ? 1.0f * _hapticManager.GetHapticDeviceInfo(hapticDeviceNumber, 6) : Kv;
        Kvr = (Kvr > 1.0f * _hapticManager.GetHapticDeviceInfo(hapticDeviceNumber, 7)) ? 1.0f * _hapticManager.GetHapticDeviceInfo(hapticDeviceNumber, 7) : Kvr;
        Kvg = (Kvr > 1.0f * _hapticManager.GetHapticDeviceInfo(hapticDeviceNumber, 8)) ? 1.0f * _hapticManager.GetHapticDeviceInfo(hapticDeviceNumber, 8) : Kvg;
    }

    private void GetPositionLabels()
    {
        xPos = GameObject.Find("x_text").GetComponent<TextMeshProUGUI>();
        yPos = GameObject.Find("y_text").GetComponent<TextMeshProUGUI>();
        zPos = GameObject.Find("z_text").GetComponent<TextMeshProUGUI>();
        massValue = GameObject.Find("mass_text").GetComponent<TextMeshProUGUI>();
        massTextValue = GameObject.Find("Input Mass").GetComponent<InputField>();
        massSlider = GameObject.Find("Slider Mass").GetComponent<Slider>();
    }

    private void UpdatePositionLabels()
    {
        float.TryParse(massTextValue.text, out mass);
        mass = (mass > 0.0f) ? mass : 0.0f;
        mass = (mass <= 5.0f) ? mass : 5.0f;
        xPos.text = ": " + position.x.ToString("0.00");
        yPos.text = ": " + position.y.ToString("0.00");
        zPos.text = ": " + position.z.ToString("0.00");
        massValue.text = "Mass: " + mass.ToString("0.00") + " Kg";
    }

    public void UpdateMassText()
    {
        massTextValue.text = massSlider.value.ToString("0.00");
    }
    public void UpdateMassSlider()
    {
        massSlider.value = mass;
        rigidBody.mass = mass;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Crate")
        {
            // HIP is touching an object
            isTouching = true;

            // set colliding object mass
            collision.rigidbody.mass = mass;

            // update IHIP position according to colliding position
            HIPCollidingPosition = collision.contacts[0].point + (radius * collision.contacts[0].normal);

            // update collision point
            objectCollidingPosition = position + (collision.contacts[0].normal * Mathf.Abs(collision.contacts[0].separation));
            objectCollidingNormal = collision.contacts[0].normal;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.name == "Crate")
        {
            // set colliding object mass
            collision.rigidbody.mass = mass;

            // update IHIP position according to colliding position
            HIPCollidingPosition = collision.contacts[0].point + (radius * collision.contacts[0].normal);
            
            // update collision point
            objectCollidingPosition = position + (collision.contacts[0].normal * Mathf.Abs(collision.contacts[0].separation));
            objectCollidingNormal = collision.contacts[0].normal;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        isTouching = false;
    }
}
