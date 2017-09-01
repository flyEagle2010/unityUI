using UnityEngine;
using System.Collections;
using UIFrameworkForProtobuf;
using System.Collections.Generic;

public class UIManager 
{

    private Dictionary<UIPanelTypeEnum, string> dicPanelPath;//存储面板路径
    private Dictionary<UIPanelTypeEnum, BaseManager> dicPanelBase;//存储面板BaseManager组件
    private Stack<BaseManager> panelStack;

    //单例模式
    private static UIManager manager;

    public static UIManager GetInstance
    {
        get
        {
            if (manager == null)
            {
                manager = new UIManager();
            }

            return manager;
        }
    }

    private UIManager()
    {
        Dederializer();
    }

    private Transform canvasTransform;
    private Transform CanvasTransform
    {
        get
        {
            if (canvasTransform == null)
            {
                canvasTransform = GameObject.Find("Canvas").transform;
            }
            return canvasTransform;
        }
    }

    public void PushPanel(UIPanelTypeEnum panelType)
    {
        if (panelStack == null)
        {
            panelStack = new Stack<BaseManager>();
        }

        //判断栈里面是否有页面，如果有，将栈中top对象取出
        if (panelStack.Count > 0)
        {
            BaseManager top = panelStack.Peek();
            top.OnPause();
        }

        BaseManager bm = GetPanel(panelType);
        panelStack.Push(bm);

//        bm.OnEnter();
    }

    public void PopPanel()
    {
        if (panelStack == null)
        {
            panelStack = new Stack<BaseManager>();
        }

        if (panelStack.Count <= 0)
        {
            return;
        }

        //关闭栈顶的页面显示
        BaseManager bm = panelStack.Pop();
        bm.OnExit();

        if (panelStack.Count <= 0)
        {
            return;
        }
        BaseManager top = panelStack.Peek();
        top.OnResume();
    }


    private BaseManager GetPanel(UIPanelTypeEnum panelType)
    {
        if (dicPanelBase == null)
        {
            dicPanelBase = new Dictionary<UIPanelTypeEnum, BaseManager>();
        }

        BaseManager bm;
        dicPanelBase.TryGetValue(panelType,out bm);

        if (bm == null)
        {
            string panelPath;
            dicPanelPath.TryGetValue(panelType, out panelPath);

            GameObject objPanel = GameObject.Instantiate(Resources.Load(panelPath)) as GameObject;
			Debug.Log ("initiat...........");
            objPanel.transform.SetParent(CanvasTransform, false);
			Debug.Log ("add to parent.......");

            dicPanelBase.Add(panelType, objPanel.GetComponent<BaseManager>());

            return objPanel.GetComponent<BaseManager>();
        }

        else
        {
            return bm;
        }
    }

    //将protobuf文件反序列化为对象
    private void Dederializer()
    {
        dicPanelPath = new Dictionary<UIPanelTypeEnum, string>();

        UIPanelTypeList panelTypes = ProtobufHelper.DederializerFromFile<UIPanelTypeList>(Application.streamingAssetsPath + "/UIPanelType.bin");

        foreach (UIPanelType type in panelTypes.uiPanelTypes)
        {
            UIPanelTypeEnum enumType = (UIPanelTypeEnum)System.Enum.Parse(typeof(UIPanelTypeEnum), type.panelName);
            dicPanelPath.Add(enumType, type.panelPath);

            Debug.Log(type.panelName);
            Debug.Log(type.panelPath);
        }
    }
}
