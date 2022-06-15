using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                break;
            case GameState.OpponentTurn:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(gameState), gameState, null);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}

public enum GameState {
    GenerateBoard,
    ShipPlacement,
    PlayerTurn,
    OpponentTurn
}
