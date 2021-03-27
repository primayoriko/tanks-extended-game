using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameTag2 : MonoBehaviour
{
    public Text m_NameText;
    // Start is called before the first frame update
    void Start()
    {
        m_NameText.text = GameMenuState.nameInput;
    }

}
