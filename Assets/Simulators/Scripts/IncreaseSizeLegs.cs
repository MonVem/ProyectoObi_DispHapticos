using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IncreaseSizeLegs : MonoBehaviour
{
    public Slider mainSlider;
    public Vector3 updateValue;
    //Invoked when a submit button is clicked.
    public void SubmitSliderSetting()
    {
        Vector3 originalValue = this.gameObject.transform.localScale;
        updateValue = new Vector3(originalValue.x, mainSlider.value/2, originalValue.z);
        this.gameObject.transform.localScale = updateValue;
    }
}
