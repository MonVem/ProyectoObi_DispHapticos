using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PulleySize : MonoBehaviour
{
    public Slider sizeSlider;
    // Invoked when slider is manipulated
    public void ChangePulleySize()
    {
        transform.localScale = new Vector3(sizeSlider.value, 0.15f, sizeSlider.value);
    }
}
