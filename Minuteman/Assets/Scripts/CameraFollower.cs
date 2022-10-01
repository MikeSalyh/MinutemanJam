using UnityEngine;
using DG.Tweening;

public class CameraFollower : MonoBehaviour
{
    public GameObject focus, reticle;
    //public float ZDepth = 400f;
    public float followTightness = 0.75f;
    public float maxSpeed = 100f;
    private Vector3 velocity = Vector3.zero;
    public float cursorEffect = 0.35f;
    public float distance = 1f;

    public Transform shakingTransform;

    public static CameraFollower Instance;

    private void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 targetPos = Vector3.Lerp(focus.transform.position, reticle.transform.position, cursorEffect);
        transform.position = Vector3.SmoothDamp(transform.position, targetPos - Vector3.forward * distance, ref velocity, followTightness, maxSpeed);
    }


    public void DoShake(float duration, float power, int vibrato)
    {
        Debug.Log("Shaking!");
        shakingTransform.transform.DOShakePosition(duration, power, vibrato);
    }
}
