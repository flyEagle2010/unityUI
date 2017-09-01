using UnityEngine;
using System.Collections;

public class AchievePanel : BaseManager {

    private CanvasGroup canvasGroup;

    void Start()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
    }

    public override void OnEnter()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;//控制透明度来控制面板的显示和隐藏
        canvasGroup.blocksRaycasts = true;
    }

    /// <summary>
    /// 处理页面的 关闭
    /// </summary>
    public override void OnExit()
    {
        canvasGroup.alpha = 0;//透明度为0，此时处于隐藏状态
        canvasGroup.blocksRaycasts = false;
    }

    public void OnClosePanel()
    {
        UIManager.GetInstance.PopPanel();
    }
}
