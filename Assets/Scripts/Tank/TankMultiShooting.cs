using System;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class TankMultiShooting : TankShooting
{
    public Transform m_LeftFireTransform;    
    public Transform m_RightFireTransform;    

    protected new void Start()
    {
        m_FireButton = "MultiFire";

        m_AmmoCost = 20;

        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;

    }

    [Command]
    protected new void CmdFire()
    {
        // Instantiate and launch the shell.
        m_Fired = true;
        
        GameObject shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation);
        GameObject leftShellInstance = Instantiate(m_Shell, m_LeftFireTransform.position, m_LeftFireTransform.rotation);
        GameObject rightShellInstance = Instantiate(m_Shell, m_RightFireTransform.position, m_RightFireTransform.rotation);

        shellInstance.GetComponent<Rigidbody>().velocity = m_CurrentLaunchForce * m_FireTransform.forward;
        leftShellInstance.GetComponent<Rigidbody>().velocity = m_CurrentLaunchForce * m_LeftFireTransform.forward;
        rightShellInstance.GetComponent<Rigidbody>().velocity = m_CurrentLaunchForce * m_RightFireTransform.forward;

        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();

        m_CurrentLaunchForce = m_MinLaunchForce;

        NetworkServer.Spawn(shellInstance);
        NetworkServer.Spawn(rightShellInstance);
        NetworkServer.Spawn(leftShellInstance);
    }

}
