using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GoToGame : StateNameController
{
    public Text Map;
    public string Map2;
    public Text Name;
    public string Name2;
    // Start is called before the first frame update
    public void btn_change_scene(){
        Map2 = Map.text;
        Name2 = Name.text;
        if(string.Equals(Map2, "Desert Land")){
            SceneManager.LoadScene("Map1");
        }else if(string.Equals(Map2, "Stone Land")){
            SceneManager.LoadScene("Map2");
        }else{
            SceneManager.LoadScene("SelectMap");
        }
        
        StateNameController.nameInput = Name2;
        
    }
}
