using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HIP_Pulley : HapticInteractionPoint
{
    //private TextMeshProUGUI xPos;
    //private TextMeshProUGUI yPos;
    //private TextMeshProUGUI zPos;

    // Update is called once per frame
    void Update()
    {
        base.Update();

        position.x = position.x - 0.1f;
        IHIP.transform.position = position;

        //UpdatePositionLabels();

        // change material color
        if (statusButton0)
        {
            material.color = Color.red;
        }
        else if (statusButton1)
        {
            material.color = Color.blue;
        }
        else if (statusButton2)
        {
            material.color = Color.green;
        }
        else if (statusButton3)
        {
            material.color = Color.yellow;
        }
        else
        {
            material.color = Color.white;
        }

    }
    //private void GetPositionLabels()
    //{
    //    xPos = GameObject.Find("x_text").GetComponent<TextMeshProUGUI>();
    //    yPos = GameObject.Find("y_text").GetComponent<TextMeshProUGUI>();
    //    zPos = GameObject.Find("z_text").GetComponent<TextMeshProUGUI>();
    //}

    //private void UpdatePositionLabels()
    //{
    //    xPos.text = ": " + position.x.ToString("0.00");
    //    yPos.text = ": " + position.y.ToString("0.00");
    //    zPos.text = ": " + position.z.ToString("0.00");
    //}
}
