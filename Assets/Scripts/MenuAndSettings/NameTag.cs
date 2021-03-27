using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameTag : MonoBehaviour
{
    public Text m_NameText;
    public static string nameInput;
    // Start is called before the first frame update

    // Start is called before the first frame update
    void Start()
    {
        m_NameText.text = GameMenuState.nameInput;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 namePos = Camera.main.WorldToScreenPoint(this.transform.position);
        m_NameText.transform.position = namePos;
    }
}
