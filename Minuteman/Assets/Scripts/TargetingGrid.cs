using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingGrid : MonoBehaviour
{
    public int gridSize = 20;
    public TargetingTile[,] sightGrid;
    public GameObject gridPrefab;
    public static TargetingGrid Instance;
    public List<TargetingTile> hotTiles = new List<TargetingTile>();
    public int tileSize = 5;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        sightGrid = new TargetingTile[gridSize, gridSize];
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                TargetingTile t = GameObject.Instantiate(gridPrefab, this.transform).GetComponent<TargetingTile>();
                sightGrid[x, y] = t;
                t.transform.localPosition = new Vector3(x * tileSize, y * tileSize);
            }
        }
    }

    public void CalculateGrid(Collider player)
    {
        hotTiles = new List<TargetingTile>();

        RaycastHit h;
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                //Debug.DrawRay(sightGrid[x, y].transform.position, (player.gameObject.transform.position - sightGrid[x, y].transform.position).normalized, Color.blue, 5f);
                if (Physics.Raycast(sightGrid[x,y].transform.position, (player.gameObject.transform.position - sightGrid[x, y].transform.position).normalized, out h, tileSize* tileSize))
                {
                    if(h.collider == player)
                    {
                        //This tile is a valid spot!
                        hotTiles.Add(sightGrid[x, y]);
                        sightGrid[x, y].debugText.text = "!";
                    }
                    else
                    {
                        sightGrid[x, y].debugText.text = "";
                    }
                }
                else
                {
                    sightGrid[x, y].debugText.text = "";
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
