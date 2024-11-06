using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using XLua;

[CSharpCallLua]
public class GameManager : MonoBehaviour
{
    //启用Lua虚拟机
    //网络连接
    //检查资源可用性
    // Start is called before the first frame update
    
    public static GameManager Instance { get; private set; }
    private Socket so;
    private IPEndPoint remote;
    private IPEndPoint localhost;
    private bool restart;

    private float lastGCTime = 0;
    private float GCInterval = 1;

    private Action luaStart;
    private Action luaAwake;
    private Action luaUpdate;
    
    //Server
    private Socket serverSo;
    //AssetBundleManager
    private List<AssetBundle> resources = new List<AssetBundle>();
    private Dictionary<string, AssetBundle> assetBundles = new Dictionary<string, AssetBundle>();
    //UIRoot
    public GameObject UIRoot;
    //LuaTable
    private LuaTable mainLuaTable;
    public LuaEnv luaEnv { get; private set; }
    void Awake()
    {
        Instance = this;
        Init();
        if(luaAwake != null) luaAwake.Invoke();
    }
    void Start()
    {
        if(luaStart != null) luaStart.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        if(luaUpdate != null) luaUpdate.Invoke();
        if (Time.time - lastGCTime > GCInterval)
        {
            luaEnv.Tick();
            lastGCTime = Time.time;
        }
    }

    void Init()
    {
        //CreateCameraTable
        var cameraTable = GetTable("camera");
        cameraTable.Set("maincamera",Camera.main);
        LoadAssetBundle(Application.dataPath +"/AssetBundle/EGB/entergamebundle");
        //Don't Destroy OnLoad
        DontDestroyOnLoad(this);
        //Create LuaVM
        luaEnv = new LuaEnv();
        //LoadLuaFile
        TextAsset mainLua = Resources.Load<TextAsset>("Lua/main.lua");
        //LuaTable
        mainLuaTable = luaEnv.NewTable();
        using (LuaTable meta = luaEnv.NewTable())
        {
            meta.Set("__index", luaEnv.Global);
            mainLuaTable.SetMetaTable(meta);
        }
        mainLuaTable.Set("GameManager",this);
        mainLuaTable.Set("uiroot",UIRoot);
        mainLuaTable.Set("uiroot_transform",UIRoot.GetComponent<Transform>());
        //RunLuaScript
        luaEnv.DoString(mainLua.text,mainLua.name,mainLuaTable);
        
        //LoadFunction
        Action main = mainLuaTable.Get<Action>("main");
        luaAwake = mainLuaTable.Get<Action>("awake");
        luaStart = mainLuaTable.Get<Action>("start");
        luaUpdate = mainLuaTable.Get<Action>("update");
        //DoFunction
        if(main != null) main.Invoke();
    }

    public void LoadAssetBundle(string path)
    {
        var ab = AssetBundle.LoadFromFile(path);
        this.resources.Add(ab);
        this.assetBundles.Add(ab.name,ab);
    }

    public void UnloadAssetBundle(string names)
    {
        var ab = assetBundles[names];
        if (ab == null) return;
        assetBundles.Remove(names);
        resources.Remove(ab);
        ab.UnloadAsync(true);
    }
    
    public UnityEngine.Object GetAsset(string bundle, string name)
    {
        if (assetBundles.TryGetValue(bundle, out AssetBundle ab))
        {
            if (ab.Contains(name))
            {
                return ab.LoadAsset<UnityEngine.Object>(name);
            }
            else
            {
                Debug.Log($"Wrong AssetName:{name}");
            }
        }
        else
        {
            Debug.Log($"Can't Find Bundle:{bundle}");
        }
        return null;
    }

    public UnityEngine.Object[] GetAllAsset(string bundle)
    {
        if (assetBundles.TryGetValue(bundle, out AssetBundle ab))
        {
            return ab.LoadAllAssets();
            
        }

        return null;
    }
    public T[] GetAllAsset<T>(string bundle) where T : UnityEngine.Object
    {
        if (assetBundles.TryGetValue(bundle, out AssetBundle ab))
        {
            return ab.LoadAllAssets<T>();
        }

        return null;
    }
    /// <summary>
    /// Set Value To Target Table which Regedit in MainLuaTable
    /// </summary>
    /// <param name="name"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void SetTable(string name, string key, object value)
    {
        mainLuaTable.Get(name,out LuaTable table);
        table.Set(key,value);
    }
    /// <summary>
    /// Get Table Form MainLuaTable
    /// </summary>
    /// <param name="tableName"></param>
    /// <returns></returns>
    public LuaTable GetTable(string tableName)
    {
        var tab = mainLuaTable.Get<LuaTable>(tableName);
        if (tab == null)
        {
            tab = luaEnv.NewTable();
            mainLuaTable.Set(tableName,tab);
        }
        return tab;
    }
    public void Reset()
    {
        luaEnv.Dispose();
        luaEnv = null;
        this.Awake();
        this.Start();
    }
}
