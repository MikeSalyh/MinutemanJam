using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Redcoat : MonoBehaviour
{
    public bool onTheAttack = false;

    public TargetingTile currentTile;
    private NavMeshAgent agent;
    public WaitForSeconds postShotDisorientation = new WaitForSeconds(1f);

    private bool initialized = false;
    private bool midFiringAtPlayer = false;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        agent = GetComponent<NavMeshAgent>();
        yield return new WaitForSeconds(1f);
        CalculateTarget();
        initialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            onTheAttack = true;
            CalculateTarget();
        }

        if (initialized)
        {
            if (agent.remainingDistance < 1f && onTheAttack && !midFiringAtPlayer && currentTile.isTargeted)
            {
                StartCoroutine(FireAtPlayerCoroutine());
            }
        }
    }

    private IEnumerator FireAtPlayerCoroutine()
    {
        midFiringAtPlayer = true;
        agent.isStopped = true;
        yield return new WaitForSeconds(UnityEngine.Random.value);
        Debug.Log("Firing!");
        ShootGun();
        yield return postShotDisorientation;
        onTheAttack = false;
        midFiringAtPlayer = false;
        CalculateTarget();
    }

    private void ShootGun()
    {

    }


    public void CalculateTarget()
    {
        TargetingTile newTargetTile = currentTile;
        Debug.Log("Calculating target");
        //Look for the nearest safety!
        if (onTheAttack)
        {
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
        agent.isStopped = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Tile")
        {
            currentTile = other.GetComponent<TargetingTile>();
            if (currentTile.isTargeted && onTheAttack) {
                agent.SetDestination(currentTile.transform.position);
                //If the agent gets a shot, it's good!
            }
        }
    }
}
