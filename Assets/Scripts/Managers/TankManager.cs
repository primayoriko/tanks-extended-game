using System;
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
    [HideInInspector] [SyncVar(hook = nameof(Setup))] public int m_PlayerNumber;
    
    // Client       
    [HideInInspector] public string m_ColoredPlayerText;
    [HideInInspector] public GameObject m_CashTextObject;
    // [HideInInspector] public GameObject m_Instance;          

    // Server
    [HideInInspector] public int m_Wins;   
    // Both
    [HideInInspector] [SyncVar(hook = nameof(SetControl))]public bool m_ControlEnabled = false;  
    [HideInInspector] [SyncVar(hook = nameof(SetCash))] public int m_Cash = 10000;

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

            Transform[] targets = { transform };
            camera.m_Targets = targets;
        }
    }

    [Client]
    public void Setup(int oldPlayerNumber, int newPlayerNumber)
    {
        m_Movement = GetComponent<TankMovement>();
        m_Shooting = GetComponent<TankShooting>();
        m_SniperShooting = GetComponent<TankSniperShooting>();
        m_CanvasGameObject = GetComponentInChildren<Canvas>().gameObject;

        // m_Movement.m_PlayerNumber = newPlayerNumber;
        // m_Shooting.m_PlayerNumber = newPlayerNumber;

        m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">PLAYER " + newPlayerNumber + "</color>";

        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = m_PlayerColor;
        }

        m_Movement.enabled = false;
        m_Shooting.enabled = false;
        m_SniperShooting.enabled = false;

        m_CanvasGameObject.SetActive(false);
    }

    [Client]
    public void SetControl(bool oldControl, bool newControl){
        m_Movement.enabled = newControl;
        m_Shooting.enabled = newControl;
        m_SniperShooting.enabled = newControl;

        m_CanvasGameObject.SetActive(newControl);
    }

    [Client]
    public void SetCash(int oldCash, int newCash){
        m_Cash = newCash;
    }

    [Client]
    public void Update(){
        m_CashTextObject.GetComponent<Text>().text = String.Format("Cash: {0}$", m_Cash);
        //m_CashText.GetComponent<Text>().text = String.Format("Cash: {0}$", m_Cash);
    }

    // [ClientRpc]
    // public void DisableControl()
    // {
    //     m_Movement.enabled = false;
    //     m_Shooting.enabled = false;

    //     m_CanvasGameObject.SetActive(false);
    // }

    // [ClientRpc]
    // public void EnableControl()
    // {
    //     m_Movement.enabled = true;
    //     m_Shooting.enabled = true;

    //     m_CanvasGameObject.SetActive(true);
    // }

    [ClientRpc]
    public void RpcReset()
    {
        transform.position = m_SpawnPosition;
        transform.rotation = m_SpawnRotation;

        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }
}
