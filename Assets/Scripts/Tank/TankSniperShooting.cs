using System;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class TankSniperShooting : TankShooting
{

    protected new void Start()
    {
        m_FireButton = "SniperFire";

        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;

    }

}
