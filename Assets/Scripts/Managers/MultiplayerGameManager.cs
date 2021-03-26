using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mirror;

/*
	Documentation: https://mirror-networking.com/docs/Components/NetworkManager.html
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkManager.html
*/

public enum GameMode { LastStand, CollectMoney }

public class MultiplayerGameManager : NetworkManager
{
    public List<NetworkConnection> connections;
    // Server
    public int m_NumRoundsToWin = 5;
    public float m_StartDelay = 3f;
    public float m_EndDelay = 3f;
    public float m_CrateSpawnDelay = 15f;
    public int m_MinPlayer = 2;
    public int m_MaxPlayer = 2;
    public int m_maxCrateSpawned = 100;
    public GameMode m_GameMode = GameMode.LastStand;
    public int m_PointsToWin = 10050;
    // public GameObject m_CashTextObject;

    // Server              
    public GameObject m_CratePrefab;
    public Transform[] m_CrateSpawnPoints;
    public bool[] m_UsedCrateSpawnPoints;
    [HideInInspector] public int m_NumSpawnedCrates = 0;

    // Server
    private int m_RoundNumber;
    private WaitForSeconds m_StartWait;
    private WaitForSeconds m_EndWait;
    private TankManager m_RoundWinner;
    private TankManager m_GameWinner;
    private List<TankManager> m_Tanks = new List<TankManager>();
    public CameraControl m_CameraControl;
    public Text m_MessageText;
    public List<Transform> m_SpawnPoints;
    private List<bool> m_Player;
    private List<bool> m_PlayerAlive;
    private List<float> m_PlayerHealth;
    private List<int> m_PlayerCash;
    public UIManager m_UIManager;
    public int m_StartingCash;

    private List<Color> m_PlayerColor;
    //public Text m_MessageText;

    [Server]
    public override void OnStartServer(){
        base.OnStartServer();
        Setup();     
    }

