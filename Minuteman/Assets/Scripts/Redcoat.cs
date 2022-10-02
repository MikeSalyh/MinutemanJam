using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Redcoat : MonoBehaviour
{
    public bool onTheAttack = false;
    private bool lookingForFiringPosition = false;

    public TargetingTile currentTile;
    private NavMeshAgent agent;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            CalculateTarget();
        }
    }

    public void CalculateTarget()
    {
        TargetingTile newTargetTile = currentTile;
        Debug.Log("Calculating targeet");
        //Look for the nearest safety!
        if (onTheAttack)
        {
            lookingForFiringPosition = true;
            TargetingTile output = TargetingGrid.Instance.FindNearestTile(currentTile, false);
            if (output != null)
                newTargetTile = output;
        } 
        else
        {
            TargetingTile output = TargetingGrid.Instance.FindNearestTile(currentTile, true);
            if (output != null)
                newTargetTile = output;
        }

        agent.SetDestination(newTargetTile.transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Tile")
        {
            currentTile = other.GetComponent<TargetingTile>();
            if (lookingForFiringPosition && currentTile.isTargeted && onTheAttack) {
                agent.SetDestination(currentTile.transform.position);
                //If the agent gets a shot, it's good!
            }
        }
    }
}
