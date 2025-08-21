using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ShopMenu",menuName ="Scriptable Objects/New Shop Item", order = -1)]
public class ShopItemSO : ScriptableObject
{
    public string label;
    public Sprite image;
    public int cost;
    public string type;
    public Color rgb;
    public Texture2D texture;

}
