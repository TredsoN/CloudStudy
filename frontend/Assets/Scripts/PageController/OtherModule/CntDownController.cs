using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CntDownController : MonoBehaviour
{
    float ContentHeight = 25f, ItemPos = -25f;

    GameObject Content;

    List<CntDown> CntDownList;

    /// <summary>
    /// 脚本加载
    /// </summary>
    private void Awake()
    {
        Content = GameObject.Find("CntDownPage/Scroll/View/Content");
        GameObject CntDownPage;
        CntDownPage = GameObject.Find("CntDownPage");
        CntDownPage.transform.SetParent(GameObject.Find("Canvas").transform);
        CntDownPage.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenHeight);
        CntDownPage.GetComponent<RectTransform>().localPosition = new Vector2(BasicInformation.ScreenWidth, 0);
        CntDownPage.transform.DOMoveX(BasicInformation.ScreenWidth / 2, 0.1f);
        GameObject.Find("CntDownPage/Return").GetComponent<Button>().onClick.AddListener(() => { OnReturnClicked(); });
        GameObject.Find("CntDownPage/Add").GetComponent<Button>().onClick.AddListener(() => { OnAddClicked(); });
    }

    /// <summary>
    /// 接收倒计时列表
    /// </summary>
    public void GetCntDowns(List<CntDown> cntDowns)
    {
            CntDownList = cntDowns;
            RefreshCnt();
    }

    /// <summary>
    /// 接收删除倒计时id
    /// </summary>
    public void GetDeleteId(int id)
    {
        for (int i = 0; i < CntDownList.Count; i++)
        {
            if (CntDownList[i].Id == id)
            {
                CntDownList.RemoveAt(i);
                break;
            }
        }
        RefreshCnt();
    }

    /// <summary>
    /// 接收新增倒计时
    /// </summary>
    public void GetAddCnt(CntDown c)
    {
        CntDownList.Add(c);
        RefreshCnt();
    }

    /// <summary>
    /// 接收修改的倒计时
    /// </summary>
    public void GetChangeCnt(CntDown c)
    {
        for (int i = 0; i < CntDownList.Count; i++)
        {
            if (CntDownList[i].Id == c.Id)
            {
                CntDownList[i].Name=c.Name;
                CntDownList[i].Remark = c.Remark;
                CntDownList[i].EndTime = c.EndTime;
                CntDownList[i].Refresh();
                break;
            }
        }
        RefreshCnt();
    }

    /// <summary>
    /// 刷新界面
    /// </summary>
    private void RefreshCnt()
    {
        while (Content.transform.childCount > 0)
            DestroyImmediate(Content.transform.GetChild(0).gameObject);
        if (CntDownList.Count == 0)
        {
            GameObject.Find("EventSystem").SendMessage("GetCntCount", 3);
            GameObject.Find("CntDownPage/Scroll/Warn").GetComponent<CanvasGroup>().alpha = 1;
            return;
        }
        GameObject.Find("CntDownPage/Scroll/Warn").GetComponent<CanvasGroup>().alpha = 0;
        CntDownList.Sort((left, right) =>
        {
            if (left.Span < 0 && right.Span < 0) {
                if (left.Span < right.Span)
                    return 1;
                else if (left.Span == right.Span)
                    return 0;
                else
                    return -1;
            }
            else if(left.Span >= 0 && right.Span >= 0)
            {
                if (left.Span > right.Span)
                    return 1;
                else if (left.Span == right.Span)
                    return 0;
                else
                    return -1;
            }
            else
            {
                if (left.Span > right.Span)
                    return 1;
                else if (left.Span == right.Span)
                    return 0;
                else
                    return -1;
            }
        });
        if (CntDownList.Count >= 2)
        {
            GameObject.Find("EventSystem").SendMessage("GetCntCount", 2);
            CntDown c = CntDownList[0];
            GameObject.Find("EventSystem").SendMessage("GetCntDown1", c);
            c = CntDownList[1];
            GameObject.Find("EventSystem").SendMessage("GetCntDown2", c);
        }
        else if (CntDownList.Count == 1)
        {
            GameObject.Find("EventSystem").SendMessage("GetCntCount", 1);
            CntDown c = CntDownList[0];
            GameObject.Find("EventSystem").SendMessage("GetCntDown1", c);
        }
        ContentHeight = 25f;
        ItemPos = -25f;
        ContentHeight += 425 * CntDownList.Count;
        Content.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, ContentHeight);
        for (int i = 0; i < CntDownList.Count; i++)
        {
            GameObject CntDownItem;
            CntDownItem = (GameObject)Instantiate(Resources.Load(CntDownList[i].Span < 0?"Prefabs/OtherModule/CntDownItem": "Prefabs/OtherModule/CntDownItem2"));
            CntDownItem.name = "CntDown" + i;
            CntDownItem.transform.SetParent(GameObject.Find("CntDownPage/Scroll/View/Content").transform);
            CntDownItem.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, ItemPos);
            CntDownItem.GetComponent<RectTransform>().sizeDelta = new Vector2(0.96f * BasicInformation.ScreenWidth, 400f);
            GameObject.Find("CntDownPage/Scroll/View/Content/" + CntDownItem.name + "/Name").GetComponent<Text>().text = CntDownList[i].Name;
            GameObject.Find("CntDownPage/Scroll/View/Content/" + CntDownItem.name + "/Remark").GetComponent<Text>().text = CntDownList[i].Remark;
            GameObject.Find("CntDownPage/Scroll/View/Content/" + CntDownItem.name + "/State").GetComponent<Text>().text = CntDownList[i].Span < 0 ? "剩" + -CntDownList[i].Span + "天" : "已结束" + CntDownList[i].Span + "天";
            GameObject.Find("CntDownPage/Scroll/View/Content/" + CntDownItem.name + "/EndTime").GetComponent<Text>().text = CntDownList[i].EndTime.ToString("yyyy-MM-dd");
            CntDown s = CntDownList[i];
            GameObject.Find("CntDownPage/Scroll/View/Content/" + CntDownItem.name).GetComponent<Button>().onClick.AddListener(()=> { OnCntDownClicked(s); });
            ItemPos = ItemPos - 425f;
        }
    }

    /// <summary>
    /// 返回
    /// </summary>
    private void OnReturnClicked()
    {
        GameObject x = GameObject.Find("CntDownPage");
        x.transform.DOMoveX(3 * BasicInformation.ScreenWidth / 2, 0.1f).OnComplete(() => { Destroy(x); });
    }

    /// <summary>
    /// 点击倒计时
    /// </summary>
    private void OnCntDownClicked(CntDown cnt)
    {
        GameObject EditCntDownPage = (GameObject)Instantiate(Resources.Load("Prefabs/OtherModule/EditCntDownPage"));
        EditCntDownPage.name = "EditCntDownPage~";
        EditCntDownPage.AddComponent<CntDownInfoController>();
        EditCntDownPage.SendMessage("GetCntDown", cnt);
    }

    /// <summary>
    /// 添加倒计时
    /// </summary>
    private void OnAddClicked()
    {
        GameObject CreateCntDown = (GameObject)Instantiate(Resources.Load("Prefabs/OtherModule/CreateCntDownPage"));
        CreateCntDown.name = "CreateCntDownPage~";
        CreateCntDown.AddComponent<AddCntDownController>();
    }
}