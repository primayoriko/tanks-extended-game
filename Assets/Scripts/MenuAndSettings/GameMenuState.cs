using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenuState: MonoBehaviour
{
    public Text m_MapText;
    
    public Text m_NameText;

    public Text m_ModeText;

    public static string nameInput;

    public static int modeInput;

    // Start is called before the first frame update
    public void btn_change_scene(){
        string Name;
        string Map;
        string Mode;

        Map = m_MapText.text;
        Name = m_NameText.text;
        Mode = m_ModeText.text;
     
        if(string.Equals(Map, "Desert Land")){
            SceneManager.LoadScene("Map1");
        }else if(string.Equals(Map, "Stone Land")){
            SceneManager.LoadScene("Map2");
        }else{
            SceneManager.LoadScene("SelectMap");
        }

        if (string.Equals(Mode, "Collect Money"))
        {
            modeInput = 1;
        }
        else
        {
            modeInput = 0;
        }

        nameInput = Name;
    }
}
