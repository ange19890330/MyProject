using UnityEngine;
using System.Collections;

public class GameSweet : MonoBehaviour {

    private int x;

    private int y;

    private GameManager.SweetType type;

    private MovedSweet movedComponent;

    private ColorSweet coloredComponent;

    [HideInInspector]
    public GameManager gameManager;

    public int X
    {
        get
        {
            return x;
        }

        set
        {
            if (CanMove())
            {
                x = value;
            }
        }
    }

    public int Y
    {
        get
        {
            return y;
        }

        set
        {
            if (CanMove())
            {
                y = value;
            }
        }
    }

    public GameManager.SweetType Type
    {
        get
        {
            return type;
        }
    }

    public MovedSweet MovedComponent
    {
        get
        {
            return movedComponent;
        }
    }

    public ColorSweet ColoredComponent
    {
        get
        {
            return coloredComponent;
        }
    }

    public bool CanMove()
    {
        return movedComponent != null;
    }

    public bool CanColor()
    {
        return coloredComponent != null;
    }

    void Awake()
    {
        movedComponent = GetComponent<MovedSweet>();
        coloredComponent = GetComponent<ColorSweet>();
    }

    public void Init(int _x, int _y, GameManager _gameManager, GameManager.SweetType _type)
    {
        x = _x;
        y = _y;
        gameManager = _gameManager;
        type = _type;
    }

    private void OnMouseEnter()
    {
        gameManager.EnterSweet(this);
    }

    private void OnMouseDown()
    {
        gameManager.PressSweet(this);
    }

    private void OnMouseUp()
    {
        gameManager.ReleaseSweet();
    }
}
