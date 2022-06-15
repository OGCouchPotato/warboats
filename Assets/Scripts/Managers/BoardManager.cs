using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;

    [SerializeField]
    public int _width, _height;

    [SerializeField]
    private Tile _tile;

    private Vector3 playerTileRotation = new Vector3(90.0f, -90.0f, 0.0f);
    private Vector3 oppTileRotation = new Vector3(0.0f, -90.0f, 0.0f);

    private Dictionary<Vector2, Tile> playerTiles;
    private Dictionary<Vector2, Tile> opponentTiles;

    void Awake() {
        Instance = this;
    }

    public void GenerateBoard() {
        playerTiles = new Dictionary<Vector2, Tile>();
        opponentTiles = new Dictionary<Vector2, Tile>();
        for (int x = 0; x < _width; x++) {
            for (int y = 0; y < _height; y++) {
                Tile playerSpawnedTile = Instantiate(_tile, new Vector3(x, -1.5f, y), Quaternion.Euler(playerTileRotation));
                playerSpawnedTile.name = $"Player Tile {x} {y}";
                playerSpawnedTile.tileType = TileType.PLAYER;
                playerTiles[new Vector2(x, y)] = playerSpawnedTile;

                Tile oppSpawnedTile = Instantiate(_tile, new Vector3(-1.0f, x, y), Quaternion.Euler(oppTileRotation));
                oppSpawnedTile.name = $"Opponent Tile {x} {y}";
                oppSpawnedTile.tileType = TileType.ENEMY;
                opponentTiles[new Vector2(x, y)] = oppSpawnedTile;
            }
        }
        GameManager.Instance.ChangeState(GameState.ShipPlacement);
    }

    public Tile GetPlayerTileAtPosition(Vector2 pos) {
        if (playerTiles.TryGetValue(pos, out var tile)) {
            return tile;
        }
        return null;
    }

    public Tile GetOpponentTileAtPosition(Vector2 pos) {
        if (playerTiles.TryGetValue(pos, out var tile)) {
            return tile;
        }
        return null;
    }
}
