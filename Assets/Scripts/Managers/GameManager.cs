using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState gameState;
    
    void Awake() {
        Instance = this;
    }

    void Start()
    {
        ChangeState(GameState.GenerateBoard);
    }

    public void Replay() {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void ChangeState(GameState newState) {
        gameState = newState;

        switch(gameState) {
            case GameState.GenerateBoard:
                BoardManager.Instance.GenerateBoard();
                break;
            case GameState.ShipPlacement:
                ShipManager.Instance.Init();
                break;
            case GameState.PlayerTurn:
                BoardManager.Instance.ScrubHoverTiles();
                StartCoroutine(CameraController.Instance.ToggleCameraMode(CameraMode.ATTACK));
                break;
            case GameState.OpponentTurn:
                BoardManager.Instance.ScrubHoverTiles();
                StartCoroutine(CameraController.Instance.ToggleCameraMode(CameraMode.DEFENSE));
                float randomX = UnityEngine.Random.Range(0, 9);
                float randomY = UnityEngine.Random.Range(0, 9);
                Tile randomTile = BoardManager.Instance.GetPlayerTileAtPosition(new Vector2(randomX, randomY));
                while(randomTile.isMarked == true) {
                    randomTile = BoardManager.Instance.GetPlayerTileAtPosition(new Vector2(UnityEngine.Random.Range(0, 9), UnityEngine.Random.Range(0, 9)));
                }
                StartCoroutine(randomTile.LaunchMissile());
                break;
            case GameState.EndScreen:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(gameState), gameState, null);
        }
    }
}

public enum GameState {
    GenerateBoard,
    ShipPlacement,
    PlayerTurn,
    OpponentTurn,
    EndScreen
}
