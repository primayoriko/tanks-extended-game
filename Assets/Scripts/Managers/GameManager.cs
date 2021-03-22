using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;

public class GameManager : NetworkManager
{
    public List<NetworkConnection> connections;
    // Server
    public int m_NumRoundsToWin = 5;        
    public float m_StartDelay = 3f;         
    public float m_EndDelay = 3f;

    // Client           
    public CameraControl m_CameraControl;   
    public Text m_MessageText;

    // Server              
    public GameObject m_TankPrefab;
    public Transform m_SpawnPoint;  
    public List<TankManager> m_Tanks;           


    // Server
    private int m_RoundNumber;              
    private WaitForSeconds m_StartWait;     
    private WaitForSeconds m_EndWait;       
    private TankManager m_RoundWinner;
    private TankManager m_GameWinner;       

    [Server]
    public override void OnServerConnect(NetworkConnection conn) 
    {
        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);

        Debug.Log(NetworkServer.connections.Values);

        // SetCameraTargets();
        if (NetworkServer.connections.Count == 2)
        {
            SpawnAllTanks();
            StartCoroutine(GameLoop());
        }
    }

    // [Server]
    // public override void OnServerAddPlayer(NetworkConnection conn)
    // {
    //     // if (NetworkServer.connections.Count == 2)
    //     // {
    //     //     SpawnAllTanks();
    //     //     StartCoroutine(GameLoop());
    //     // }
    //     // m_StartWait = new WaitForSeconds(m_StartDelay);
    //     // m_EndWait = new WaitForSeconds(m_EndDelay);

    //     // // SetCameraTargets();
    //     // if (numPlayers == 2)
    //     // {
    //     //     StartCoroutine(GameLoop());
    //     // }

    // }

    [Server]
    private void SpawnAllTanks(){
        for (int i = 0; i < NetworkServer.connections.Count; i++)
        {
            GameObject tank = Instantiate(playerPrefab, m_SpawnPoint.position, m_SpawnPoint.rotation);
            NetworkServer.AddPlayerForConnection(NetworkServer.connections[i], tank);
           
        }
    }


    private void SpawnTanks(NetworkConnection conn)
    {
        // TankManager tank = Instantiate(playerPrefab, m_SpawnPoint.position, m_SpawnPoint.rotation).GetComponent<TankManager>();
        // NetworkServer.AddPlayerForConnection(conn, tank.gameObject);

        // tank.m_PlayerColor = Random.ColorHSV(0, 1, 0.9f, 0.9f, 1f, 1f);
        // tank.RpcSetSpawnPoint(m_SpawnPoint.position, m_SpawnPoint.rotation);

        // tank.m_PlayerNumber = numPlayers;

        // m_Tanks.Add(tank);

        // tank.RpcSetCamera();

        // for (int i = 0; i < m_Tanks.Length; i++)
        // {
        //     m_Tanks[i].m_Instance =
        //         Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
        //     m_Tanks[i].m_PlayerNumber = i + 1;
        //     m_Tanks[i].Setup();
        // }
    }


    // private void SetCameraTargets()
    // {
    //     Transform[] targets = new Transform[m_Tanks.Length];

    //     for (int i = 0; i < targets.Length; i++)
    //     {
    //         targets[i] = m_Tanks[i].m_Instance.transform;
    //     }

    //     m_CameraControl.m_Targets = targets;
    // }


    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(RoundStarting());
        yield return StartCoroutine(RoundPlaying());
        yield return StartCoroutine(RoundEnding());

        if (m_GameWinner != null)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            StartCoroutine(GameLoop());
        }
    }


    private IEnumerator RoundStarting()
    {
        ResetAllTanks();
        DisableTankControl();

        m_CameraControl.SetStartPositionAndSize();

        m_RoundNumber++;
        m_MessageText.text = "ROUND " + m_RoundNumber;

        yield return m_StartWait;
    }


    private IEnumerator RoundPlaying()
    {
        EnableTankControl();

        m_MessageText.text = string.Empty;

        while(!OneTankLeft())
        {
            yield return null;
        }
    }


    private IEnumerator RoundEnding()
    {
        DisableTankControl();

        m_RoundWinner = null;

        m_RoundWinner = GetRoundWinner();

        if(m_RoundWinner != null) m_RoundWinner.m_Wins++;

        m_GameWinner = GetGameWinner();

        string message = EndMessage();
        m_MessageText.text = message;

        yield return m_EndWait;
    }


    private bool OneTankLeft()
    {
        int numTanksLeft = 0;

        for (int i = 0; i < m_Tanks.Count; i++)
        {
            if (m_Tanks[i].gameObject.activeSelf)
                numTanksLeft++;
        }

        return numTanksLeft <= 1;
    }


    private TankManager GetRoundWinner()
    {
        for (int i = 0; i < m_Tanks.Count; i++)
        {
            if (m_Tanks[i].gameObject.activeSelf)
                return m_Tanks[i];
        }

        return null;
    }


    private TankManager GetGameWinner()
    {
        for (int i = 0; i < m_Tanks.Count; i++)
        {
            if (m_Tanks[i].m_Wins == m_NumRoundsToWin)
                return m_Tanks[i];
        }

        return null;
    }


    private string EndMessage()
    {
        string message = "DRAW!";

        if (m_RoundWinner != null)
            message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!";

        message += "\n\n\n\n";

        for (int i = 0; i < m_Tanks.Count; i++)
        {
            message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " WINS\n";
        }

        if (m_GameWinner != null)
            message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!";

        return message;
    }


    private void ResetAllTanks()
    {
        for (int i = 0; i < m_Tanks.Count; i++)
        {
            m_Tanks[i].RpcReset();
        }
    }


    private void EnableTankControl()
    {
        for (int i = 0; i < m_Tanks.Count; i++)
        {
            m_Tanks[i].m_ControlEnabled = true;
        }
    }


    private void DisableTankControl()
    {
        for (int i = 0; i < m_Tanks.Count; i++)
        {
            m_Tanks[i].m_ControlEnabled = false;
        }
    }
}