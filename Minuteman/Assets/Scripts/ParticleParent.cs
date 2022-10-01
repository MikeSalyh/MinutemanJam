using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleParent : MonoBehaviour
{
    public static ParticleParent Instance;

    private void Start()
    {
        Instance = this;
    }
}
