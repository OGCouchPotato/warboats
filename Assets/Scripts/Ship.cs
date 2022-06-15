using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Ship : MonoBehaviour
{
    public float size;
    public Orientation currentOrientation; 
    private bool _isSelected = false;

    private Camera _mainCamera;
    private MeshRenderer _mRenderer;
    private Color _meshColor;
    private Color _highlightedColor = Color.yellow;

    private float _boardY = -1.27f;

    void Start() {
        _mainCamera = Camera.main;
        _mRenderer = gameObject.GetComponent<MeshRenderer>();
        _meshColor = _mRenderer.material.color;
        currentOrientation = Orientation.VERTICAL;
    }
    void OnMouseEnter() {
        if (GameManager.Instance.gameState != GameState.ShipPlacement) {
            return;
        }
       _mRenderer.material.color = _highlightedColor;
    }

    void OnMouseDown() {
        if (GameManager.Instance.gameState != GameState.ShipPlacement || _isSelected) {
            return;
        }
        _isSelected = true;
        _meshColor.a = 0.5f;
        _mRenderer.material.color = _meshColor;
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        ShipManager.Instance.CurrentlySelected = this;
    }  

    void OnMouseExit() {
        if (GameManager.Instance.gameState != GameState.ShipPlacement) {
            return;
        }
        _mRenderer.material.color = _meshColor;
    }

    private void RotateShip() {
        if (!_isSelected) return;

        Vector3 eulers = gameObject.transform.eulerAngles;
        eulers.y += 90.0f;
        if (eulers.y >= 360.0f) {
            eulers.y = 0.0f;
        }
        gameObject.transform.rotation = Quaternion.Euler(eulers);
        currentOrientation = currentOrientation == Orientation.HORIZONTAL ? Orientation.VERTICAL : Orientation.HORIZONTAL;
    }

    public void PlaceShip(Vector2 tilePos) {
        _isSelected = false;
        _meshColor.a = 1.0f;
        _mRenderer.material.color = _meshColor;
        gameObject.layer = LayerMask.NameToLayer("Default");
        ShipManager.Instance.CurrentlySelected = null;
        transform.position = new Vector3(tilePos.x, _boardY, tilePos.y);
    }

    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.R)) {
            RotateShip();
        }
        if (_isSelected) {
            Vector3 mousePos = Input.mousePosition;
            Vector3 mouseWorldPosition = _mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, _mainCamera.nearClipPlane+7));
            mouseWorldPosition.x += 2.0f;
            transform.position = mouseWorldPosition;
        }
    }
}

public enum Orientation {
    HORIZONTAL,
    VERTICAL
}
