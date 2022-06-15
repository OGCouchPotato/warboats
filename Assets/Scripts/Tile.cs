using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField]
    private GameObject _selected;

    public TileType tileType;

    void OnMouseEnter() {
        if (GameManager.Instance.gameState == GameState.PlayerTurn) {
            if (tileType == TileType.ENEMY) {
                _selected.SetActive(true);
            }
        } else if (GameManager.Instance.gameState == GameState.ShipPlacement) {
            if (tileType == TileType.PLAYER) {
                _selected.SetActive(true);
            }
        } else {
            return;
        }
        return;
    }

    void OnMouseDown() {
        if (GameManager.Instance.gameState == GameState.PlayerTurn) {
            if (tileType == TileType.ENEMY) {
               // fire missile!
            }
        } else if (GameManager.Instance.gameState == GameState.ShipPlacement) {
            if (tileType == TileType.PLAYER && ShipManager.Instance.CurrentlySelected != null) {
                Ship currentShip = ShipManager.Instance.CurrentlySelected;
            }
        } else {
            return;
        }
        return;
    }

    void OnMouseExit() {
        if (GameManager.Instance.gameState == GameState.PlayerTurn) {
            if (tileType == TileType.ENEMY) {
                _selected.SetActive(false);
            }
        } else if (GameManager.Instance.gameState == GameState.ShipPlacement) {
            if (tileType == TileType.PLAYER) {
                _selected.SetActive(false);
            }
        } else {
            return;
        }
        return;
    }
}

public enum TileType {
    PLAYER,
    ENEMY
}
