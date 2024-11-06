using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XLua;

//YuanShuJu
public class Model
{
    
}

public class ModelView
{
    //->
}

public class View
{
    
}
public class MonoConfig : MonoBehaviour
{
    //Base On FunctionName To Do Auto Bind & Regedit
    //GameObject : * Root
    //Panel : *Panel
    //Button : *Button / *Btn
    //Text : *TMP / *Text
    //Image : *Img / *Image
    //SpriteRenderer: *SR / *spriteRenderer
    //InputField : *IF / * InputField
    //DropDownButton : *DropDown
    public string LuaScriptName;
    public string LuaTableName;
    private void Start()
    {
        GameManager.Instance.luaEnv.DoString("require '{0}'", LuaScriptName);
        Init(GameManager.Instance.GetTable(LuaTableName));
    }

    public enum FieldType
    {
        GameObject = 0,
        RectTransform = 1,
        Button = 2,
        Text = 3,
        Image = 9,
        Slider = 4,
        Toggle = 5,
        DropDown =6,
        Input = 7,
        ScrollRect = 8,
        
        ScrollBar = 10,
        RawImage = 11,
        ToggleGroup = 12,
        CanvasGroup = 13,
        SpriteRenderer = 14,
        LayoutGroup = 15
    }
    public enum LuaFieldType
    {
        None,
        Normal,
        LuaMonoBase,
        CustomTable,
        CustomTableWithMono,
    }

    [Serializable]
    public class UILuaField
    {
        public FieldType type;
        
        public LuaFieldType luaType;
        
        public string tableName;

        private bool showLuaType()
        {
            return type == FieldType.GameObject;
        }

        private bool showLuaTableName()
        {
            return luaType == LuaFieldType.CustomTable || luaType == LuaFieldType.CustomTableWithMono;
        }

        public GameObject obj;

        public string customName;
    }

    public List<UILuaField> fields = new List<UILuaField>();

    public void Init(LuaTable table)
    {
        table["_go"] = this;
        RectTransform t = this.GetComponent<RectTransform>();
        if (t == null)
        {
            table["_tf"] = this.transform;
        }
        else
        {
            table["_tf"] = t;
        }
        
        //fieldCfger.enabled = false;
        for (int i = 0; i < fields.Count; i++)
        {
            MonoConfig.UILuaField field = fields[i];
            if (field.obj == null)
            {
                Debug.LogError(string.Format("{0} 下 序列 {1} gameobject没有绑定", field, i));
                continue;
            }

            string fieldName = field.customName;
            if (string.IsNullOrEmpty(fieldName))
            {
                fieldName = field.obj.name;
            }

            table[string.Format("{0}_go", fieldName)] = field.obj;
            table[string.Format("{0}_tf", fieldName)] = field.obj.transform;
            switch (field.type)
            {
                case MonoConfig.FieldType.GameObject:
                //if (field.luaType == MonoConfig.LuaFieldType.None)
                //{
                //    table[fieldName] = field.obj;
                //}
                //else if (field.luaType == MonoConfig.LuaFieldType.Normal)
                //{
                //    table[fieldName] = BindLua(field.obj, LuaMgr.me.CreateLuaTable());
                //}
                //else if (field.luaType == MonoConfig.LuaFieldType.LuaMonoBase)
                //{
                //    table[fieldName] = BindLuaMono(field.obj, LuaMgr.me.CreateLuaTable("LuaMonoBase"));
                //}
                //else if (field.luaType == MonoConfig.LuaFieldType.CustomTable)
                //{
                //    table[fieldName] = BindLua(field.obj, LuaMgr.me.CreateLuaTable(field.tableName));
                //}
                //else if (field.luaType == MonoConfig.LuaFieldType.CustomTableWithMono)
                //{
                //    table[fieldName] = BindLuaMono(field.obj, LuaMgr.me.CreateLuaTable(field.tableName));
                //}
                //break;
                case MonoConfig.FieldType.Button:
                    table[fieldName] = field.obj.GetComponent<Button>();
                    break;
                case MonoConfig.FieldType.Image:
                    table[fieldName] = field.obj.GetComponent<Image>();
                    break;
                case MonoConfig.FieldType.RectTransform:
                    table[fieldName] = field.obj.GetComponent<RectTransform>();
                    break;
                case MonoConfig.FieldType.ScrollRect:
                    table[fieldName] = field.obj.GetComponent<ScrollRect>();
                    break;
                case MonoConfig.FieldType.Input:
                    table[fieldName] = field.obj.GetComponent<InputField>();
                    break;
                case MonoConfig.FieldType.Slider:
                    table[fieldName] = field.obj.GetComponent<Slider>();
                    break;
                case MonoConfig.FieldType.Text:
                    table[fieldName] = field.obj.GetComponent<Text>();
                    break;
                case MonoConfig.FieldType.Toggle:
                    table[fieldName] = field.obj.GetComponent<Toggle>();
                    break;
                case MonoConfig.FieldType.DropDown:
                    table[fieldName] = field.obj.GetComponent<Dropdown>();
                    break;
                case MonoConfig.FieldType.ScrollBar:
                    table[fieldName] = field.obj.GetComponent<Scrollbar>();
                    break;
                case MonoConfig.FieldType.RawImage:
                    table[fieldName] = field.obj.GetComponent<RawImage>();
                    break;
                case MonoConfig.FieldType.ToggleGroup:
                    table[fieldName] = field.obj.GetComponent<ToggleGroup>();
                    break;
                case MonoConfig.FieldType.CanvasGroup:
                    table[fieldName] = field.obj.GetComponent<CanvasGroup>();
                    break;
                case MonoConfig.FieldType.SpriteRenderer:
                    table[fieldName] = field.obj.GetComponent<SpriteRenderer>();
                    break;
            }
        }
    }
}