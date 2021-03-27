using UnityEngine;
using Mirror;
using System.Collections.Generic;

/*
	Documentation: https://mirror-networking.com/docs/Guides/NetworkBehaviour.html
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

public class SpawnlingMovement : NetworkBehaviour
{        
    public float m_Speed = 12f;            
    public float m_TurnSpeed = 180f;
       
    private Rigidbody m_Rigidbody;         
    // private float m_MovementInputValue;    
    // private float m_TurnInputValue;
    private float m_Start;
    public float m_MoveDelay;
    public float m_MovementDuration;
    private float m_Move;
    private bool m_Movement;
    private float m_MovementStart;
    private float m_Turn;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Start = Time.time;
    }


    private void OnEnable ()
    {
        m_Rigidbody.isKinematic = false;
        m_MovementStart = Time.time;
    }


    private void OnDisable ()
    {
        m_Rigidbody.isKinematic = true;
    }
    
    [Server]
    private void Update(){
        float dur = Time.time - m_Start;
        if(dur >= m_MoveDelay){
            if(!m_Movement){
                m_MovementStart = Time.time;
                m_Move = Random.Range(-1f, 1f);
                m_Turn = Random.Range(-1f, 1f);
                m_Movement = true;
            }
            if(m_Movement && (Time.time - m_MovementStart <= m_MovementDuration)){
                Move();
                Turn();
            } else {
                m_Start = Time.time;
                m_Movement = false;
            }
        }
    }

    // [Server]
    // private void FixedUpdate()
    // {
        
    // }

    private void Move()
    {
        Vector3 movement = transform.forward * m_Move * m_Speed * Time.deltaTime;
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
    }

    private void Turn()
    {
        //Adjust the rotation of the tank based on the player's input.
        float turn = m_Turn * m_TurnSpeed *  Time.deltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, -0f);
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
    }
}
