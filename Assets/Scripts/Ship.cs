using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Ship : MonoBehaviour
{
    [HideInInspector]
    public float size;

    [HideInInspector]
    public Orientation currentOrientation;

    [HideInInspector]
    public Shiptype shiptype;

    private bool _isSelected = false;
    private Camera _mainCamera;
    private MeshRenderer _mRenderer;
    private Color _meshColor;
    private Color _highlightedColor = Color.yellow;
    private Color _misplaceColor = Color.red;
    private bool _isPlaced = false;
    private float _boardY = -1.27f;

    void Start()
    {
        _misplaceColor.a = 0.5f;
        _mainCamera = Camera.main;
        _mRenderer = gameObject.GetComponent<MeshRenderer>();
        _meshColor = _mRenderer.material.color;
        currentOrientation = Orientation.UP;
    }
    void OnMouseEnter()
    {
        if (GameManager.Instance.gameState != GameState.ShipPlacement)
        {
            return;
        }
        _mRenderer.material.color = _highlightedColor;
    }

    public IEnumerator FlashRed()
    {
        _mRenderer.material.color = _misplaceColor;
        yield return new WaitForSeconds(0.1f);
        _mRenderer.material.color = _meshColor;
    }

    public IEnumerator SinkShip()
    {
        _mRenderer.material.color = Color.black;
        float time = 0.0f;
        float duration = 0.5f;
        while (time < duration)
        {
            Vector3 pos = transform.position;
            pos.y -= 0.002f;
            transform.position = pos;
            time += Time.unscaledDeltaTime;
            yield return null;
        }
    }

    void OnMouseDown()
    {
        if (GameManager.Instance.gameState != GameState.ShipPlacement || ShipManager.Instance.CurrentlySelected != null)
        {
            return;
        }

        _isSelected = true;

        if (_isPlaced)
        {
            _isPlaced = false;
            Vector3 currentPos = transform.position;
            BoardManager.Instance.UpdateTiles(currentOrientation, shiptype, size, new Vector2(currentPos.x, currentPos.z), false, TileType.PLAYER);
            ShipManager.Instance.numPlaced--;
        }

        _meshColor.a = 0.5f;
        _mRenderer.material.color = _meshColor;
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        ShipManager.Instance.CurrentlySelected = this;
    }

    void OnMouseExit()
    {
        if (GameManager.Instance.gameState != GameState.ShipPlacement)
        {
            return;
        }
        _mRenderer.material.color = _meshColor;
    }

    private void RotateShip()
    {
        if (!_isSelected) return;

        Vector3 eulers = gameObject.transform.eulerAngles;
        eulers.y += 90.0f;
        if (eulers.y >= 360.0f)
        {
            eulers.y = 0.0f;
        }
        gameObject.transform.rotation = Quaternion.Euler(eulers);
        if (currentOrientation == Orientation.RIGHT)
        {
            currentOrientation = 0;
        }
        else
        {
            currentOrientation++;
        }
        BoardManager.Instance.ScrubHoverTiles();
    }

    public void RotateShip(Orientation newOrientation) {
        Vector3 eulers = gameObject.transform.eulerAngles;
        if (newOrientation == Orientation.DOWN) {
            eulers.y = 180.0f;
        } else if (newOrientation == Orientation.RIGHT) {
            eulers.y = 90.0f;
        } else if (newOrientation == Orientation.LEFT) {
            eulers.y = 270.0f;
        } else {
            eulers.y = 0.0f;
        }
        transform.rotation = Quaternion.Euler(eulers);
    }

    public void PlaceShip(Vector2 tilePos, bool random)
    {
        if (!random) {
            _isSelected = false;
            _meshColor.a = 1.0f;
            _mRenderer.material.color = _meshColor;
            gameObject.layer = LayerMask.NameToLayer("Default");
            ShipManager.Instance.CurrentlySelected = null;
        }
        _isPlaced = true;
        BoardManager.Instance.UpdateTiles(currentOrientation, shiptype, size, tilePos, true, TileType.PLAYER);
        ShipManager.Instance.numPlaced++;
        transform.position = new Vector3(tilePos.x, _boardY, tilePos.y);
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RotateShip();
        }
        if (_isSelected)
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3 mouseWorldPosition = _mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, _mainCamera.nearClipPlane + 12));
            transform.position = mouseWorldPosition;
        }
    }
}

public enum Orientation
{
    DOWN,
    LEFT,
    UP,
    RIGHT,
    LOST
}

public enum Shiptype
{
    EMPTY,
    CARRIER,
    BATTLESHIP,
    CRUISER,
    SUBMARINE,
    DESTROYER
}
