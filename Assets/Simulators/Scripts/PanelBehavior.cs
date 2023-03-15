using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.UI;

public class PanelBehavior : MonoBehaviour
{
    public float openSpeed;
    public float openPosition;
    public float closePosition;
    private bool openedPanel = false;
    public bool showInputs = false;
    private Vector3 position;
    private GameObject content;
    private GameObject labelsSupport;
    private GameObject inputsSupport;
    // Start is called before the first frame update

    void Start()
    {
        content = GameObject.Find("Content");
        labelsSupport = GameObject.Find("Labels Support");
        inputsSupport = GameObject.Find("Inputs Support");
    }

    public void ChangePanelStatus()
    {
        openedPanel = !openedPanel;
        OpenPanel();
    }

    private void OpenPanel()
    {
        position = transform.position;
        
        if (openedPanel)
        {
            content.SetActive(true);
            if (showInputs)
            {
                inputsSupport.transform.position = new Vector3(-471.0f, 51.0f, 0.0f);
            }
            else
            {
                inputsSupport.transform.position = new Vector3(-1471.0f, 51.0f, 0.0f);
            }
            for (float i = 0; i <= Mathf.Abs(closePosition-openPosition)/openSpeed; i++)
            {
                position.x += openSpeed;
                transform.position = position;
            }
        }
        else
        {
            for (float i = 0; i <= Mathf.Abs(closePosition - openPosition) / openSpeed; i++)
            {
                position.x -= openSpeed;
                transform.position = position;
            }
            content.SetActive(false);
            labelsSupport.SetActive(true);
        }
    }

    public void MoveInputs(bool b)
    {
        showInputs = b;

        if (showInputs)
        {
            inputsSupport.transform.position = new Vector3(-71.0f, 51.0f, 0.0f);
        }
        else
        {
            inputsSupport.transform.position = new Vector3(-1471.0f, 51.0f, 0.0f);
        }
    }
    
}
