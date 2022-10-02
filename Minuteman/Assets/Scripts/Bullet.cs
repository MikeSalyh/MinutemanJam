using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float blastVelocity = 25f;
    private Rigidbody2D rb2;
    private Rigidbody rb;
    private bool is2D = false;

    public GameObject smokePrefab, impactPrefab;
    public GameObject trailPrefab;
    private GameObject trailInstantiated;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb2 = GetComponent<Rigidbody2D>();
        is2D = rb2 != null;
    }

    public void Shoot(Vector3 direction)
    {
        trailInstantiated = GameObject.Instantiate(trailPrefab, this.transform);
        GameObject smoke = GameObject.Instantiate(smokePrefab, GameObject.FindGameObjectWithTag("ParticleParent").transform);
        smoke.transform.position = this.transform.position;
        smoke.transform.LookAt(transform.position + (Vector3)direction, Vector3.forward);
        Destroy(smoke, 1f);

        if (is2D)
        {
            rb2.AddForce(direction * blastVelocity);
        } else
        {
            rb.AddForce(direction * blastVelocity);
        }

        Destroy(this.gameObject, 1f); //after 1 second, the bullet self destructs.
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleImpact();
    }

    private void OnCollisionEnter(Collision collision)
    {
        HandleImpact();
    }

    private void HandleImpact()
    {
        GameObject spark = GameObject.Instantiate(impactPrefab, GameObject.FindGameObjectWithTag("ParticleParent").transform);
        spark.transform.position = this.transform.position;
        Destroy(spark, 1f);

        if (trailInstantiated != null)
        {
            trailInstantiated.transform.parent = this.transform.parent;
            Destroy(trailInstantiated, 1f);
        }
        Destroy(this.gameObject);
    }
}
