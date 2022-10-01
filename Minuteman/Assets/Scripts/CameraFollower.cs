using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    public GameObject focus, reticle;
    //public float ZDepth = 400f;
    public float followTightness = 0.75f;
    public float maxSpeed = 100f;
    private Vector3 velocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 targetPos = Vector3.Lerp(focus.transform.position, reticle.transform.position, 0.1f);
        transform.position = Vector3.SmoothDamp(transform.position, targetPos - Vector3.forward, ref velocity, followTightness, maxSpeed);
    }
}
