using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AimWithMouse : MonoBehaviour
{
    public const int SCREEN_WIDTH = 800, SCREEN_HEIGHT = 600;
    public CameraFollower cameraTarget;

    //public LineRenderer lineRenderer;
    public Vector3 center = new Vector3(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2, 0);

    // Update is called once per frame
    void Update()
    {
        UpdatePosition();
        Cursor.visible = false;
    }

    private void UpdatePosition()
    {
        transform.position = Input.mousePosition + cameraTarget.transform.position + center;
    }

    //private void LateUpdate()
    //{
    //    //DrawLine();
    //}

    //void DrawLine()
    //{
    //    List<Vector3> pos = new List<Vector3>();
    //    pos.Add(character.transform.position);
    //    pos.Add(transform.position);
    //    lineRenderer.startWidth = 1f;
    //    lineRenderer.endWidth = 1f;
    //    lineRenderer.SetPositions(pos.ToArray());
    //    lineRenderer.useWorldSpace = true;
    //}
}