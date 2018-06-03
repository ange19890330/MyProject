using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColorSweet : MonoBehaviour {

	public enum ColorType
    {
        YELLOW,
        PURPLE,
        RED,
        BLUE,
        GREEN,
        PINK,
        ANY,
        COUNT
    }

    [System.Serializable]
    public struct ColorSprite
    {
        public ColorType color;
        public Sprite sprite;
    }

    public ColorSprite[] colorSprites;

    private Dictionary<ColorType, Sprite> colorSpriteDict;

    private SpriteRenderer sprite;

    /// <summary>
    /// 我们所有的颜色的数量
    /// </summary>
    public int NumColors
    {
        get { return colorSprites.Length; }
    }

    public ColorType Color
    {
        get
        {
            return color;
        }

        set
        {
            SetColor(value);
        }
    }

    private ColorType color;

    void Awake()
    {
        sprite = transform.Find("sweet").GetComponent<SpriteRenderer>();

        colorSpriteDict = new Dictionary<ColorType, Sprite>();

        for(int i = 0; i < colorSprites.Length; i++)
        {
            if(!colorSpriteDict.ContainsKey(colorSprites[i].color))
            {
                colorSpriteDict.Add(colorSprites[i].color, colorSprites[i].sprite);
            }
        }
    }

    public void SetColor(ColorType newColor)
    {
        color = newColor;
        if(colorSpriteDict.ContainsKey(newColor))
        {
            sprite.sprite = colorSpriteDict[newColor];
        }
    }
}
