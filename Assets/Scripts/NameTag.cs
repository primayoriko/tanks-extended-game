using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameTag : MonoBehaviour
{
    public Text nameLabel;
    public static string nameInput;
    // Start is called before the first frame update
  

    // Update is called once per frame
    void Update()
    {
        Vector3 namePos = Camera.main.WorldToScreenPoint(this.transform.position);
        nameLabel.transform.position = namePos;
    }
}
