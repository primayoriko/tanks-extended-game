using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ModeSelection : MonoBehaviour
{
    public Text output;
    // Start is called before the first frame update
    public void HandleInputData(int val)
    {
        if (val == 0)
        {
            output.text = "Last Stand";
        }
        if (val == 1)
        {
            output.text = "Collect Money";
        }
    }
}
