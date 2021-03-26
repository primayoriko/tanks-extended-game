using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [Serializable]
public class Crate: MonoBehaviour
{
    // public Transform m_SpawnPoint;
    // [HideInInspector] public GameObject m_Instance;      
    [HideInInspector] public int index;     
    [HideInInspector] public CrateManager manager;  
    [HideInInspector] public int value = 50;  

    // void OnCollisionEnter(Collision collide){
    //     if(collide.gameObject.name == "Player1"){
    //     } else if(collide.gameObject.name == "Player2"){
    //     }
    // }

    void OnTriggerEnter(Collider other){
        value = UnityEngine.Random.Range(20, 50);

        TankManager tm = other.GetComponent<TankManager>();

        MultiplayerGameManager gm = other.GetComponentInParent<MultiplayerGameManager>();

        if(tm != null)
        {
            tm.m_Cash += value;
            
            if(gm != null)
            {
                gm.m_NumSpawnedCrates--;
            } else
            {
                Debug.Log("GMGMMGMGMGM");
            }

            Destroy(gameObject);
        }
        else
        {
            Debug.Log("TMMTMTMTM");
        }


        //int newIdx = 0;
        //Transform newPoint = manager.getUnusedSpawnPoint(ref newIdx);
        //manager.m_UsedSpawnPoints[index] = false;
        //index = newIdx;

        // if(other.name == "Player1"){
        //     other.GetComponent<TankCash>().m_Cash += value;
        // } else if(other.name == "Player2"){
        //     other.GetComponent<TankCash>().m_Cash += value;
        // } else if(other.tag == "Box"){
        //     Debug.Log("Box");

        // } else {
        //     Debug.Log("UHUY");

        // }

        //transform.position = newPoint.position;
    }
}