using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AimWithMouse : MonoBehaviour
{
    public const int SCREEN_WIDTH = 800, SCREEN_HEIGHT = 600;
    public CameraFollower cameraTarget;
    public GameObject character;
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
}