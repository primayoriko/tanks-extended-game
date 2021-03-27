using System;
using UnityEngine;
using Mirror;
using System.Collections.Generic;

/*
	Documentation: https://mirror-networking.com/docs/Guides/NetworkBehaviour.html
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

public class TankSpawn : NetworkBehaviour
{
    public GameObject m_Spawnling;            
    public Transform m_SpawnTransform;
    
    protected int m_SpawnCost;
    protected string m_SpawnButton;         
    private bool m_Spawned;                

    protected void Start()
    {
        m_SpawnButton = "Spawn";
        m_SpawnCost = 40;
    }

    protected void Update()
    {
        TankManager tm = GetComponent<TankManager>();
        if(tm == null)
        {
            Debug.LogError("WTF where is my manager");
        }

        if(Input.GetButtonDown(m_SpawnButton))
        {
            // have we pressed Spawn for the first time?
            m_Spawned = false;
        }
        else if(Input.GetButtonUp(m_SpawnButton) && !m_Spawned && tm.m_Cash >= m_SpawnCost)
        {
            tm.m_Cash -= m_SpawnCost;
            // we released the button, havint not Spawned yet
            CmdSpawn();
        }
    }

    [Command]
    protected void CmdSpawn()
    {
        TankManager tm = GetComponent<TankManager>();
        // Instantiate and launch the shell.
        m_Spawned = true;
        
        GameObject spawnInstance = Instantiate(m_Spawnling, m_SpawnTransform.position, m_SpawnTransform.rotation);
        // spawnInstance.m_Player = tm.m_Player;

        NetworkServer.Spawn(spawnInstance);
    }

}
