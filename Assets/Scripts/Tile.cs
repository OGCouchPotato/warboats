using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField]
    [HideInInspector]
    public GameObject selected;

    [HideInInspector]
    public TileType tileType;

    [HideInInspector]
    public Vector2 tileIndex;

    [HideInInspector]
    public bool isOccupied = false;

    [HideInInspector]
    public bool isMarked = false;

    [HideInInspector]
    public Shiptype shipType;

    private Color _hitColor = Color.red;
    private Color _missColor = Color.blue;
    private Color _sunkColor = new Color(1.0f, 0.65f, 0.0f, 1.0f);

    private MeshRenderer _meshRenderer;

    void Start()
    {
        _meshRenderer = selected.GetComponent<MeshRenderer>();
    }

    public IEnumerator LaunchMissile()
    {
        float time = 0.0f;
        float duration = 1.0f;
        GameObject launchedMissile;
        BoardManager.Instance.oneTilePerTurn = true;
        if (tileType == TileType.PLAYER)
        {
            yield return new WaitForSeconds(1.0f);
            launchedMissile = Instantiate(BoardManager.Instance.missile, new Vector3(tileIndex.x, 15.0f, tileIndex.y), Quaternion.Euler(0.0f, 0.0f, 180.0f));
        }
        else
        {
            launchedMissile = Instantiate(BoardManager.Instance.missile, new Vector3(15.0f, tileIndex.x, tileIndex.y), Quaternion.Euler(0.0f, 0.0f, 90.0f));
        }
        while (time < duration)
        {
            Vector3 pos = launchedMissile.transform.position;
            if (tileType == TileType.PLAYER)
            {
                pos.y -= 0.1f;
            }
            else
            {
                pos.x -= 0.1f;
            }

            launchedMissile.transform.position = pos;
            time += Time.unscaledDeltaTime;
            yield return null;
        }
        Destroy(launchedMissile);
        CheckAttack();
        yield return new WaitForSeconds(2.0f);
        if (ShipManager.Instance.hasWon == false)
        {
            BoardManager.Instance.oneTilePerTurn = false;
            if (GameManager.Instance.gameState == GameState.PlayerTurn)
            {
                GameManager.Instance.ChangeState(GameState.OpponentTurn);
            }
            else
            {
                GameManager.Instance.ChangeState(GameState.PlayerTurn);
            }
        }

    }

    private void CheckAttack()
    {
        isMarked = true;
        if (isOccupied)
        {
            if (tileType == TileType.PLAYER)
            {
                AIManager.Instance.SetHit(this);
                ShipManager.Instance.playerHitCount++;
            }
            else
            {
                ShipManager.Instance.enemyHitCount++;
            }

            List<Tile> tiles = BoardManager.Instance.CheckIfSunk(shipType, tileType);
            if (tiles != null)
            {
                if (tileType == TileType.ENEMY)
                {
                    selected.SetActive(true);
                    foreach (Tile _tile in tiles)
                    {
                        _tile.selected.GetComponent<MeshRenderer>().material.color = _sunkColor;
                    }
                } else {
                    Ship sunkShip = ShipManager.Instance.GetShipByType(shipType);
                    if (sunkShip != null) {
                        StartCoroutine(sunkShip.SinkShip());
                    }
                    
                    Instantiate(ShipManager.Instance.fireParticles, new Vector3(tileIndex.x, -1.27f, tileIndex.y), Quaternion.Euler(-90.0f, 0.0f, 0.0f));
                }

                ShipManager.Instance.CheckScore(tileType);
            }
            else
            {
                if (tileType == TileType.ENEMY)
                {
                    _meshRenderer.material.color = _hitColor;
                }
                else
                {
                    Instantiate(ShipManager.Instance.fireParticles, new Vector3(tileIndex.x, -1.27f, tileIndex.y), Quaternion.Euler(-90.0f, 0.0f, 0.0f));
                }
            }
        }
        else
        {
            selected.SetActive(true);
            _meshRenderer.material.color = _missColor;
        }

    }

    void OnMouseEnter()
    {
        if (GameManager.Instance.gameState == GameState.PlayerTurn)
        {
            if (tileType == TileType.ENEMY)
            {
                if (isMarked)
                {
                    return;
                }
                selected.SetActive(true);
            }
        }
        else if (GameManager.Instance.gameState == GameState.ShipPlacement)
        {
            if (tileType == TileType.PLAYER)
            {
                Ship currentShip = ShipManager.Instance.CurrentlySelected;
                if (currentShip == null) {
                    selected.SetActive(true);
                    return;
                }
                List<Tile> tiles = BoardManager.Instance.GetHoveringTiles(currentShip.currentOrientation, currentShip.size, tileIndex);
                if (tiles.Count != currentShip.size) {
                    foreach(Tile _tile in tiles) {
                       _tile.selected.GetComponent<MeshRenderer>().material.color = _hitColor;
                       _tile.selected.SetActive(true);
                    }
                    return;
                }
                foreach(Tile _tile in tiles) {
                    if(_tile.isOccupied) {
                        foreach(Tile tile in tiles) {
                            tile.selected.GetComponent<MeshRenderer>().material.color = _hitColor;
                            tile.selected.SetActive(true);
                        }
                        return;
                    }
                    _tile.selected.GetComponent<MeshRenderer>().material.color = Color.yellow;
                    _tile.selected.SetActive(true);
                }
                return;
            }
        }
        else
        {
            return;
        }
        return;
    }

    void OnMouseDown()
    {
        if (GameManager.Instance.gameState == GameState.PlayerTurn)
        {
            if (tileType == TileType.ENEMY && isMarked == false && BoardManager.Instance.oneTilePerTurn == false)
            {
                isMarked = true;
                StartCoroutine(LaunchMissile());
            }
        }
        else if (GameManager.Instance.gameState == GameState.ShipPlacement)
        {
            if (tileType == TileType.PLAYER && ShipManager.Instance.CurrentlySelected != null && isOccupied == false)
            {
                Ship currentShip = ShipManager.Instance.CurrentlySelected;
                if (BoardManager.Instance.CanPlace(currentShip.currentOrientation, currentShip.size, tileIndex, TileType.PLAYER))
                {
                    currentShip.PlaceShip(tileIndex, false);
                    BoardManager.Instance.ScrubHoverTiles();
                }
                else
                {
                    StartCoroutine(currentShip.FlashRed());
                }
            }
        }
        else
        {
            return;
        }
        return;
    }

    void OnMouseExit()
    {
        if (GameManager.Instance.gameState == GameState.PlayerTurn)
        {
            if (tileType == TileType.ENEMY)
            {
                if (isMarked)
                {
                    return;
                }
                selected.SetActive(false);
            }
        }
        else if (GameManager.Instance.gameState == GameState.ShipPlacement)
        {
            if (tileType == TileType.PLAYER)
            {
                Ship currentShip = ShipManager.Instance.CurrentlySelected;
                if (currentShip == null) {
                    selected.SetActive(false);
                    return;
                }
                List<Tile> tiles = BoardManager.Instance.GetHoveringTiles(currentShip.currentOrientation, currentShip.size, tileIndex);
                foreach(Tile _tile in tiles) {
                    _tile.selected.GetComponent<MeshRenderer>().material.color = Color.yellow;
                    _tile.selected.SetActive(false);
                }
            }
        }
        else
        {
            return;
        }
        return;
    }
}

public enum TileType
{
    PLAYER,
    ENEMY
}
