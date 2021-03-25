using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CrateManager : NetworkBehaviour
{

    public int m_NumOfCrate = 3;            // The number of rounds a single player has to win to win the game.
    // public float m_IntervalTime = 3000;             // The delay between the start of RoundStarting and RoundPlaying phases.
    public GameObject m_CratePrefab;             // Reference to the prefab the players will control.
    // public TankManager[] m_Tanks;               // A collection of managers for enabling and disabling different aspects of the tanks.

    public Transform[] m_SpawnPoints;
    public int m_CashCrateNumber;
    // public GameObject[] m_CashCrates;
    [HideInInspector] public bool[] m_UsedSpawnPoints;

    private int m_RoundNumber;                  // Which round the game is currently on.
    private WaitForSeconds m_StartWait;         // Used to have a delay whilst the round starts.
    private WaitForSeconds m_EndWait;           // Used to have a delay whilst the round or game ends.


    private void Start()
    {
        // for(int i = 0; i < m_CashCrates.Length; i++){
        //     m_CashCrates[i] =  gameObject.AddComponent<CashCrate>(); 
        //     // m_CashCrates[i] =  new CashCrate(); 
        //     m_CashCrates[i].index = i; 
        //     m_CashCrates[i].manager = this; 
        // }

        m_UsedSpawnPoints = new bool[m_SpawnPoints.Length];

        for (int i = 0; i < m_SpawnPoints.Length; i++)
        {
            m_UsedSpawnPoints[i] = false;
        }
        // Create the delays so they only have to be made once.
        // m_StartWait = new WaitForSeconds (m_StartDelay);
        // m_EndWait = new WaitForSeconds (m_EndDelay);
        for (int i = 0; i < m_CashCrateNumber; i++)
        {
            int idx = 0;
            Transform newPoints = getUnusedSpawnPoint(ref idx);

            // m_CashCrates[i] = Instantiate(m_CratePrefab, newPoints.position, newPoints.rotation) as GameObject;
            GameObject crate = Instantiate(m_CratePrefab, newPoints.position, newPoints.rotation) as GameObject;

            // CashCrate cashCrate = crate.GetComponentInChildren<CashCrate>();
            Crate cashCrate = crate.GetComponent<Crate>();

            cashCrate.value = Random.Range(30, 70);
            cashCrate.index = idx;
            cashCrate.manager = this;

        }
    }

    public Transform getUnusedSpawnPoint(ref int idx)
    {
        int j;

        do
        {
            j = Random.Range(0, m_SpawnPoints.Length - 1);

        } while (m_UsedSpawnPoints[j]);

        m_UsedSpawnPoints[j] = true;

        idx = j;

        Debug.Log(idx);

        return m_SpawnPoints[j];
    }

    void Update()
    {

    }

}
