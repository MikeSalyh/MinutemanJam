using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reticle : MonoBehaviour
{
    public const int SCREEN_WIDTH = 800, SCREEN_HEIGHT = 600;
    public CameraFollower cameraTarget;
    public GameObject character;
    public Vector3 center = new Vector3(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2, 0);

    public static Reticle Instance;
    public LineRenderer lineRenderer;

    private void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePosition();
        Cursor.visible = false;
    }

    private void UpdatePosition()
    {
        transform.position = Input.mousePosition + cameraTarget.transform.position + center;
        DrawLine();
    }

    private void DrawLine()
    {
        List<Vector3> pos = new List<Vector3>();
        pos.Add(new Vector3(character.transform.position.x, character.transform.position.y, 0));
        pos.Add(new Vector3(transform.position.x, transform.position.y, 0));
        lineRenderer.SetPositions(pos.ToArray());
        lineRenderer.useWorldSpace = true;
    }
}