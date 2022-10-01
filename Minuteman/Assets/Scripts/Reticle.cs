using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Reticle : MonoBehaviour
{
    public const int SCREEN_WIDTH = 800, SCREEN_HEIGHT = 600;
    public CameraFollower cameraTarget;
    public PlayerCharacter character;
    public Vector3 center = new Vector3(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2, 0);

    public static Reticle Instance;
    public LineRenderer lineRenderer;

    public Image reloadAnimation;
    private bool _isLoaded = true;
    public GameObject loadedGraphics, emptyGraphics;

    public bool is2D = true;

    private void Start()
    {
        Instance = this;
        character.OnFire += HandleFire;
        character.OnReloadComplete += HandleReload;
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePosition();
        Cursor.visible = false;

        if (_isLoaded)
        {
            DrawLine();
        }
        else
        {
            reloadAnimation.fillAmount = 1 - character.ReloadTimeNormalized;
        }
    }

    private void UpdatePosition()
    {
        if (is2D)
        {
        transform.position = Input.mousePosition + cameraTarget.transform.position + center;
        } else
        {
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Camera.main.gameObject.transform.position.z * Vector3.forward);
        }
    }

    private void DrawLine()
    {
        List<Vector3> pos = new List<Vector3>();
        pos.Add(new Vector3(character.transform.position.x, character.transform.position.y, 0));
        pos.Add(new Vector3(transform.position.x, transform.position.y, 0));
        lineRenderer.SetPositions(pos.ToArray());
        lineRenderer.useWorldSpace = true;
    }

    private void HandleFire()
    {
        _isLoaded = false;
        loadedGraphics.SetActive(false);
        emptyGraphics.SetActive(true);
        lineRenderer.gameObject.SetActive(false);

        emptyGraphics.transform.localScale = Vector3.zero;
        emptyGraphics.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack).SetDelay(0.25f);

    }

    private void HandleReload()
    {
        _isLoaded = true;
        loadedGraphics.SetActive(true);
        emptyGraphics.SetActive(false);
        lineRenderer.gameObject.SetActive(true);

        loadedGraphics.transform.localScale = Vector3.zero;
        loadedGraphics.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);

    }
}