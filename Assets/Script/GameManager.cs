using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public enum SweetType
    {
        EMPTY,
        NORMAL,
        BARRIER,
        ROW_CLEAR,
        COLUMN_CLEAR,
        RAINBOWCANDY,
        /// <summary>
        /// 标记类型
        /// </summary>
        COUNT
    }

    /// <summary>
    /// 甜品预置体的字典
    /// </summary>
    public Dictionary<SweetType, GameObject> sweetPrefabDic;

    [System.Serializable]
    public struct sweetPrefab
    {
        public SweetType type;
        public GameObject prefab;
    }

    public sweetPrefab[] sweetPrefabs;

    //单例实例化
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            return _instance;
        }

        set
        {
            _instance = value;
        }
    }

    //大网格长宽
    public int xColumn;
    public int yRow;

    //填充时间
    public float fillTime;

    public GameObject gridPrefab;

    /// <summary>
    /// 甜品数组
    /// </summary>
    private GameSweet[,] sweets;

    private GameSweet pressedSweet;

    private GameSweet enteredSweet;

    void Awake()
    {
        _instance = this;
    }

    // Use this for initialization
    void Start ()
    {
        sweetPrefabDic = new Dictionary<SweetType, GameObject>();

        for(int i = 0; i < sweetPrefabs.Length; i++)
        {
            if(!sweetPrefabDic.ContainsKey(sweetPrefabs[i].type))
            {
                sweetPrefabDic.Add(sweetPrefabs[i].type, sweetPrefabs[i].prefab);
            }
        }

	    for(int i = 0; i < xColumn; i++)
        {
            for(int j = 0; j < yRow; j++)
            {
                GameObject grid = Instantiate(gridPrefab, CorrectPosition(i, j), Quaternion.identity) as GameObject;
                grid.transform.SetParent(this.transform);
            }
        }

        sweets = new GameSweet[xColumn, yRow];

        for (int i = 0; i < xColumn; i++)
        {
            for (int j = 0; j < yRow; j++)
            {
                //GameObject newSweet = Instantiate(sweetPrefabDic[SweetType.NORMAL], CorrectPosition(i, j), Quaternion.identity) as GameObject;
                //newSweet.transform.SetParent(this.transform);
                //sweets[i, j] = newSweet.GetComponent<GameSweet>();
                //sweets[i, j].Init(i, j, this, SweetType.NORMAL);

                //if(sweets[i,j].CanColor())
                //{
                //    sweets[i, j].ColoredComponent.SetColor((ColorSweet.ColorType)(Random.Range(0, sweets[i, j].ColoredComponent.NumColors)));
                //}
                CreateNewSweet(i, j, SweetType.EMPTY);
            }
        }

        Destroy(sweets[4, 4].gameObject);
        CreateNewSweet(4, 4, SweetType.BARRIER);

        //AllFill();
        StartCoroutine(AllFill());
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public Vector3 CorrectPosition(int x, int y)
    {
        return new Vector3(transform.position.x - xColumn / 2f + x, transform.position.y + yRow / 2f - y);
    }

    //产生甜品的方法
    public GameSweet CreateNewSweet(int x, int y, SweetType type)
    {
        GameObject newSweet = Instantiate(sweetPrefabDic[type], CorrectPosition(x, y), Quaternion.identity) as GameObject;
        newSweet.transform.parent = transform;

        sweets[x, y] = newSweet.GetComponent<GameSweet>();
        sweets[x, y].Init(x, y, this, type);

        return sweets[x, y];
    }

    /// <summary>
    /// 全部填充的方法
    /// </summary>
    public IEnumerator AllFill()
    {
        while(Fill())
        {
            yield return new WaitForSeconds(fillTime);
        }
    }



    /// <summary>
    /// 分步填充
    /// </summary>
    public bool Fill()
    {
        bool filledNotFinished = false;//判断本次填充是否完成

        for(int y = yRow - 2; y >= 0; y--)
        {
            for(int x = 0; x < xColumn; x++)
            {
                GameSweet sweet = sweets[x, y]; //得到当前元素位置tianpin

                if(sweet.CanMove()) //如果无法移动，则无法往下填充
                {
                    GameSweet sweetBelow = sweets[x, y + 1];

                    if(sweetBelow.Type == SweetType.EMPTY)
                    {
                        Destroy(sweetBelow.gameObject);
                        sweet.MovedComponent.Move(x, y + 1, fillTime);
                        sweets[x, y + 1] = sweet;
                        CreateNewSweet(x, y, SweetType.EMPTY);
                        filledNotFinished = true;
                    }
                    else   //斜向填充
                    {
                        for (int down = -1; down <= 1; down++)
                        {
                            if (down != 0)
                            {
                                int downX = x + down;

                                if (downX >= 0 && downX < xColumn)
                                {
                                    GameSweet downSweet = sweets[downX, y + 1];
                                    if (downSweet.Type == SweetType.EMPTY)
                                    {
                                        bool canfill = true;
                                        for (int aboveY = y; aboveY >= 0; aboveY--)
                                        {
                                            GameSweet sweetAbove = sweets[downX, aboveY];
                                            if (sweetAbove.CanMove())
                                            {
                                                break;
                                            }
                                            else if (!sweetAbove.CanMove() && sweetAbove.Type != SweetType.EMPTY)
                                            {
                                                canfill = false;
                                                break;
                                            }
                                        }

                                        if (!canfill)
                                        {
                                            Destroy(downSweet.gameObject);
                                            sweet.MovedComponent.Move(downX, y + 1, fillTime);
                                            sweets[downX, y + 1] = sweet;
                                            CreateNewSweet(x, y, SweetType.EMPTY);
                                            filledNotFinished = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        //最上排特殊情况
        for(int x =0; x < xColumn; x++)
        {
            GameSweet sweet = sweets[x, 0];

            if(sweet.Type == SweetType.EMPTY)
            {
               GameObject newSweet = Instantiate(sweetPrefabDic[SweetType.NORMAL], CorrectPosition(x, -1), Quaternion.identity) as GameObject;
                newSweet.transform.parent = transform;
                sweets[x, 0] = newSweet.GetComponent<GameSweet>();
                sweets[x, 0].Init(x, -1, this, SweetType.NORMAL);
                sweets[x, 0].MovedComponent.Move(x, 0, fillTime);
                sweets[x, 0].ColoredComponent.SetColor((ColorSweet.ColorType)(Random.Range(0, sweets[x, 0].ColoredComponent.NumColors)));
                filledNotFinished = true;
            }
        }

        return filledNotFinished;
    }

    //甜品是否相邻的判断方法
    private bool IsFriend(GameSweet sweet1, GameSweet sweet2)
    {
        return (sweet1.X == sweet2.X && Mathf.Abs(sweet1.Y - sweet2.Y) == 1) || (sweet1.Y == sweet2.Y && Mathf.Abs(sweet1.X - sweet2.X) == 1);
    }

    //交换两个甜品的方法
    private void ExChangeSweets(GameSweet sweet1, GameSweet sweet2)
    {
        if (sweet1.CanMove() && sweet2.CanMove())
        {
            sweets[sweet1.X, sweet1.Y] = sweet2;
            sweets[sweet2.X, sweet2.Y] = sweet1;

            int tempX = sweet1.X;
            int tempY = sweet1.Y;

            sweet1.MovedComponent.Move(sweet2.X, sweet2.Y, fillTime);
            sweet2.MovedComponent.Move(tempX, tempY, fillTime);
        }
    }

    public void PressSweet(GameSweet sweet)
    {
        pressedSweet = sweet;
    }

    public void EnterSweet(GameSweet sweet)
    {
        enteredSweet = sweet;
    }

    public void ReleaseSweet()
    {
        if(IsFriend(pressedSweet,enteredSweet))
        {
            ExChangeSweets(pressedSweet, enteredSweet);
        }
    }

    /// <summary>
    /// 匹配方法
    /// </summary>
    /// <param name="sweet">当前选中甜品</param>
    /// <param name="newX">位置，x坐标</param>
    /// <param name="newY">位置，Y坐标</param>
    /// <returns></returns>
    public List<GameSweet> MatchSweets(GameSweet sweet, int newX, int newY)
    {
        if(sweet.CanColor())
        {
            ColorSweet.ColorType color = sweet.ColoredComponent.Color;
            List<GameSweet> matchRowSweets = new List<GameSweet>();
            List<GameSweet> matchLineSweets = new List<GameSweet>();
            List<GameSweet> finishedMatchingSweets = new List<GameSweet>();

            //行匹配
            matchRowSweets.Add(sweet);

            //i=0代表往左，i=1代表往右
            for(int i = 0; i <= 1; i++)
            {
                for (int xDistance = 0; xDistance < xColumn; xDistance++)
                {
                    int x;
                    if (i == 0)
                    {
                        x = newX - xDistance;
                    }
                    else
                    {
                        x = newX + xDistance;
                    }
                    if (x < 0 || x >= xColumn)
                    {
                        break;
                    }

                    if(sweets[x,newY].CanColor() && sweets[x,newY].ColoredComponent.Color == color)
                    {
                        matchRowSweets.Add(sweets[x, newY]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            //检查一下当前行遍历列表中的元素数量是否大于3
            if(matchRowSweets.Count >= 3)
            {
                for(int i = 0; i < matchRowSweets.Count; i++)
                {
                    finishedMatchingSweets.Add(matchRowSweets[i]);
                }
            }

            if(finishedMatchingSweets.Count >= 3)
            {
                return finishedMatchingSweets;
            }
        }

        return null;
    }
}
