using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitButton : MonoBehaviour
{
    // Start is called before the first frame update
    
    public void doExitGame() {
        Application.Quit();
    }
}
