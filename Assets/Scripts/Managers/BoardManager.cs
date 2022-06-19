using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;
    public GameObject missile;

    [SerializeField]
    public int _width, _height;

    [SerializeField]
    private Tile _tile;

    private Vector3 playerTileRotation = new Vector3(90.0f, -90.0f, 0.0f);
    private Vector3 oppTileRotation = new Vector3(0.0f, -90.0f, 90.0f);

    private Dictionary<Vector2, Tile> playerTiles;
    private Dictionary<Vector2, Tile> opponentTiles;
    public delegate Tile GetTileAtPosition(Vector2 tilePos);
    public GetTileAtPosition tileDelegate;

    public bool oneTilePerTurn = false;

    void Awake()
    {
        Instance = this;
    }

    public void GenerateBoard()
    {
        playerTiles = new Dictionary<Vector2, Tile>();
        opponentTiles = new Dictionary<Vector2, Tile>();
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Vector2 index = new Vector2(x, y);
                Tile playerSpawnedTile = Instantiate(_tile, new Vector3(x, -1.5f, y), Quaternion.Euler(playerTileRotation));
                playerSpawnedTile.name = $"Player Tile {x} {y}";
                playerSpawnedTile.tileType = TileType.PLAYER;
                playerSpawnedTile.tileIndex = index;
                playerSpawnedTile.isOccupied = false;
                playerSpawnedTile.shipType = Shiptype.EMPTY;
                playerTiles[index] = playerSpawnedTile;

                Tile oppSpawnedTile = Instantiate(_tile, new Vector3(-1.0f, x, y), Quaternion.Euler(oppTileRotation));
                oppSpawnedTile.name = $"Opponent Tile {x} {y}";
                oppSpawnedTile.tileType = TileType.ENEMY;
                oppSpawnedTile.tileIndex = index;
                oppSpawnedTile.isOccupied = false;
                oppSpawnedTile.shipType = Shiptype.EMPTY;
                opponentTiles[new Vector2(x, y)] = oppSpawnedTile;
            }
        }
        GameManager.Instance.ChangeState(GameState.ShipPlacement);
    }

    public List<Tile> CheckIfSunk(Shiptype shipType, TileType tileType)
    {
        Dictionary<Vector2, Tile> tiles = tileType == TileType.PLAYER ? playerTiles : opponentTiles;
        List<Tile> result = new List<Tile>();
        foreach (Tile _tile in tiles.Values)
        {
            if (_tile.shipType != shipType)
            {
                continue;
            }
            if (_tile.isMarked == false)
            {
                return null;
            }

            result.Add(_tile);
        }
        if (tileType == TileType.PLAYER) {
            AIManager.Instance.SetSunk();
        }
        return result;
    }

    public void ResetBoard()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                Vector2 index = new Vector2(i, j);
                playerTiles[index].isOccupied = false;
                playerTiles[index].shipType = Shiptype.EMPTY;
                opponentTiles[index].isOccupied = false;
                opponentTiles[index].shipType = Shiptype.EMPTY;
            }
        }
    }

    public void ScrubHoverTiles()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                Vector2 index = new Vector2(i, j);
                if (playerTiles[index].isMarked == false && playerTiles[index].selected.activeInHierarchy)
                {
                    playerTiles[index].selected.SetActive(false);
                }
                if (opponentTiles[index].isMarked == false && opponentTiles[index].selected.activeInHierarchy)
                {
                    opponentTiles[index].selected.SetActive(false);
                }
            }
        }
    }

    public void UpdateTiles(Orientation shipOrientation, Shiptype shipType, float shipSize, Vector2 tilePos, bool placement, TileType tileType)
    {
        tileDelegate = tileType == TileType.PLAYER ? BoardManager.Instance.GetPlayerTileAtPosition : BoardManager.Instance.GetOpponentTileAtPosition;
        if (placement == false)
        {
            shipType = Shiptype.EMPTY;
        }
        tileDelegate(tilePos).isOccupied = placement;
        tileDelegate(tilePos).shipType = shipType;
        if (shipSize == 5)
        {
            for (int i = 1; i < 3; i++)
            {
                Vector2 tile1 = tilePos;
                Vector2 tile2 = tilePos;
                if (shipOrientation == Orientation.LEFT || shipOrientation == Orientation.RIGHT)
                {
                    tile1.y += i;
                    tile2.y -= i;
                }
                else
                {
                    tile1.x += i;
                    tile2.x -= i;
                }
                tileDelegate(tile1).shipType = shipType;
                tileDelegate(tile1).isOccupied = placement;
                tileDelegate(tile2).shipType = shipType;
                tileDelegate(tile2).isOccupied = placement;
            }
        }
        else if (shipSize == 4)
        {
            Vector2 tile1 = tilePos;
            Vector2 tile2 = tilePos;
            Vector2 tile3 = tilePos;
            if (shipOrientation == Orientation.DOWN)
            {
                tile1.x -= 1;
                tile2.x += 1;
                tile3.x += 2;
            }
            else if (shipOrientation == Orientation.UP)
            {
                tile1.x += 1;
                tile2.x -= 1;
                tile3.x -= 2;
            }
            else if (shipOrientation == Orientation.LEFT)
            {
                tile1.y += 1;
                tile2.y -= 1;
                tile3.y -= 2;
            }
            else if (shipOrientation == Orientation.RIGHT)
            {
                tile1.y -= 1;
                tile2.y += 1;
                tile3.y += 2;
            }
            tileDelegate(tile1).isOccupied = placement;
            tileDelegate(tile1).shipType = shipType;
            tileDelegate(tile2).isOccupied = placement;
            tileDelegate(tile2).shipType = shipType;
            tileDelegate(tile3).isOccupied = placement;
            tileDelegate(tile3).shipType = shipType;
        }
        else
        {
            for (int i = 1; i < shipSize; i++)
            {
                Vector2 tile = tilePos;
                tileDelegate(tile).isOccupied = placement;
                tileDelegate(tile).shipType = shipType;
                if (shipOrientation == Orientation.UP)
                {
                    tile.x -= i;
                }
                else if (shipOrientation == Orientation.DOWN)
                {
                    tile.x += i;
                }
                else if (shipOrientation == Orientation.LEFT)
                {
                    tile.y -= i;
                }
                else if (shipOrientation == Orientation.RIGHT)
                {
                    tile.y += i;
                }
                tileDelegate(tile).isOccupied = placement;
                tileDelegate(tile).shipType = shipType;
            }
        }
    }

    public List<Tile> GetHoveringTiles(Orientation shipOrientation, float shipSize, Vector2 tilePos)
    {
        List<Tile> result = new List<Tile>();
        result.Add(GetPlayerTileAtPosition(tilePos));
        if (shipSize == 5)
        {
            for (int i = 1; i < 3; i++)
            {
                Vector2 check1 = tilePos;
                Vector2 check2 = tilePos;
                if (shipOrientation == Orientation.LEFT || shipOrientation == Orientation.RIGHT)
                {
                    check1.y += i;
                    check2.y -= i;
                }
                else
                {
                    check1.x += i;
                    check2.x -= i;
                }

                Tile tile1 = GetPlayerTileAtPosition(check1);
                Tile tile2 = GetPlayerTileAtPosition(check2);
                if (tile1 != null)
                {
                    result.Add(tile1);
                }
                if (tile2 != null)
                {
                    result.Add(tile2);
                }
            }
            return result;
        }
        else if (shipSize == 4)
        {
            Vector2 check1 = tilePos;
            Vector2 check2 = tilePos;
            Vector2 check3 = tilePos;
            if (shipOrientation == Orientation.DOWN)
            {
                check1.x -= 1;
                check2.x += 1;
                check3.x += 2;
            }
            else if (shipOrientation == Orientation.UP)
            {
                check1.x += 1;
                check2.x -= 1;
                check3.x -= 2;
            }
            else if (shipOrientation == Orientation.LEFT)
            {
                check1.y += 1;
                check2.y -= 1;
                check3.y -= 2;
            }
            else if (shipOrientation == Orientation.RIGHT)
            {
                check1.y -= 1;
                check2.y += 1;
                check3.y += 2;
            }
            Tile tile1 = GetPlayerTileAtPosition(check1);
            Tile tile2 = GetPlayerTileAtPosition(check2);
            Tile tile3 = GetPlayerTileAtPosition(check3);
            if (tile1 != null)
            {
                result.Add(tile1);
            }
            if (tile2 != null)
            {
                result.Add(tile2);
            }
            if (tile3 != null)
            {
                result.Add(tile3);
            }
            return result;
        }
        else
        {
            for (int i = 1; i < shipSize; i++)
            {
                Vector2 check = tilePos;
                if (shipOrientation == Orientation.UP)
                {
                    check.x -= i;
                }
                else if (shipOrientation == Orientation.DOWN)
                {
                    check.x += i;
                }
                else if (shipOrientation == Orientation.LEFT)
                {
                    check.y -= i;
                }
                else if (shipOrientation == Orientation.RIGHT)
                {
                    check.y += i;
                }
                Tile tile = GetPlayerTileAtPosition(check);
                if (tile != null) {
                    result.Add(tile);
                }
            }
            return result;
        }
    }

    public bool CanPlace(Orientation shipOrientation, float shipSize, Vector2 tilePos, TileType tileType)
    {
        tileDelegate = tileType == TileType.PLAYER ? BoardManager.Instance.GetPlayerTileAtPosition : BoardManager.Instance.GetOpponentTileAtPosition;
        if (tileDelegate(tilePos).isOccupied == true)
        {
            return false;
        }
        if (shipSize == 5)
        {
            for (int i = 1; i < 3; i++)
            {
                Vector2 check1 = tilePos;
                Vector2 check2 = tilePos;
                if (shipOrientation == Orientation.LEFT || shipOrientation == Orientation.RIGHT)
                {
                    check1.y += i;
                    check2.y -= i;
                }
                else
                {
                    check1.x += i;
                    check2.x -= i;
                }

                if (tileDelegate(check1) == null || tileDelegate(check2) == null)
                {
                    return false;
                }
                if (tileDelegate(check1).isOccupied == true || tileDelegate(check2).isOccupied == true)
                {
                    return false;
                }
            }
            return true;
        }
        else if (shipSize == 4)
        {
            Vector2 check1 = tilePos;
            Vector2 check2 = tilePos;
            Vector2 check3 = tilePos;
            if (shipOrientation == Orientation.DOWN)
            {
                check1.x -= 1;
                check2.x += 1;
                check3.x += 2;
            }
            else if (shipOrientation == Orientation.UP)
            {
                check1.x += 1;
                check2.x -= 1;
                check3.x -= 2;
            }
            else if (shipOrientation == Orientation.LEFT)
            {
                check1.y += 1;
                check2.y -= 1;
                check3.y -= 2;
            }
            else if (shipOrientation == Orientation.RIGHT)
            {
                check1.y -= 1;
                check2.y += 1;
                check3.y += 2;
            }
            if (tileDelegate(check1) == null || tileDelegate(check2) == null || tileDelegate(check3) == null)
            {
                return false;
            }
            if (tileDelegate(check1).isOccupied == true || tileDelegate(check2).isOccupied == true || tileDelegate(check3).isOccupied == true)
            {
                return false;
            }
            return true;
        }
        else
        {
            for (int i = 1; i < shipSize; i++)
            {
                Vector2 check = tilePos;
                if (shipOrientation == Orientation.UP)
                {
                    check.x -= i;
                }
                else if (shipOrientation == Orientation.DOWN)
                {
                    check.x += i;
                }
                else if (shipOrientation == Orientation.LEFT)
                {
                    check.y -= i;
                }
                else if (shipOrientation == Orientation.RIGHT)
                {
                    check.y += i;
                }
                if (tileDelegate(check) == null)
                {
                    return false;
                }
                if (tileDelegate(check).isOccupied == true)
                {
                    return false;
                }
            }
            return true;
        }
    }

    public Tile GetPlayerTileAtPosition(Vector2 pos)
    {
        if (playerTiles.TryGetValue(pos, out var tile))
        {
            return tile;
        }
        return null;
    }

    public Tile GetOpponentTileAtPosition(Vector2 pos)
    {
        if (opponentTiles.TryGetValue(pos, out var tile))
        {
            return tile;
        }
        return null;
    }
}
