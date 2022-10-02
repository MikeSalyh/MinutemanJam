using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TargetingTile : MonoBehaviour
{
    public int x, y;
    public bool isTargeted = false;
    public Sprite[] possibleSprites;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = possibleSprites[Random.Range(0, possibleSprites.Length)];
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetTargeted(bool value)
    {
        isTargeted = value;
    }
}
