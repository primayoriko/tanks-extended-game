using UnityEngine;
using Mirror;

public class TankMovement : NetworkBehaviour
{
    // public int m_PlayerNumber = 1;         
    public float m_Speed = 12f;            
    public float m_TurnSpeed = 180f;       
    public AudioSource m_MovementAudio;    
    public AudioClip m_EngineIdling;       
    public AudioClip m_EngineDriving;      
    public float m_PitchRange = 0.2f;

    private string m_MovementAxisName;     
    private string m_TurnAxisName;         
    private Rigidbody m_Rigidbody;         
    private float m_MovementInputValue;    
    private float m_TurnInputValue;        
    private float m_OriginalPitch;   

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }


    private void OnEnable ()
    {
        m_Rigidbody.isKinematic = false;
        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;
    }


    private void OnDisable ()
    {
        m_Rigidbody.isKinematic = true;
    }


    private void Start()
    {
        m_MovementAxisName = "Vertical";
        m_TurnAxisName = "Horizontal";

        m_OriginalPitch = m_MovementAudio.pitch;
    }

    [Client]
    private void Update()
    {
        if (!hasAuthority) return;
        // Store the player's input and make sure the audio for the engine is playing.
        m_MovementInputValue = Input.GetAxis(m_MovementAxisName);
        m_TurnInputValue = Input.GetAxis(m_TurnAxisName);

        EngineAudio();
    }

    [Client]
    private void EngineAudio()
    {
        // Play the correct audio clip based on whether or not the tank is moving and what audio is currently playing.
        if(Mathf.Abs(m_MovementInputValue) < 0.1f && Mathf.Abs(m_TurnInputValue) < 0.1f)
        {
            if(m_MovementAudio.clip == m_EngineDriving)
            {
                m_MovementAudio.clip = m_EngineIdling;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        } 
        else 
        {
            if(m_MovementAudio.clip == m_EngineIdling)
            {
                m_MovementAudio.clip = m_EngineDriving;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
    }

    [Client]
    private void FixedUpdate()
    {
        if (!hasAuthority) return;
        Move();
        Turn();
        // Move and turn the tank.
        // CmdMove();
        // CmdTurn();
    }

    private void Move()
    {
        Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
    }

    private void Turn()
    {
        //Adjust the rotation of the tank based on the player's input.
        float turn = m_TurnInputValue * m_TurnSpeed *  Time.deltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, -0f);
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
    }

    // [Command]
    // private void CmdMove()
    // {
    //     // Adjust the position of the tank based on the player's input.
    //     Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;

    //     RpcMove(movement);
    // }

    // [ClientRpc]
    // private void RpcMove(Vector3 movement)
    // {
    //     m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
    // }

    // [Command]
    // private void CmdTurn()
    // {
    //     // Adjust the rotation of the tank based on the player's input.
    //     float turn = m_TurnInputValue * m_TurnSpeed *  Time.deltaTime;
    //     Quaternion turnRotation = Quaternion.Euler(0f, turn, -0f);

    //     RpcTurn(turnRotation);
    // }

    // [ClientRpc]
    // private void RpcTurn(Quaternion turnRotation)
    // {
    //     m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
    // }

}