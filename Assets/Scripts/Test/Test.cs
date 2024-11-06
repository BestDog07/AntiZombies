using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;
[CSharpCallLua]
public class Test : MonoBehaviour
{
    public enum Event
    {
        OnLuaScriptChange
    }
    public static Test Instance { get; private set; }
    public static LuaEnv luaEnv { get; } = new LuaEnv();
    public TextAsset luaScript;
    public static LuaTable MainTable { get; private set; }
    private Action update;

    private Action awake;

    private Action start;
    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
        MainTable = luaEnv.NewTable();
        using (LuaTable meta = luaEnv.NewTable())
        {
            meta.Set("__index", luaEnv.Global);
            MainTable.SetMetaTable(meta);
        }
        luaEnv.DoString(luaScript.text, luaScript.name, MainTable);

        LuaTable test = luaEnv.NewTable();
        luaEnv.Global.Set("MainTable",MainTable);
        
        AssetBundle asset = AssetBundle.LoadFromFile(Application.dataPath + "/Resources/Test/testenter");
        GameObject[] objs = asset.LoadAllAssets<GameObject>();
        foreach (var obj in objs)
        {
            Debug.LogWarning(obj.name);
        }
        if(asset == null) Debug.LogError("Assets Null");
        var canvas = asset.LoadAsset<GameObject>("testentercanvas");
        if(canvas == null) Debug.LogError("Canvas Null");
        MainTable.Set("testentercanvas",canvas);
        
        awake = MainTable.Get<Action>("awake");
        update = MainTable.Get<Action>("update");
        awake?.Invoke();
    }

    void Start()
    {
        Action start = MainTable.Get<Action>("start");
        start?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        update?.Invoke();
        if(luaEnv != null)
            luaEnv.Tick();
    }

    private void OnDestroy()
    {
        
    }
    
    public LuaTable CreateTable(string tableName)
    {
        LuaTable table = luaEnv.NewTable();
        luaEnv.Global.Set(tableName,table);
        Debug.LogWarning(DateTime.Now);
        //Debug.LogWarning(tableName);
        //var tab = MainTable.Get<LuaTable>(tableName);
        //if(tab == null) Debug.LogError("Can't Get Table");
        return table;
    }

    public LuaTable GetTable(string tableName)
    {
        return MainTable.Get<LuaTable>(tableName);
    }
    
    public void OnLuaScriptChanged(LuaTable table = null)
    {
        if (table == null) table = MainTable;
        awake = MainTable.Get<Action>("awake");
        update = MainTable.Get<Action>("update");
        start = MainTable.Get<Action>("start");
        awake?.Invoke();
        start?.Invoke();
    }
    
}
//Unity:MainEnter -> InitializeLua -> LuaMainScript -> OtherLuaScript & UI 
//LuaMainScript -> InitializeNextBundle -> 
//
