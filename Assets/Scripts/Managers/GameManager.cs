using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [HideInInspector]
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

    public void ExitApplication() {
        Application.Quit();
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
                Tile guessedTile = AIManager.Instance.MakeGuess();
                StartCoroutine(guessedTile.LaunchMissile());
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
