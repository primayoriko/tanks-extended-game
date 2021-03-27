using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankSpawnSniper : TankSpawn
{
    // Start is called before the first frame update
    protected new void Start()
    {
        m_SpawnButton = "Spawn2";
        m_SpawnCost = 50;

    }
}
