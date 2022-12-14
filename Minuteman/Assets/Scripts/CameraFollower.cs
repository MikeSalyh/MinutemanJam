using UnityEngine;
using DG.Tweening;
using System;

public class CameraFollower : MonoBehaviour
{
    public GameObject focus, reticle;
    //public float ZDepth = 400f;
    public float followTightness = 0.75f;
    public float maxSpeed = 100f;
    private Vector3 velocity = Vector3.zero;
    public float cursorEffect = 0.35f;
    public float distance = 1f;
    private Vector3 recoilOffset;
    public float recoilPower = 100f, recoilDampen = 1.25f;

    public Transform shakingTransform;

    public static CameraFollower Instance;

    private void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 targetPos;
        if (PlayerCharacter.Instance.alive)
        {
            targetPos = Vector3.Lerp(focus.transform.position + recoilOffset, reticle.transform.position, cursorEffect);
        }
        else
        {
            targetPos = focus.transform.position;
        }
        transform.position = Vector3.SmoothDamp(transform.position, targetPos - Vector3.up * distance, ref velocity, followTightness, maxSpeed);

        if(recoilOffset.magnitude > 0.1f)
        {
            recoilOffset /= recoilDampen;
        }
    }


    public void DoShake(float duration, float power, int vibrato)
    {
        shakingTransform.transform.localPosition = Vector3.zero;
        if(PlayerCharacter.Instance.alive)
            shakingTransform.transform.DOShakePosition(duration, power, vibrato);
    }

    internal void HandleRecoil(Vector3 direction)
    {
        recoilOffset = direction * recoilPower;
    }
}
