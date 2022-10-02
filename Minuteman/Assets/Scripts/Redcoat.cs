using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Redcoat : MonoBehaviour
{
    public TargetingTile currentTile;
    private NavMeshAgent agent;
    public WaitForSeconds postShotDisorientation = new WaitForSeconds(1f);

    private bool initialized = false;
    private bool midFiringAtPlayer = false;

    public float shootCooldownRemaining = 0f;
    private float shootCooldown = 10f;

    public bool IsReadyToShoot => shootCooldownRemaining <= 0f;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        agent = GetComponent<NavMeshAgent>();
        yield return new WaitForSeconds(3f);
        TakeFiringPosition();
        initialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            TakeFiringPosition();
        }

        if (initialized)
        {
            if (agent.remainingDistance < 1f && IsReadyToShoot && !midFiringAtPlayer && currentTile.isTargeted)
            {
                StartCoroutine(FireAtPlayerCoroutine());
            }
        }

        if(shootCooldownRemaining > 0f)
        {
            shootCooldownRemaining -= Time.deltaTime;
            if(shootCooldownRemaining <= 0f)
            {
                //Reloading is complete!
                TakeFiringPosition();
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
        midFiringAtPlayer = false;
        TakeCover();
    }

    private void ShootGun()
    {
        shootCooldownRemaining = shootCooldown;
    }

    public void TakeCover()
    {
        TargetingTile output = TargetingGrid.Instance.FindNearestTile(currentTile, true);
        if (output != null)
            agent.SetDestination(output.transform.position);

        agent.isStopped = false;
    }

    public void TakeFiringPosition()
    {
        TargetingTile output = TargetingGrid.Instance.FindNearestTile(currentTile, false);
        if (output != null)
            agent.SetDestination(output.transform.position);

        agent.isStopped = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Tile")
        {
            currentTile = other.GetComponent<TargetingTile>();
            if (currentTile.isTargeted && IsReadyToShoot) {
                agent.SetDestination(currentTile.transform.position);
                //If the agent gets a shot, it's good!
            }
        }
    }
}