    [Server]
    public void Setup(){
        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);
        m_Player = new List<bool>();
        m_PlayerAlive = new List<bool>();
        m_PlayerCash = new List<int>();
        m_PlayerColor = new List<Color>();
        m_PlayerHealth = new List<float>();
        int numOfPlayer = (m_MaxPlayer < m_SpawnPoints.Count) ? m_MaxPlayer : m_SpawnPoints.Count;
        for(int i = 0; i < numOfPlayer; i++){
            m_Player.Add(false);
            m_PlayerAlive.Add(false);
            m_PlayerHealth.Add(0f);
            m_PlayerCash.Add(0);
        }
        m_PlayerColor.Add(Color.red);
        m_PlayerColor.Add(Color.green);
        m_PlayerColor.Add(Color.blue);
    }

    [Server]
    public override void OnServerAddPlayer(NetworkConnection conn) {
        SpawnTank(conn);
        SetCameraTargets();

        // Check if game should start
        bool startGame = true;
        for(int i = 0; i < m_Player.Count; i++){
            if(!m_Player[i]){
                startGame = false;
                break;
            }
        }
        
        if(startGame){
            StartCoroutine(GameLoop());
        }
    }

    [Server]
    private int GetPlayerNumber(){
        int player = -1;
        for(int i = 0; i < m_SpawnPoints.Count; i++){
            if(!m_Player[i]){
                player = i;
                break;
            }
        }
        if(player == -1) Debug.Log("PLAYER -1");
        return player;
    }

    [Server]
    private void SpawnTank(NetworkConnection conn)
    {
        // Init Player Attribute
        int playerNumber = GetPlayerNumber();
        Transform spawnPoint = m_SpawnPoints[playerNumber];
        Color playerColor = m_PlayerColor[playerNumber];

        // Make Player
        TankManager tank = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation).GetComponent<TankManager>();
        
        // Set Player Color
        // MeshRenderer[] renderers = tank.GetComponentsInChildren<MeshRenderer>();
        // for (int i = 0; i < renderers.Length; i++) renderers[i].material.color = playerColor;

        // Add Player
        NetworkServer.AddPlayerForConnection(conn, tank.gameObject);

        // Set Player Attribute
        tank.m_PlayerNumber = playerNumber + 1;
        tank.RpcSetSpawnPoint(spawnPoint.position, spawnPoint.rotation);
        tank.m_UIManager = m_UIManager;
        tank.m_PlayerColor = playerColor;
        tank.RpcSetup();

        // Finalize Player
        m_Tanks.Add(tank);
        m_Player[playerNumber] = true;
        m_PlayerAlive[playerNumber] = true;
        m_PlayerHealth[playerNumber] = 100f;
        m_PlayerCash[playerNumber] = m_StartingCash;
    }

    private void SetCameraTargets()
    {
        List<Transform> targets = new List<Transform>();

        for (int i = 0; i < m_Player.Count; i++)
        {
            if(m_Player[i] && m_PlayerAlive[i]) targets.Add(m_Tanks[i].transform);
        }

        m_CameraControl.RpcSetCameraTargets(targets);
    }

    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(RoundStarting());
        yield return StartCoroutine(RoundPlaying());
        //yield return StartCoroutine(SpawnCrates());
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
        EmptyCrateSpawnPoints();

        m_CameraControl.SetStartPositionAndSize();

        m_RoundNumber++;
        m_UIManager.RpcSetMessage("ROUND " + m_RoundNumber);

        yield return m_StartWait;
    }

    private IEnumerator RoundPlaying()
    {
        EnableTankControl();

        m_UIManager.RpcSetMessage("");

        // DateTime start = DateTime.Now;
        float start = Time.time;

        while(!IsTerminalState())
        {
            float dur = Time.time - start;
            if (dur >= m_CrateSpawnDelay)
            {
                Debug.Log("Time: " + dur);
                SpawnCrates();
                start = Time.time;
            }
            
            //yield return StartCoroutine();
            //yield return new WaitForSecondsRealtime(m_CrateSpawnDelay);

            //SpawnCrates();

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
        m_UIManager.RpcSetMessage(message);

        yield return m_EndWait;
    }

    private bool IsTerminalState()
    {
        if(m_GameMode == GameMode.LastStand)
        {
            return OneTankLeft();
        }
        else
        {
            return ReachMinPoints();
        }
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

     private bool OneTankLeft()
    {
        int numTanksLeft = 0;

        for (int i = 0; i < m_Tanks.Count; i++)
        {
            if (m_Tanks[i].gameObject.activeSelf)
                numTanksLeft++;
        }

        return numTanksLeft == 1;
    }

    private bool ReachMinPoints()
    {
        for (int i = 0; i < m_Tanks.Count; i++)
        {
            if (m_Tanks[i].gameObject.activeSelf && m_Tanks[i].m_Cash >= m_PointsToWin)
                return true;
        }
        return false;
    }

    private TankManager GetRoundWinner()
    {
        if (m_GameMode == GameMode.LastStand)
        {
            for (int i = 0; i < m_Tanks.Count; i++)
            {
                if (m_Tanks[i].gameObject.activeSelf)
                    return m_Tanks[i];
            }
        }
        else
        {
            for (int i = 0; i < m_Tanks.Count; i++)
            {
                if (m_Tanks[i].gameObject.activeSelf && m_Tanks[i].m_Cash >= m_PointsToWin)
                    return m_Tanks[i];
            }
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

    private void ResetAllTanks()
    {
        for (int i = 0; i < m_Tanks.Count; i++)
        {
            m_Tanks[i].RpcReset();
        }
    }

    private void EnableTankControl()
    {
        foreach(TankManager tank in m_Tanks){
            tank.m_ControlEnabled = true;        }
    }

    private void DisableTankControl()
    {
        foreach(TankManager tank in m_Tanks){
            tank.m_ControlEnabled = false;
        }
    }
    
    private void EmptyCrateSpawnPoints()
    {
        m_NumSpawnedCrates = 0;

        Debug.Log("size "+ m_UsedCrateSpawnPoints.Length);

        for (int i = 0; i < m_UsedCrateSpawnPoints.Length; i++)
        {
            m_UsedCrateSpawnPoints[i] = false;

        }
    }

    private void SpawnCrates()
    {
        Debug.Log("Started Coroutine at timestamp : " + Time.time);

        if (m_NumSpawnedCrates == m_maxCrateSpawned || m_RoundWinner != null){
            // yield return null;
            return;
        }

        // yield return new WaitForSecondsRealtime(m_CrateSpawnDelay);

        int idx = 0;

        Transform loc = getUnusedSpawnPoint(ref idx);

        if(loc != null && m_RoundWinner == null)
        {
            GameObject crate = Instantiate(m_CratePrefab, loc.position, loc.rotation);

            NetworkServer.Spawn(crate);

            Debug.Log("crate pwned");
        }

        //After we have waited 5 seconds print the time again.
        Debug.Log("Finished Coroutine at timestamp : " + Time.time);
    }
    
    public Transform getUnusedSpawnPoint(ref int idx)
    {
        if (m_NumSpawnedCrates == m_maxCrateSpawned)
            return null;

        int j;

        j = UnityEngine.Random.Range(0, m_CrateSpawnPoints.Length - 1);

        m_UsedCrateSpawnPoints[j] = true;

        idx = j;

        Debug.Log("selected idx:" + j + " / "+ m_NumSpawnedCrates);


        m_NumSpawnedCrates++;

        return m_CrateSpawnPoints[j];
    }

    #region Unity Callbacks

    public override void OnValidate()
    {
        base.OnValidate();
    }

    /// <summary>
    /// Runs on both Server and Client
    /// Networking is NOT initialized when this fires
    /// </summary>
    public override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// Runs on both Server and Client
    /// Networking is NOT initialized when this fires
    /// </summary>
    public override void Start()
    {
        base.Start();
    }

    /// <summary>
    /// Runs on both Server and Client
    /// </summary>
    public override void LateUpdate()
    {
        base.LateUpdate();
    }

    /// <summary>
    /// Runs on both Server and Client
    /// </summary>
    public override void OnDestroy()
    {
        base.OnDestroy();
    }

    #endregion

    #region Start & Stop

    /// <summary>
    /// Set the frame rate for a headless server.
    /// <para>Override if you wish to disable the behavior or set your own tick rate.</para>
    /// </summary>
    public override void ConfigureServerFrameRate()
    {
        base.ConfigureServerFrameRate();
    }

    /// <summary>
    /// called when quitting the application by closing the window / pressing stop in the editor
    /// </summary>
    public override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
    }

    #endregion

    #region Scene Management

    /// <summary>
    /// This causes the server to switch scenes and sets the networkSceneName.
    /// <para>Clients that connect to this server will automatically switch to this scene. This is called automatically if onlineScene or offlineScene are set, but it can be called from user code to switch scenes again while the game is in progress. This automatically sets clients to be not-ready. The clients must call NetworkClient.Ready() again to participate in the new scene.</para>
    /// </summary>
    /// <param name="newSceneName"></param>
    public override void ServerChangeScene(string newSceneName)
    {
        base.ServerChangeScene(newSceneName);
    }

    /// <summary>
    /// Called from ServerChangeScene immediately before SceneManager.LoadSceneAsync is executed
    /// <para>This allows server to do work / cleanup / prep before the scene changes.</para>
    /// </summary>
    /// <param name="newSceneName">Name of the scene that's about to be loaded</param>
    public override void OnServerChangeScene(string newSceneName) { }

    /// <summary>
    /// Called on the server when a scene is completed loaded, when the scene load was initiated by the server with ServerChangeScene().
    /// </summary>
    /// <param name="sceneName">The name of the new scene.</param>
    public override void OnServerSceneChanged(string sceneName) { }

    /// <summary>
    /// Called from ClientChangeScene immediately before SceneManager.LoadSceneAsync is executed
    /// <para>This allows client to do work / cleanup / prep before the scene changes.</para>
    /// </summary>
    /// <param name="newSceneName">Name of the scene that's about to be loaded</param>
    /// <param name="sceneOperation">Scene operation that's about to happen</param>
    /// <param name="customHandling">true to indicate that scene loading will be handled through overrides</param>
    public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling) { }

    /// <summary>
    /// Called on clients when a scene has completed loaded, when the scene load was initiated by the server.
    /// <para>Scene changes can cause player objects to be destroyed. The default implementation of OnClientSceneChanged in the NetworkManager is to add a player object for the connection if no player object exists.</para>
    /// </summary>
    /// <param name="conn">The network connection that the scene change message arrived on.</param>
    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        base.OnClientSceneChanged(conn);
    }

    #endregion

    #region Server System Callbacks

    /// <summary>
    /// Called on the server when a new client connects.
    /// <para>Unity calls this on the Server when a Client connects to the Server. Use an override to tell the NetworkManager what to do when a client connects to the server.</para>
    /// </summary>
    /// <param name="conn">Connection from client.</param>
    public override void OnServerConnect(NetworkConnection conn) {
        base.OnServerConnect(conn);
        // connections.Add(conn);
    }

    /// <summary>
    /// Called on the server when a client is ready.
    /// <para>The default implementation of this function calls NetworkServer.SetClientReady() to continue the network setup process.</para>
    /// </summary>
    /// <param name="conn">Connection from client.</param>
    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);
    }

    /// <summary>
    /// Called on the server when a client adds a new player with ClientScene.AddPlayer.
    /// <para>The default implementation for this function creates a new player object from the playerPrefab.</para>
    /// </summary>
    /// <param name="conn">Connection from client.</param>
    // public override void OnServerAddPlayer(NetworkConnection conn)
    // {
    //     base.OnServerAddPlayer(conn);
    // }

    /// <summary>
    /// Called on the server when a client disconnects.
    /// <para>This is called on the Server when a Client disconnects from the Server. Use an override to decide what should happen when a disconnection is detected.</para>
    /// </summary>
    /// <param name="conn">Connection from client.</param>
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
    }

    #endregion

    #region Client System Callbacks

    /// <summary>
    /// Called on the client when connected to a server.
    /// <para>The default implementation of this function sets the client as ready and adds a player. Override the function to dictate what happens when the client connects.</para>
    /// </summary>
    /// <param name="conn">Connection to the server.</param>
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
    }

    /// <summary>
    /// Called on clients when disconnected from a server.
    /// <para>This is called on the client when it disconnects from the server. Override this function to decide what happens when the client disconnects.</para>
    /// </summary>
    /// <param name="conn">Connection to the server.</param>
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
    }

    /// <summary>
    /// Called on clients when a servers tells the client it is no longer ready.
    /// <para>This is commonly used when switching scenes.</para>
    /// </summary>
    /// <param name="conn">Connection to the server.</param>
    public override void OnClientNotReady(NetworkConnection conn) { }

    #endregion

    #region Start & Stop Callbacks

    // Since there are multiple versions of StartServer, StartClient and StartHost, to reliably customize
    // their functionality, users would need override all the versions. Instead these callbacks are invoked
    // from all versions, so users only need to implement this one case.

    /// <summary>
    /// This is invoked when a host is started.
    /// <para>StartHost has multiple signatures, but they all cause this hook to be called.</para>
    /// </summary>
    public override void OnStartHost() { }

    /// <summary>
    /// This is invoked when a server is started - including when a host is started.
    /// <para>StartServer has multiple signatures, but they all cause this hook to be called.</para>
    /// </summary>
    public override void OnStartClient() { }

    /// <summary>
    /// This is called when a host is stopped.
    /// </summary>
    public override void OnStopHost() { }

    /// <summary>
    /// This is called when a server is stopped - including when a host is stopped.
    /// </summary>
    public override void OnStopServer() { }

    /// <summary>
    /// This is called when a client is stopped.
    /// </summary>
    public override void OnStopClient() { }

    #endregion
}
