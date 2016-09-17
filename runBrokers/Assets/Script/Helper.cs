
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Logic;
using System.Linq;
using FreeNet;

public class Helper : CSingleton<Helper>
{

    public Color ReturnColor(Common.PlayerColor color)
    {
        Color retColor;
        switch (color)
        {
            case Common.PlayerColor.PURPLE:
                retColor = new Color(0.47f, 0.16f, 0.56f);
                break;
            case Common.PlayerColor.GREEN:
                retColor = Color.green;
                break;
            case Common.PlayerColor.RED:
                retColor = Color.red;
                break;
            case Common.PlayerColor.YELLOW:
                retColor = Color.yellow;
                break;
            case Common.PlayerColor.WHITE:
                retColor = Color.white;
                break;

            default:
                retColor = Color.magenta;
                break;

        }

        return retColor;
    }

    public string ReturnColorTGext(Common.PlayerColor color)
    {
        string retColor;
        switch (color)
        {
            case Common.PlayerColor.PURPLE:
                retColor = "PURPLE";
                break;
            case Common.PlayerColor.GREEN:
                retColor = "GREEN";
                break;
            case Common.PlayerColor.RED:
                retColor = "RED";
                break;
            case Common.PlayerColor.YELLOW:
                retColor = "YELLOW";
                break;
            case Common.PlayerColor.WHITE:
                retColor = "WHITE";
                break;

            default:
                retColor = "DEFAULT";
                break;

        }

        return retColor;
    }

    public Common.PlayerColor nextColor(Common.PlayerColor currentColor)
    {
        int nextColor = (int)currentColor;
        nextColor++;
        nextColor %= Common.numColor;

        return (Common.PlayerColor)nextColor;
    }

    public Common.PlayerType nextType(Common.PlayerType currentType)
    {
        int nextType = (int)currentType;
        nextType++;
        nextType %= Common.numType;

        return (Common.PlayerType)nextType;
    }

    public void DescendingDictValue<key>(Dictionary<key, int> dict)
    {
        
    }

    public void DictDescendingOrderByValue<key>(ref Dictionary<key, int> dict)
    {
        /*dict.Clear();
        
        //foreach (var keyPair in items)
        {
            dict.Add(keyPair.Key, keyPair.Value);
        }*/

    }

    public T CopyComponent<T>(T original, GameObject destination) where T : Component
    {
        System.Type type = original.GetType();
        Component copy = destination.AddComponent(type);
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(original));
        }
        return copy as T;
    }

    public Object GetResource(string format, string filename)
    {
        var retResource = Resources.Load(format + "/" + filename);

        return retResource;
    }


}
