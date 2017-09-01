using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ManagerUI : MonoBehaviour
{

	//单例模式
	private static ManagerUI manager;
	private ArrayList panels;
	private Dictionary<UIPanelTypeEnum,BaseUI> uiDic = new Dictionary<UIPanelTypeEnum,BaseUI> ();
	public static ManagerUI GetInstance
	{
		get
		{
			if (manager == null)
			{
				manager = new ManagerUI();
			}

			return manager;
		}
	}

	private ManagerUI()
	{
//		Dederializer();
	}

	private Transform canvas;
	private Transform Canvas
	{
		get
		{
			if (canvas == null)
			{
				canvas = GameObject.Find("Canvas").transform;
			}
			return canvas;
		}
	}

	public void show(UIPanelTypeEnum type,bool isModel= true){
		BaseUI ui = GetUI (type);
		ui.OnEnter ();
	}


	private BaseUI GetUI(UIPanelTypeEnum panelType)
	{

		BaseUI ui;
		uiDic.TryGetValue(panelType,out ui);

		if (ui == null){
			string panelPath = "";

			GameObject goUI = GameObject.Instantiate(Resources.Load(panelPath)) as GameObject;
			goUI.transform.SetParent(canvas, false);

			uiDic.Add(panelType, goUI.GetComponent<BaseUI>());

			ui = goUI.GetComponent<BaseUI>();
		}
		return ui;
	}

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}

