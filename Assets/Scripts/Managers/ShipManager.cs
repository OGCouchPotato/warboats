using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipManager : MonoBehaviour
{
    public static ShipManager Instance;
    public GameObject readyButton;
    public GameObject randomButton;
    public GameObject resultText;
    public GameObject replayButton;
    public List<Ship> ships;
    private List<GameObject> playerShips;
    public GameObject shipDock;
    public GameObject fireParticles;
    //public GameObject phaseUI;

    [HideInInspector]
    public  float numPlaced;
    private Vector3 _dockPosition;
    private float totalPossibleHits = 17;

    public float playerHitCount = 0;
    public float enemyHitCount = 0;
    public Ship CurrentlySelected { get; set;}

    void Awake() {
        Instance = this;
    }

    void Start()
    {
        _dockPosition = new Vector3(3.5f, -1.27f, 12.25f);
        playerShips = new List<GameObject>();
        replayButton.SetActive(false);
        resultText.SetActive(false);
    }

    public void Init() {
        foreach(Ship _ship in ships) {
            playerShips.Add(Instantiate(_ship.gameObject, _dockPosition, Quaternion.identity));
            _dockPosition.z += 1.0f;
        }
        shipDock.SetActive(true);
        randomButton.SetActive(true);
        //phaseUI.SetActive(true);
    }

    public void CheckScore(TileType tileType) {
        if (tileType == TileType.PLAYER && playerHitCount < totalPossibleHits) {
            return;
        }
        if (tileType == TileType.ENEMY && enemyHitCount < totalPossibleHits) {
            return;
        }

        string result;
        if (tileType == TileType.PLAYER) 
        {
            result = TileType.ENEMY.ToString() + " wins!";
        } else {
             result = TileType.PLAYER.ToString() + " wins!";
        }
       
        resultText.GetComponent<TMPro.TextMeshProUGUI>().text = result;
        resultText.SetActive(true);
        replayButton.SetActive(true);
    }

    public void RandomShipPlacement() {
        BoardManager.Instance.ResetBoard();
        foreach(GameObject _ship in playerShips) {
            Ship currentShip = _ship.GetComponent<Ship>();
            currentShip.currentOrientation = (Orientation)Random.Range(0, 3);
            currentShip.RotateShip(currentShip.currentOrientation);
            Vector2 randomTile = new Vector2(Random.Range(0, 9),Random.Range(0, 9));
            while(BoardManager.Instance.CanPlace(currentShip.currentOrientation, currentShip.size, randomTile, TileType.PLAYER) == false) {
                randomTile = new Vector2(Random.Range(0, 9),Random.Range(0, 9));
            }
            currentShip.PlaceShip(randomTile, true);
        }
    }

    public void EnemyShipPlacement() {
        for (int i = 0; i < ships.Count; i++) {
            Orientation randomOrientation = (Orientation)Random.Range(0, 3);
            Vector2 randomTile = new Vector2(Random.Range(0, 9),Random.Range(0, 9));
            while(BoardManager.Instance.CanPlace(randomOrientation, ships[i].size, randomTile, TileType.ENEMY) == false) {
                randomTile = new Vector2(Random.Range(0, 9),Random.Range(0, 9));
            }
            BoardManager.Instance.UpdateTiles(randomOrientation, ships[i].size, randomTile, true, TileType.ENEMY);
        }
        randomButton.SetActive(false);
        readyButton.SetActive(false);
        shipDock.SetActive(false);
        GameManager.Instance.ChangeState(GameState.PlayerTurn);
    }

    void Update()
    {
        if (numPlaced == 5 && readyButton.activeInHierarchy == false && GameManager.Instance.gameState == GameState.ShipPlacement) {
            readyButton.SetActive(true);
        } else if (numPlaced < 5 && readyButton.activeInHierarchy == true) {
            readyButton.SetActive(false);
        }
    }
}
