using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Collections.Generic;

/*
	Documentation: https://mirror-networking.com/docs/Guides/NetworkBehaviour.html
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

public class SpawnlingShooting : NetworkBehaviour
{

    public GameObject m_Shell;            
    public Transform m_FireTransform;     
    public AudioSource m_ShootingAudio;   
    public AudioClip m_FireClip;         
    public float m_LaunchForce = 15f; 
    private float m_Start;
    public float m_FireDelay;
             
    protected bool m_Fired;                

    protected void OnEnable()
    {
        m_Start = Time.time;
        m_Fired = false;
    }


    protected void Update()
    {
        float dur = Time.time - m_Start;
        if(dur >= m_FireDelay && !m_Fired){
            Fire();
            m_Start = Time.time;
            m_Fired = false;
        }
    }

    protected void Fire()
    {
        // Instantiate and launch the shell.
        m_Fired = true;
        
        GameObject shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation);

        shellInstance.GetComponent<Rigidbody>().velocity = m_LaunchForce * m_FireTransform.forward;

        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();

        NetworkServer.Spawn(shellInstance);
    }
 
}
