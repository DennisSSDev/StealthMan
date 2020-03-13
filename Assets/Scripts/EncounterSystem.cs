using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using Worq;

public class EncounterSystem : MonoBehaviour
{
    public AWSPatrol[] aWSPatrols;
    public GameObject player;
    public float viewAngle;
    public float minDistanceToTarget = 10000.0f;
    private NavMeshAgent[] activeAgents;
    private bool[] foundPlayer;
    private StealthSystem stealthSystem;
    // Start is called before the first frame update
    void Start()
    {
        int patrollersSize = aWSPatrols.Length;
        uint indexCount = 0;
        activeAgents = new NavMeshAgent[patrollersSize];
        foundPlayer = new bool[patrollersSize];
        foreach(var patroller in aWSPatrols)
        {
            activeAgents[indexCount] = patroller.GetAgent();
        }
        stealthSystem = player.GetComponentInChildren<StealthSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        uint count = 0;
        // define a public var to determine what is an acceptable range outside their view. If outside + has x distance away + in shadow -> fine
        foreach(var patroller in aWSPatrols)
        {
            if(foundPlayer[count])
            {
                NavMeshAgent agent = patroller.GetAgent();
                agent.SetDestination(player.transform.position);
                // @todo can check if the player is still in view
            }
            else
            {
                float fovView = Vector3.Dot(patroller.transform.forward, (player.transform.position - patroller.transform.position).normalized);
                float angle = Mathf.Acos(fovView);
                float degAngle = Mathf.Rad2Deg * angle;
                if(degAngle < viewAngle && Vector3.SqrMagnitude(player.transform.position - patroller.transform.position) < minDistanceToTarget && !stealthSystem.GetIsInShadow())
                {
                    // the player is in the view of the seeker and is within the minimum distance
                    foundPlayer[count] = true;
                }
            }
            count++;
        }
    }
}
