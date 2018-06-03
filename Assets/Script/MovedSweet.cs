using UnityEngine;
using System.Collections;

public class MovedSweet : MonoBehaviour {

    private GameSweet sweet;

    private IEnumerator moveCoroutine; //这样得到其他指令的时候可硬终止协同程序

    void Awake()
    {
        sweet = GetComponent<GameSweet>();
    }

    /// <summary>
    /// 开启或者结束一个协程
    /// </summary>
    /// <param name="newX"></param>
    /// <param name="newY"></param>
    public void Move(int newX, int newY, float time)
    {
        if(moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        moveCoroutine = MoveCoroutine(newX, newY, time);
        StartCoroutine(moveCoroutine);

        //sweet.X = newX;
        //sweet.Y = newY;
        //sweet.transform.position = sweet.gameManager.CorrectPosition(newX, newY);
    }

    //负责移动的协程
    private IEnumerator MoveCoroutine(int newX, int newY, float time)
    {
        sweet.X = newX;
        sweet.Y = newY;

        //每一帧移动一点点
        Vector3 startPos = transform.position;
        Vector3 endPos = sweet.gameManager.CorrectPosition(newX, newY);

        for(float t = 0; t < time; t += Time.deltaTime )
        {
            sweet.transform.position = Vector3.Lerp(startPos, endPos, t / time);
            yield return 0;
        }

        //防止突发
        sweet.transform.position = endPos;
    }
}
