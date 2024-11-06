using System.Text.RegularExpressions;
using UnityEngine;
using XLua;

namespace Utility
{
    public class LuaUtility
    {
        public void InstantiateCanvas(string canvasName)
        {
            LuaTable table = GameManager.Instance.GetTable(canvasName);
            var canvasAssets = GameManager.Instance.GetAsset("entergamebundle", "canvas");
            if (canvasAssets == null)
                Debug.LogError("Load Base Asset Field");
            var entergameSceneCanvas = UnityEngine.GameObject.Instantiate(canvasAssets);
            //InstantiateCanvas
            foreach (var keys in table.GetKeys())
            {
                if (Regex.IsMatch(keys.ToString(), @"^.*?(button|slider)$"))
                {
                    
                }
            }
        }
    }
}