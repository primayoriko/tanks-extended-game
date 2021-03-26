using System;
using System.Collections.Generic; 
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class TankManager : NetworkBehaviour
{
    // Both
    public Color m_PlayerColor;      

    // Client      
    public Vector3 m_SpawnPosition;
    public Quaternion m_SpawnRotation;
    
    // Both
    [HideInInspector] /*[SyncVar(hook = nameof(Setup))]*/ public int m_PlayerNumber;
    
    // Client       
    [HideInInspector] public string m_ColoredPlayerText;
    [HideInInspector] public string m_CashText;
    // [HideInInspector] public GameObject m_CashTextObject;
    public UIManager m_UIManager;
    // [HideInInspector] public GameObject m_Instance;          

    // Server
    public int m_StartingCash = 10000;
    [HideInInspector] public int m_Wins;   
    // Both
    [HideInInspector] [SyncVar(hook = nameof(SetControlHook))] public bool m_ControlEnabled = false;  
    [HideInInspector] [SyncVar(hook = nameof(SetCashHook))] public int m_Cash = 0;

    private TankMovement m_Movement;       
    private TankShooting m_Shooting;
    private TankSniperShooting m_SniperShooting;
    private GameObject m_CanvasGameObject;

    [ClientRpc]
    public void RpcSetSpawnPoint(Vector3 position, Quaternion rotation)
    {
        m_SpawnPosition = position;
        m_SpawnRotation = rotation;

    }
    
    [ClientRpc]
    public void RpcSetCamera()
    {
        if(isLocalPlayer)
        {
            CameraControl camera = ((GameManager)NetworkManager.singleton).m_CameraControl;

            // List<Transform> targets = { transform };
            // camera.m_Targets = targets;
        }
    }

    [ClientRpc]
    public void RpcSetup()
    {
        m_Movement = GetComponent<TankMovement>();
        m_Shooting = GetComponent<TankShooting>();
        m_SniperShooting = GetComponent<TankSniperShooting>();
        m_CanvasGameObject = GetComponentInChildren<Canvas>().gameObject;

        m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">PLAYER " + m_PlayerNumber + "</color>";
        
        SetControl(false);
    }

    public void SetControlHook(bool oldControl, bool newControl){
        SetControl(newControl);
    }

    public void SetControl(bool value){
        m_Movement.enabled = value;
        m_Shooting.enabled = value;
        m_SniperShooting.enabled = value;

        m_CanvasGameObject.SetActive(value);
    }

    public void SetCashHook(int oldCash, int newCash){
        SetCash(newCash);
    }

    public void SetCash(int amount){
        m_CashText = "Cash: " + amount;
        m_UIManager.SetCashText(m_CashText);
    }

    [Client]
    public void Update(){
        // m_CashTextObject.GetComponent<Text>().text = m_CashText;
    //     //m_CashText.GetComponent<Text>().text = String.Format("Cash: {0}$", m_Cash);
    }

    [ClientRpc]
    public void RpcReset()
    {
        transform.position = m_SpawnPosition;
        transform.rotation = m_SpawnRotation;

        // m_Cash = m_StartingCash;

        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }
}
