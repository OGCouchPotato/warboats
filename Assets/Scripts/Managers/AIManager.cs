using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    public static AIManager Instance;
    public TMPro.TextMeshProUGUI _text;
    private Difficulty _difficulty = Difficulty.HARD;
    private Mode _currentMode = Mode.RANDOM;
    private Tile _lastHit;
    private Tile _currentHit;
    private Orientation _currentOrientation = Orientation.LOST;
    private Axis _currentAxis = Axis.NONE;
    private bool _justSunk = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ToggleDifficulty();
    }

    public void SetHit(Tile hitTile)
    {
        _currentHit = hitTile;
    }

    public void SetSunk()
    {
        _justSunk = true;
    }

    private Tile RandomGuess()
    {
        float randomX = UnityEngine.Random.Range(0, 10);
        float randomY = UnityEngine.Random.Range(0, 10);
        Tile randomTile = BoardManager.Instance.GetPlayerTileAtPosition(new Vector2(randomX, randomY));
        while (randomTile.isMarked == true)
        {
            randomTile = BoardManager.Instance.GetPlayerTileAtPosition(new Vector2(UnityEngine.Random.Range(0, 10), UnityEngine.Random.Range(0, 10)));
        }
        return randomTile;
    }

    private Tile CardinalGuess()
    {
        if (_currentOrientation == Orientation.LOST)
        {
            _currentOrientation = 0;
        }
        else
        {
            _currentOrientation++;
        }
        if (_currentOrientation == Orientation.DOWN)
        {
            Vector2 guessPos = _currentHit.tileIndex;
            guessPos.x++;
            Tile guessTile = BoardManager.Instance.GetPlayerTileAtPosition(guessPos);
            if (guessTile == null || guessTile != null && guessTile.isMarked == true)
            {
                _currentOrientation++;
            }
            else
            {
                _currentAxis = Axis.VERTICAL;
                return guessTile;
            }
        }
        if (_currentOrientation == Orientation.LEFT)
        {
            Vector2 guessPos = _currentHit.tileIndex;
            guessPos.y--;
            Tile guessTile = BoardManager.Instance.GetPlayerTileAtPosition(guessPos);
            if (guessTile == null || guessTile != null && guessTile.isMarked == true)
            {
                _currentOrientation++;
            }
            else
            {
                _currentAxis = Axis.HORIZONTAL;
                return guessTile;
            }
        }
        if (_currentOrientation == Orientation.UP)
        {
            Vector2 guessPos = _currentHit.tileIndex;
            guessPos.x--;
            Tile guessTile = BoardManager.Instance.GetPlayerTileAtPosition(guessPos);
            if (guessTile == null || guessTile != null && guessTile.isMarked == true)
            {
                _currentOrientation++;
            }
            else
            {
                _currentAxis = Axis.VERTICAL;
                return guessTile;
            }
        }
        if (_currentOrientation == Orientation.RIGHT)
        {
            Vector2 guessPos = _currentHit.tileIndex;
            guessPos.y++;
            Tile guessTile = BoardManager.Instance.GetPlayerTileAtPosition(guessPos);
            if (guessTile == null || guessTile != null && guessTile.isMarked == true)
            {
                _currentAxis = Axis.NONE;
                _currentOrientation = Orientation.LOST;
                return RandomGuess();
            }
            else
            {
                _currentAxis = Axis.HORIZONTAL;
                return guessTile;
            }
        }
        return RandomGuess();
    }

    private Tile AxisGuess()
    {
        if (_currentHit != _lastHit)
        {
            _lastHit = _currentHit;
            if (_currentAxis == Axis.HORIZONTAL)
            {
                if (_currentOrientation == Orientation.RIGHT)
                {
                    Vector2 guessPos = _currentHit.tileIndex;
                    guessPos.y++;
                    Tile guessTile = BoardManager.Instance.GetPlayerTileAtPosition(guessPos);
                    if (guessTile != null && guessTile.isMarked == false)
                    {
                        return guessTile;
                    }
                    else
                    {
                        _currentHit = _lastHit;
                        _currentOrientation = Orientation.LEFT;
                        return AxisGuess();
                    }
                }
                if (_currentOrientation == Orientation.LEFT)
                {
                    Vector2 guessPos = _currentHit.tileIndex;
                    guessPos.y--;
                    Tile guessTile = BoardManager.Instance.GetPlayerTileAtPosition(guessPos);
                    if (guessTile != null && guessTile.isMarked == false)
                    {
                        return guessTile;
                    }
                    else
                    {
                        _currentHit = _lastHit;
                        _currentOrientation = Orientation.RIGHT;
                        return AxisGuess();
                    }
                }
            }
            if (_currentAxis == Axis.VERTICAL)
            {
                if (_currentOrientation == Orientation.DOWN)
                {
                    Vector2 guessPos = _currentHit.tileIndex;
                    guessPos.x++;
                    Tile guessTile = BoardManager.Instance.GetPlayerTileAtPosition(guessPos);
                    if (guessTile != null && guessTile.isMarked == false)
                    {
                        return guessTile;
                    }
                    else
                    {
                        _currentHit = _lastHit;
                        _currentOrientation = Orientation.UP;
                        return AxisGuess();
                    }
                }
                if (_currentOrientation == Orientation.UP)
                {
                    Vector2 guessPos = _currentHit.tileIndex;
                    guessPos.x--;
                    Tile guessTile = BoardManager.Instance.GetPlayerTileAtPosition(guessPos);
                    if (guessTile != null && guessTile.isMarked == false)
                    {
                        return guessTile;
                    }
                    else
                    {
                        _currentHit = _lastHit;
                        _currentOrientation = Orientation.DOWN;
                        return AxisGuess();
                    }
                }
            }
        }
        _lastHit = _currentHit;
        _currentAxis = Axis.NONE;
        _currentOrientation = Orientation.LOST;
        return RandomGuess();
    }

    /* If easy mode, random guess every move. If on hard mode, make a random guess until it gets a hit, then makes a guess in all cardinal directions
        if it finds another hit, limits next guesses to specific axis. If it doesn't get another hit, default back to random */
    public Tile MakeGuess()
    {
        if (_justSunk)
        {
            _currentHit = null;
            _justSunk = false;
        }

        if (_currentHit == null || (_currentHit == _lastHit && _currentOrientation == Orientation.LOST && _currentAxis == Axis.NONE))
        {
            _currentMode = Mode.RANDOM;
            _currentOrientation = Orientation.LOST;
            _currentAxis = Axis.NONE;
        }
        else if (_currentHit != _lastHit && _currentAxis == Axis.NONE)
        {
            _currentMode = Mode.CARDINAL;
        }
        else if (_currentHit != _lastHit && _currentAxis != Axis.NONE)
        {
            _currentMode = Mode.AXIS;
        }
        
        if (_difficulty == Difficulty.EASY || _currentMode == Mode.RANDOM)
        {
            _lastHit = _currentHit;
            return RandomGuess();
        }
        else if (_currentMode == Mode.CARDINAL)
        {
            _lastHit = _currentHit;
            return CardinalGuess();
        }
        else
        {
            return AxisGuess();
        }
    }

    public void ToggleDifficulty()
    {
        if (_difficulty == Difficulty.EASY)
        {
            _difficulty = Difficulty.HARD;
            _text.text = "AI: Hard";
        }
        else
        {
            _difficulty = Difficulty.EASY;
            _text.text = "AI: Easy";
        }
    }
}

public enum Difficulty
{
    EASY,
    HARD
}

public enum Mode
{
    RANDOM,
    CARDINAL,
    AXIS
}

public enum Axis
{
    HORIZONTAL,
    VERTICAL,
    NONE
}
