using UnityEngine;
using System.Collections;

public class MainPanel : BaseManager
{
    private CanvasGroup canvasGroup;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
		Debug.Log ("mainPanel start...." + canvasGroup.name + "==" + canvasGroup.transform.parent);
    }


    public override void OnPause()
    {
        canvasGroup.blocksRaycasts = false;//当弹出新的面板的时候，让主菜单面板 不再和鼠标交互
    }
    public override void OnResume()
    {
        canvasGroup.blocksRaycasts = true;
    }

    public void OnPushPanel(string panelTypeString)
    {
        UIPanelTypeEnum panelType = (UIPanelTypeEnum)System.Enum.Parse(typeof(UIPanelTypeEnum), panelTypeString);
        UIManager.GetInstance.PushPanel(panelType);
    }
}
