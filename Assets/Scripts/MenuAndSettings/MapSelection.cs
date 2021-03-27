using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapSelection : MonoBehaviour
{
    public Text output;
    // Start is called before the first frame update
    public void HandleInputData(int val)
    {
        if (val == 0)
        {
            output.text = "Desert Land";
        }
        if (val == 1)
        {
            output.text = "Stone Land";
        }
    }
}
