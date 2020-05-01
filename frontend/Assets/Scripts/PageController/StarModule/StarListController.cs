using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarListController : MonoBehaviour
{
    int Galaxy, Batch = 0, Index = 0;

    float ViewHeight, ContentHeight = 25f, ItemPos = -25f;

    RectTransform Content;

    GameObject StarListPage;

    List<Star> stars = new List<Star>();

    int changedstarindex;

    /// <summary>
    /// 脚本加载
    /// </summary>
    private void Awake()
    {
        ViewHeight = GameObject.Find("StarListPage/Scroll").GetComponent<RectTransform>().rect.height;
        Content = GameObject.Find("StarListPage/Scroll/View/Content").GetComponent<RectTransform>();

        StarListPage = GameObject.Find("StarListPage");
        StarListPage.transform.SetParent(GameObject.Find("Canvas").transform);
        StarListPage.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenHeight);
        StarListPage.GetComponent<RectTransform>().localPosition = new Vector2(BasicInformation.ScreenWidth, 0);
        StarListPage.transform.DOMoveX(BasicInformation.ScreenWidth / 2, 0.1f);
        GameObject.Find("StarListPage/Scroll").GetComponent<ScrollRect>().onValueChanged.AddListener((Vector2 x) => { OnScrollToEnd(); });
        GameObject.Find("StarListPage/Return").GetComponent<Button>().onClick.AddListener(() => { OnReturnClicked(); });
    }

    /// <summary>
    /// 接收星系信息
    /// </summary>
    public void GetMsg(int galaxy)
    {
        if (galaxy == 0)
            StarListPage.transform.GetChild(0).GetComponent<Text>().text = "情侣星系";
        else if (galaxy == 1)
            StarListPage.transform.GetChild(0).GetComponent<Text>().text = "考研星系";
        else if (galaxy == 2)
            StarListPage.transform.GetChild(0).GetComponent<Text>().text = "高考星系";
        else if (galaxy == 3)
            StarListPage.transform.GetChild(0).GetComponent<Text>().text = "外语星系";
        else if (galaxy == 4)
            StarListPage.transform.GetChild(0).GetComponent<Text>().text = "校园星系";
        else if (galaxy == 5)
            StarListPage.transform.GetChild(0).GetComponent<Text>().text = "外太空";
        else if (galaxy == 6)
        {
            StarListPage.transform.GetChild(0).GetComponent<Text>().text = "我的星球";
            Destroy(GameObject.Find("StarListPage/Add"));
            StartCoroutine(BasicInformation.StarInterface.IF04(BasicInformation.CurUser.Id));
            Batch = -1;
        }
        else if (galaxy == 7)
        {
            StarListPage.transform.GetChild(0).GetComponent<Text>().text = "搜索结果";
            Destroy(GameObject.Find("StarListPage/Add"));
            Batch = -1;
        }
        if (galaxy <= 5)
        {
            StartCoroutine(BasicInformation.StarInterface.IF03(galaxy, Batch));
            Batch++;
        }
        GameObject.Find("StarListPage/Add").GetComponent<Button>().onClick.AddListener(() => { OnAddClicked(galaxy); });
        Galaxy = galaxy;
    }

    /// <summary>
    /// 接收星球信息
    /// </summary>
    public void GetStars(List<Star> StarList)
    {
        stars.AddRange(StarList);
        if (StarList.Count == 0)
        {
            Batch = -1;
            return;
        }
        Destroy(GameObject.Find("StarListPage/Scroll/Warn"));
        if (StarList.Count < 10)
            Batch = -1;
        AddNewStars(StarList);
    }

    /// <summary>
    /// 获取修改的星球id
    /// </summary>
    public void GetChanegdId(int id)
    {
        for (int i = 0; i < stars.Count; i++)
            if (stars[i].Id == id)
            {
                changedstarindex = i;
                break;
            }
    }

    /// <summary>
    /// 获取退出星球的id
    /// </summary>
    /// <param name="id"></param>
    public void GetQuitedId(int id)
    {
        if (Galaxy == 6)
        {
            for (int i = 0; i < stars.Count; i++)
            {
                if (stars[i].Id == id)
                {
                    ContentHeight -= 375f;
                    Content.sizeDelta = new Vector2(BasicInformation.ScreenWidth, ContentHeight);
                    stars.RemoveAt(i);
                    Destroy(GameObject.Find("StarListPage/Scroll/View/Content/Star" + i));
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 获取修改的星球名
    /// </summary>
    public void GetChanegdName(string name)
    {
        stars[changedstarindex].Name = name;
        GameObject.Find("StarListPage/Scroll/View/Content/Star"+changedstarindex+"/Name").GetComponent<Text>().text = name;
    }

    /// <summary>
    /// 获取修改的星球介绍
    /// </summary>
    public void GetChanegdIntro(string intro)
    {
        stars[changedstarindex].Introduce = intro;
        GameObject.Find("StarListPage/Scroll/View/Content/Star" + changedstarindex + "/Intro").GetComponent<Text>().text = intro;
    }

    /// <summary>
    /// 接收新创建星球
    /// </summary>
    public void GetNewStar(Star star)
    {
        stars.Add(star);
        AddNewStars(new List<Star>() { star });
    }

    /// <summary>
    /// 返回
    /// </summary>
    private void OnReturnClicked()
    {
        GameObject x = GameObject.Find("StarListPage");
        x.transform.DOMoveX(3 * BasicInformation.ScreenWidth / 2, 0.1f).OnComplete(() => { Destroy(x); });
    }

    /// <summary>
    /// 动态添加星球
    /// </summary>
    /// <param name="stars">星球列表</param>
    private void AddNewStars(List<Star> stars)
    {
        ContentHeight += 375f * stars.Count;
        Content.sizeDelta = new Vector2(BasicInformation.ScreenWidth, ContentHeight);
        for (int i = 0; i < stars.Count; i++)
        {
            GameObject StarItem;
            if (stars[i].Type == 0)
                StarItem = (GameObject)Instantiate(Resources.Load("Prefabs/StarModule/StarItem0"));
            else if (stars[i].Type == 1)
                StarItem = (GameObject)Instantiate(Resources.Load("Prefabs/StarModule/StarItem1"));
            else
                StarItem = (GameObject)Instantiate(Resources.Load("Prefabs/StarModule/StarItem2"));
            StarItem.name = "Star" + Index;
            StarItem.transform.SetParent(GameObject.Find("StarListPage/Scroll/View/Content").transform);
            StarItem.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, ItemPos);
            StarItem.GetComponent<RectTransform>().sizeDelta = new Vector2(0.96f * BasicInformation.ScreenWidth, 350f);
            GameObject.Find("StarListPage/Scroll/View/Content/" + StarItem.name + "/ID").GetComponent<Text>().text = "ID:" + stars[i].StarId.ToString();
            GameObject.Find("StarListPage/Scroll/View/Content/" + StarItem.name + "/Name").GetComponent<Text>().text = stars[i].Name;
            GameObject.Find("StarListPage/Scroll/View/Content/" + StarItem.name + "/Intro").GetComponent<Text>().text = stars[i].Introduce;
            Star s = stars[i];
            GameObject.Find("StarListPage/Scroll/View/Content/" + StarItem.name).GetComponent<Button>().onClick.AddListener(() => { OnStarItemClicked(s); });
            ItemPos = ItemPos - 375f;
            Index++;
        }
    }

    /// <summary>
    /// 点击星球
    /// </summary>
    /// <param name="star">星球</param>
    private void OnStarItemClicked(Star star)
    {
        GameObject StarInfoPage;
        if (star.Type == 0)
        {
            StarInfoPage = (GameObject)Instantiate(Resources.Load("Prefabs/StarModule/StarInfoPage0"));
            StarInfoPage.name = "StarInfoPage";
            StarInfoPage.AddComponent<StarInfoController>();
        }
        else if (star.Type == 1)
        {
            StarInfoPage = (GameObject)Instantiate(Resources.Load("Prefabs/StarModule/StarInfoPage1"));
            StarInfoPage.name = "StarInfoPage";
            StarInfoPage.AddComponent<StarInfoController>();
        }
        else
        {
            StarInfoPage = (GameObject)Instantiate(Resources.Load("Prefabs/StarModule/StarInfoPage2"));
            StarInfoPage.name = "StarInfoPage";
            StarInfoPage.AddComponent<StarInfoController>();
        }
        StarInfoPage.SendMessage("GetMsg", star);
    }

    /// <summary>
    /// 滚到底部刷新
    /// </summary>
    private void OnScrollToEnd()
    {
        if (Content.anchoredPosition.y == ContentHeight - ViewHeight && Batch >= 0)
        {
            StartCoroutine(BasicInformation.StarInterface.IF03(Galaxy, Batch));
            Batch++;
        }
    }

    /// <summary>
    /// 创建星球
    /// </summary>
    /// <param name="Galaxy">星系Id</param>
    private void OnAddClicked(int Galaxy)
    {
        GameObject SelectTypePage = (GameObject)Instantiate(Resources.Load("Prefabs/StarModule/StarTypePage"));
        SelectTypePage.name = "SelectTypePage~";
        SelectTypePage.AddComponent<SelectTypeController>();
        SelectTypePage.SendMessage("GetMsg", Galaxy);
    }
}
