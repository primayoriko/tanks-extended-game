using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class TankShooting : NetworkBehaviour
{    
    public GameObject m_Shell;            
    public Transform m_FireTransform;    
    public Slider m_AimSlider;           
    public AudioSource m_ShootingAudio;  
    public AudioClip m_ChargingClip;     
    public AudioClip m_FireClip;         
    public float m_MinLaunchForce = 15f; 
    public float m_MaxLaunchForce = 30f; 
    public float m_MaxChargeTime = 0.75f;

    protected string m_FireButton;         
    protected float m_CurrentLaunchForce;  
    protected float m_ChargeSpeed;         
    protected bool m_Fired;                


    protected void OnEnable()
    {
        m_CurrentLaunchForce = m_MinLaunchForce;
        m_AimSlider.value = m_MinLaunchForce;
    }


    protected void Start()
    {
        m_FireButton = "Fire";

        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
    }

    protected void Update()
    {
        if (!hasAuthority) return;
        // Track the current state of the fire button and make decisions based on the current launch force.
        m_AimSlider.value = m_MinLaunchForce;

        if(m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired)
        {
            // at max charge, not yet fired
            m_CurrentLaunchForce = m_MaxLaunchForce;
            CmdFire();
        } else if(Input.GetButtonDown(m_FireButton))
        {
            // have we pressed fire for the first time?
            m_Fired = false;
            m_CurrentLaunchForce = m_MinLaunchForce;
            
            m_ShootingAudio.clip = m_ChargingClip;
            m_ShootingAudio.Play();
        } else if(Input.GetButton(m_FireButton) && !m_Fired)
        {
            // Holding the fire button, not yet fired
            m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;
            m_AimSlider.value = m_CurrentLaunchForce;
        } else if(Input.GetButtonUp(m_FireButton) && !m_Fired)
        {
            // we released the button, havint not fired yet
            CmdFire();
        }
    }

    [Command]
    protected void CmdFire()
    {
        // Instantiate and launch the shell.
        m_Fired = true;
        
        GameObject shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation);

        shellInstance.GetComponent<Rigidbody>().velocity = m_CurrentLaunchForce * m_FireTransform.forward;

        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();

        m_CurrentLaunchForce = m_MinLaunchForce;

        NetworkServer.Spawn(shellInstance);
    }
}