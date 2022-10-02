using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TargetingTile : MonoBehaviour
{
    public bool isTargeted = false;
    public TMP_Text debugText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetTargeted(bool value)
    {
        isTargeted = value;
        debugText.text = isTargeted ? "HOT!" : "";
    }
}
