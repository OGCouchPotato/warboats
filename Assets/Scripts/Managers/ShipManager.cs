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
    public GameObject shipDock;
    public GameObject fireParticles;
    public GameObject placeShipText;
    public GameObject placementTooltip;
    public List<Ship> ships;
    public Ship CurrentlySelected { get; set;}

    [HideInInspector]
    public  float numPlaced;

    [HideInInspector]
    public float playerHitCount = 0;

    [HideInInspector]
    public float enemyHitCount = 0;

    [HideInInspector]
    public bool hasWon = false;

    private List<GameObject> playerShips;
    private Vector3 _dockPosition;
    private float totalPossibleHits = 17;

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
        placeShipText.SetActive(true);
        placementTooltip.SetActive(false);
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
        hasWon = true;
    }

    public Ship GetShipByType(Shiptype shiptype) {
        foreach(GameObject _ship in playerShips) {
            Ship currentShip = _ship.GetComponent<Ship>();
            if (currentShip.shiptype == shiptype) {
                return currentShip;
            }
        }
        return null;
    }

    public void RandomShipPlacement() {
        BoardManager.Instance.ResetBoard();
        numPlaced = 0;
        foreach(GameObject _ship in playerShips) {
            Ship currentShip = _ship.GetComponent<Ship>();
            currentShip.currentOrientation = (Orientation)Random.Range(0, 4);
            currentShip.RotateShip(currentShip.currentOrientation);
            Vector2 randomTile = new Vector2(Random.Range(0, 10),Random.Range(0, 10));
            while(BoardManager.Instance.CanPlace(currentShip.currentOrientation, currentShip.size, randomTile, TileType.PLAYER) == false) {
                randomTile = new Vector2(Random.Range(0, 10),Random.Range(0, 10));
            }
            currentShip.PlaceShip(randomTile, true);
        }
    }

    public void EnemyShipPlacement() {
        for (int i = 0; i < ships.Count; i++) {
            Orientation randomOrientation = (Orientation)Random.Range(0, 4);
            Vector2 randomTile = new Vector2(Random.Range(0, 10),Random.Range(0, 10));
            while(BoardManager.Instance.CanPlace(randomOrientation, ships[i].size, randomTile, TileType.ENEMY) == false) {
                randomTile = new Vector2(Random.Range(0, 10),Random.Range(0, 10));
            }
            BoardManager.Instance.UpdateTiles(randomOrientation, ships[i].shiptype, ships[i].size, randomTile, true, TileType.ENEMY);
        }
        randomButton.SetActive(false);
        readyButton.SetActive(false);
        shipDock.SetActive(false);
        placeShipText.SetActive(false);
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
