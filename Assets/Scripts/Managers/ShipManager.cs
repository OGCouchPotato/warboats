using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipManager : MonoBehaviour
{
    public static ShipManager Instance;

    public List<Ship> ships;
    public GameObject shipDock;
    //public GameObject phaseUI;

    private float numPlaced;
    private Vector3 _dockPosition;

    public Ship CurrentlySelected { get; set;}

    void Awake() {
        Instance = this;
    }

    void Start()
    {
        _dockPosition = new Vector3(3.5f, -1.27f, 12.25f);
    }

    public void Init() {
        foreach(Ship _ship in ships) {
            Instantiate(_ship.gameObject, _dockPosition, Quaternion.identity);
            _dockPosition.z += 1.0f;
        }
        shipDock.SetActive(true);
        //phaseUI.SetActive(true);
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }
}
