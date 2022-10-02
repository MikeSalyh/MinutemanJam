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
    public GameObject rifleTip;
    private CanvasGroup cg;


    private void Awake()
    {
        cg = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        Instance = this;
        character.OnFire += HandleFire;
        character.OnReloadComplete += HandleReload;
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerCharacter.Instance.alive)
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
            cg.alpha = 1f;
        }
        else
        {
            cg.alpha = 0f;
            Cursor.visible = true;
            lineRenderer.gameObject.SetActive(false);
        }
        
    }

    private void UpdatePosition()
    {
        if (is2D)
        {
        transform.position = Input.mousePosition + cameraTarget.transform.position + center;
        } else
        {
            Vector3 reticlePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Camera.main.gameObject.transform.position.z * Vector3.up);
            transform.position = reticlePos;
        }
    }

    private void DrawLine()
    {
        List<Vector3> pos = new List<Vector3>();
        pos.Add(new Vector3(rifleTip.transform.position.x, 0f, rifleTip.transform.position.z));
        pos.Add(new Vector3(transform.position.x, 0f, transform.position.z));
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