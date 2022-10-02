using System;
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

    private static int[] DirectionsX = { -1, 0, 1, 0 };
    private static int[] DirectionsY = { 0, 1, 0, -1 };

    private int walkingLayerMask = 0;

    private void Awake()
    {
        Instance = this;
        walkingLayerMask = 1 << LayerMask.NameToLayer("WalkingArea") | (1 << LayerMask.NameToLayer("EnemyCharacter")); 
        walkingLayerMask = ~walkingLayerMask;
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
                t.x = x;
                t.y = y;
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
                if (Physics.Raycast(sightGrid[x,y].transform.position, (player.gameObject.transform.position - sightGrid[x, y].transform.position).normalized, out h, 50f, walkingLayerMask))
                {
                    if(h.collider == player)
                    {
                        //This tile is a valid spot!
                        hotTiles.Add(sightGrid[x, y]);
                        sightGrid[x, y].SetTargeted(true);
                    }
                    else
                    {
                        sightGrid[x, y].SetTargeted(false);
                    }
                }
                else
                {
                    sightGrid[x, y].SetTargeted(false);
                }
            }
        }
    }

    public TargetingTile FindNearestTile(TargetingTile currentTile, bool lookForSafeTile)
    {
        Queue<TargetingTile> queue = new Queue<TargetingTile>();
        queue.Enqueue(currentTile);
        bool[,] visitedCells = new bool[gridSize, gridSize];
        visitedCells[currentTile.x, currentTile.y] = true;
        startingPosition = currentTile.transform.position;
        while(queue.Count > 0)
        {
            TargetingTile t = queue.Dequeue();
            if (t.isTargeted == lookForSafeTile)
            {
                AddNeighborsToQueue(t.x, t.y, visitedCells, queue, WalkableFilter);
            } else
            {
                return t;
            }
        }
        return null;
    }

    public TargetingTile FindRandomTile(bool lookForSafeTile)
    {
        return null;
    }

    private bool AnyTileFilter(TargetingTile t)
    {
        return true;
    }

    private bool WalkableFilter(TargetingTile t)
    {
        return true;
        //Placeholder for now...

        //RaycastHit h;
        //if (Physics.Raycast(startingPosition, (startingPosition - t.transform.position).normalized, out h, 100f, walkingLayerMask))
        //{
        //    return false;
        //}
        //else
        //{
        //    return true;
        //}
    }

    private Vector3 startingPosition;

    private bool IsCoordinateWithinMap(int x, int y)
    {
        if (x < 0 || y < 0 ||
            x >= gridSize || y >= gridSize)
            return false;

        return true;
    }

    public void AddNeighborsToQueue(int oldX, int oldY, bool[,] visitedCells, Queue<TargetingTile> queue, Func<TargetingTile, bool> checkFunction, Func<TargetingTile, bool> enqueFunction = null)
    {
        for (int i = 0; i < 4; i++)
        {
            int newX = oldX + DirectionsX[i];
            int newY = oldY + DirectionsY[i];
            if (IsCoordinateWithinMap(newX, newY) && !visitedCells[newX, newY] && checkFunction(sightGrid[newX, newY]))
            {
                //Then, check if it's ever been found before, and if it has not, add it to the queue to become generated.
                //The reason for this bool array is to keep the search O(N) instead of O(N^2)
                if (enqueFunction != null)
                    enqueFunction(sightGrid[newX, newY]);

                queue.Enqueue(sightGrid[newX, newY]);
                visitedCells[newX, newY] = true;
            }
        }
    }
}
