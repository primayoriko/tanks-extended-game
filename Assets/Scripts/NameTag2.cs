using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameTag2 : MonoBehaviour
{
    public Text theName;
    // Start is called before the first frame update
    void Start()
    {
        theName.text = StateNameController.nameInput;
    }

}
