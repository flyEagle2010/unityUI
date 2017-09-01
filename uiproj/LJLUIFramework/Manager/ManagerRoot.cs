using UnityEngine;
using System.Collections;

public class ManagerRoot : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
    {
        UIManager.GetInstance.PushPanel(UIPanelTypeEnum.Main);
	}
	
}
