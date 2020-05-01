using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
using XCharts;
using System.Collections.Generic;
using System;

public class MainController : MonoBehaviour
{
    private int Page = 1, Batch = 0, Category = 0;

    private float LastPos, LastTime;

    RectTransform Pages;

    /// <summary>
    /// 脚本加载
    /// </summary>
    private void Awake()
    {
        GameObject.Find("Nav_MyStars").GetComponent<Button>().onClick.AddListener(() => { OnNavigatorClicked(1); });
        GameObject.Find("Nav_Galaxy").GetComponent<Button>().onClick.AddListener(() => { OnNavigatorClicked(2); });
        GameObject.Find("Nav_Statistics").GetComponent<Button>().onClick.AddListener(() => { OnNavigatorClicked(3); });
        GameObject.Find("Pages/Page3/View/Content/HeadShot/HeadShotMask/HeadShot").GetComponent<Button>().onClick.AddListener(() => { OnHeadShotClicked(); });
        GameObject.Find("Pages/Page2/View/Content/Galaxy0").GetComponent<Button>().onClick.AddListener(() => { OnGalaxyClicked(0); });
        GameObject.Find("Pages/Page2/View/Content/Galaxy1").GetComponent<Button>().onClick.AddListener(() => { OnGalaxyClicked(1); });
        GameObject.Find("Pages/Page2/View/Content/Galaxy2").GetComponent<Button>().onClick.AddListener(() => { OnGalaxyClicked(2); });
        GameObject.Find("Pages/Page2/View/Content/Galaxy3").GetComponent<Button>().onClick.AddListener(() => { OnGalaxyClicked(3); });
        GameObject.Find("Pages/Page2/View/Content/Galaxy4").GetComponent<Button>().onClick.AddListener(() => { OnGalaxyClicked(4); });
        GameObject.Find("Pages/Page2/View/Content/Galaxy5").GetComponent<Button>().onClick.AddListener(() => { OnGalaxyClicked(5); });
        GameObject.Find("Pages/Page2/View/Content/MyStars").GetComponent<Button>().onClick.AddListener(() => { OnGalaxyClicked(6); });
        GameObject.Find("Pages/Page2/View/Content/SearchBar/Button").GetComponent<Button>().onClick.AddListener(() => { OnSearchClicked(); });
        GameObject.Find("Pages/Page1/View/Content/CountDownArea").GetComponent<Button>().onClick.AddListener(() => { OnCntDownClicked(); });
        GameObject.Find("Pages/Page1/View/Content/TodoListArea").GetComponent<Button>().onClick.AddListener(() => { OnTodoListClicked(); });
        Pages = GameObject.Find("Pages").GetComponent<RectTransform>();
        PageChange();

        GameObject.Find("Pages/Page3/View/Content/TimeStatic/Last").GetComponent<Button>().onClick.AddListener(() => { OnChart1LastClicked(); });
        GameObject.Find("Pages/Page3/View/Content/TimeStatic/Next").GetComponent<Button>().onClick.AddListener(() => { OnChart1NextClicked(); });
        GameObject.Find("Pages/Page3/View/Content/TimeStatic2/Last").GetComponent<Button>().onClick.AddListener(() => { OnChart2LastClicked(); });
        GameObject.Find("Pages/Page3/View/Content/TimeStatic2/Next").GetComponent<Button>().onClick.AddListener(() => { OnChart2NextClicked(); });
        BarChart barchart = GameObject.Find("Pages/Page3/View/Content/TimeStatic/ChartMask/Chart1").GetComponent<BarChart>();
        barchart.SetSize(BasicInformation.ScreenWidth * 0.96f, 550);
        GameObject.Find("Pages/Page3/View/Content/TimeStatic/ChartMask").GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth * 0.96f, 570);
        GameObject.Find("Pages/Page3/View/Content/TimeStatic/ChartMask").GetComponent<RectTransform>().anchoredPosition = new Vector2(BasicInformation.ScreenWidth * 0.02f, 30);
        GameObject.Find("Pages/Page3/View/Content/TimeStatic/ChartMask/Chart1").GetComponent<RectTransform>().anchoredPosition = new Vector2(BasicInformation.ScreenWidth * 0.02f, 20);
        barchart.themeInfo.backgroundColor = new Color(1, 1, 1, 0);
        barchart.themeInfo.axisLineColor = Color.white;
        barchart.themeInfo.axisTextColor = Color.white;
        barchart.themeInfo.titleTextColor = Color.white;
        barchart.title.show = false;
        barchart.yAxis0.minMaxType = Axis.AxisMinMaxType.Custom;
        barchart.yAxis0.axisLabel.fontSize = 25;
        barchart.yAxis0.axisTick.show = false;
        barchart.xAxis0.axisLabel.fontSize = 25;
        barchart.xAxis0.axisTick.show = false;

        PieChart piechart = GameObject.Find("Pages/Page3/View/Content/TimeStatic2/Chart2").GetComponent<PieChart>();
        piechart.SetSize(BasicInformation.ScreenWidth, 620);
        piechart.themeInfo.backgroundColor = new Color(1, 1, 1, 0);
        piechart.title.show = false;
    }

    /// <summary>
    /// 脚本每帧检测
    /// </summary>
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            LastPos = Pages.transform.position.x;
            LastTime = Time.time;
        }
        if (Input.GetMouseButtonUp(0))
        {
            float MoveDel = Pages.transform.position.x - LastPos;
            if ((Time.time - LastTime < 0.2 && MoveDel < 0) || MoveDel < -BasicInformation.ScreenWidth / 2)
                PageSwiftLeft();
            else if ((Time.time - LastTime < 0.2 && MoveDel > 0) || MoveDel > BasicInformation.ScreenWidth / 2)
                PageSwiftRight();
            else
                Pages.DOMoveX(-(Page - 1) * BasicInformation.ScreenWidth, 0.3f);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameObject Canvas = GameObject.Find("Canvas");
            GameObject FirstPage = Canvas.transform.GetChild(Canvas.transform.childCount - 1).gameObject;
            if (FirstPage.name == "MsgNote")
                FirstPage = Canvas.transform.GetChild(Canvas.transform.childCount - 2).gameObject;
            if (FirstPage.name.Contains("~") || FirstPage.name == "Warning")
                Destroy(FirstPage);
            else if (FirstPage.name == "LoadPage" || !FirstPage.name.Contains("Page"))
                return;
            else if (FirstPage.name == "StarStudyPage")
            {
                GameObject StarStudyPage = GameObject.Find("StarStudyPage");
                StarStudyPage.SendMessage("GetEndCall");
            }
            else
                FirstPage.transform.DOMoveX(3 * BasicInformation.ScreenWidth / 2, 0.1f).OnComplete(() => { Destroy(FirstPage); });
        }
    }

    /// <summary>
    /// 接收加载头像信息
    /// </summary>
    public void GetSignal()
    {
        StartCoroutine(DownloadHeadImage(BasicInformation.CurUser.HeadShot));
        GameObject.Find("Page3/View/Content/UserInfo/UserName").GetComponent<Text>().text = BasicInformation.CurUser.Name;
        GameObject.Find("Page3/View/Content/UserInfo/TotalTime").GetComponent<Text>().text = BasicInformation.CurUser.TotLearnTime.ToString();
        StartCoroutine(BasicInformation.OtherInterface.IF01(BasicInformation.CurUser.Id, Batch));
        StartCoroutine(BasicInformation.OtherInterface.IF02(BasicInformation.CurUser.Id, Category));
        StartCoroutine(BasicInformation.OtherInterface.IF05_0(BasicInformation.CurUser.Id));
        StartCoroutine(BasicInformation.OtherInterface.IF13_0(BasicInformation.CurUser.Id));
        StartCoroutine(BasicInformation.OtherInterface.IF15(BasicInformation.CurUser.Id));
    }

    /// <summary>
    /// 接收更新头像信息
    /// </summary>
    public void GetHeadshotUpdate()
    {
        StartCoroutine(DownloadHeadImage(BasicInformation.CurUser.HeadShot));
    }

    /// <summary>
    /// 接收学习时间
    /// </summary>
    public void GetDrawChart1(List<int> times)
    {
        bool flag = true;
        BarChart chart = GameObject.Find("Pages/Page3/View/Content/TimeStatic/ChartMask/Chart1").GetComponent<BarChart>();
        foreach (int i in times)
        {
            if (i > 1080)
            {
                chart.yAxis0.interval = 4f;
                chart.yAxis0.max = 24f;
                flag = false;
                break;
            }
            else if (i > 720)
            {
                chart.yAxis0.interval = 3f;
                chart.yAxis0.max = 18f;
                flag = false;
                break;
            }
            else if (i > 360)
            {
                chart.yAxis0.interval = 2f;
                chart.yAxis0.max = 12f;
                flag = false;
                break;
            }
        }
        if (flag)
        {
            chart.yAxis0.interval = 1f;
            chart.yAxis0.max = 6f;
        }
        chart.RemoveData();
        chart.AddSerie(SerieType.Bar);
        chart.series.list[0].barType = BarType.Capsule;
        chart.series.list[0].barWidth = 0.3f;
        chart.series.list[0].areaStyle.color = new Color(0.65f, 1, 0);
        chart.series.list[0].label.show = true;
        chart.series.list[0].label.position = SerieLabel.Position.Top;
        chart.series.list[0].label.fontSize = 30;
        chart.series.list[0].label.formatter = "{c}";
        chart.series.list[0].label.border = false;
        chart.series.list[0].label.color = Color.white;
        chart.series.list[0].label.offset = new Vector3(0, 20, 0);
        chart.AddXAxisData(DateTime.Now.AddDays(-(Batch + 1) * 6).ToString("MM.dd"));
        chart.AddData(0, (float)Math.Round((float)times[0] / 60, 2));
        chart.AddXAxisData(DateTime.Now.AddDays(-(Batch + 1) * 5).ToString("MM.dd"));
        chart.AddData(0, (float)Math.Round((float)times[1] / 60, 2));
        chart.AddXAxisData(DateTime.Now.AddDays(-(Batch + 1) * 4).ToString("MM.dd"));
        chart.AddData(0, (float)Math.Round((float)times[2] / 60, 2));
        chart.AddXAxisData(DateTime.Now.AddDays(-(Batch + 1) * 3).ToString("MM.dd"));
        chart.AddData(0, (float)Math.Round((float)times[3] / 60, 2));
        chart.AddXAxisData(DateTime.Now.AddDays(-(Batch + 1) * 2).ToString("MM.dd"));
        chart.AddData(0, (float)Math.Round((float)times[4] / 60, 2));
        chart.AddXAxisData(DateTime.Now.AddDays(-(Batch + 1) * 1).ToString("MM.dd"));
        chart.AddData(0, (float)Math.Round((float)times[5] / 60, 2));
        chart.AddXAxisData(DateTime.Now.AddDays(-Batch * 7).ToString("MM.dd"));
        chart.AddData(0, (float)Math.Round((float)times[6] / 60, 2));
        GameObject.Find("Pages/Page3/View/Content/TimeStatic/Loading").transform.DOScale(1, 0.3f).OnComplete(() =>
        {
            GameObject.Find("Pages/Page3/View/Content/TimeStatic/Loading").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.Find("Pages/Page3/View/Content/TimeStatic/Loading").transform.SetAsFirstSibling();
            GameObject.Find("Pages/Page3/View/Content/TimeStatic/ChartMask").GetComponent<CanvasGroup>().alpha = 1;
        });
    }

    /// <summary>
    /// 接收学习时间
    /// </summary>
    public void GetDrawChart2(List<int> times)
    {
        PieChart chart = GameObject.Find("Pages/Page3/View/Content/TimeStatic2/Chart2").GetComponent<PieChart>();
        chart.RemoveData();
        chart.AddSerie(SerieType.Pie);
        if (times[0] == 0 && times[1] == 0 && times[2] == 0 && times[3] == 0 && times[4] == 0 && times[5] == 0)
        {
            chart.series.list[0].radius[1] = BasicInformation.ScreenWidth * 0.2f;
            chart.series.list[0].label.show = true;
            chart.series.list[0].label.position = SerieLabel.Position.Inside;
            chart.series.list[0].label.fontSize = 30;
            chart.series.list[0].label.formatter = "{b}";
            chart.series.list[0].label.border = false;
            chart.series.list[0].label.color = Color.white;
            chart.AddData(0, 1, "暂无学习数据");
        }
        else
        {
            chart.series.list[0].radius[1] = BasicInformation.ScreenWidth * 0.2f;
            chart.series.list[0].label.show = true;
            chart.series.list[0].label.position = SerieLabel.Position.Outside;
            chart.series.list[0].label.fontSize = 30;
            chart.series.list[0].label.formatter = "{b}:{c}h";
            chart.series.list[0].label.border = false;
            chart.series.list[0].label.color = Color.black;
            chart.series.list[0].label.lineType = SerieLabel.LineType.Curves;
            chart.series.list[0].label.lineLength1 = BasicInformation.ScreenWidth * 0.03f;
            chart.series.list[0].label.lineLength2 = BasicInformation.ScreenWidth * 0.03f;
            if (times[0] != 0)
                chart.AddData(0, (float)Math.Round((float)times[0] / 60, 2), "情侣星系");
            if (times[1] != 0)
                chart.AddData(0, (float)Math.Round((float)times[1] / 60, 2), "考研星系");
            if (times[2] != 0)
                chart.AddData(0, (float)Math.Round((float)times[2] / 60, 2), "高考星系");
            if (times[3] != 0)
                chart.AddData(0, (float)Math.Round((float)times[3] / 60, 2), "外语星系");
            if (times[4] != 0)
                chart.AddData(0, (float)Math.Round((float)times[4] / 60, 2), "校园星系");
            if (times[5] != 0)
                chart.AddData(0, (float)Math.Round((float)times[5] / 60, 2), "外太空");
        }
        GameObject.Find("Pages/Page3/View/Content/TimeStatic2/Loading").transform.DOScale(1, 0.3f).OnComplete(() =>
        {

            GameObject.Find("Pages/Page3/View/Content/TimeStatic2/Chart2").GetComponent<CanvasGroup>().alpha = 1;
            GameObject.Find("Pages/Page3/View/Content/TimeStatic2/Loading").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.Find("Pages/Page3/View/Content/TimeStatic2/Loading").transform.SetAsFirstSibling();

        });
    }

    /// <summary>
    /// 接收刷新常用星球
    /// </summary>
    public void GetRefreshRecent(int type)
    {
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
        StartCoroutine(BasicInformation.OtherInterface.IF15(BasicInformation.CurUser.Id));
        StartCoroutine(BasicInformation.OtherInterface.IF01(BasicInformation.CurUser.Id, Batch));
        StartCoroutine(BasicInformation.OtherInterface.IF02(BasicInformation.CurUser.Id, Category));
        StartCoroutine(BasicInformation.OtherInterface.IF16(BasicInformation.CurUser.Id, type));
    }

    /// <summary>
    /// 更新学习总时长
    /// </summary>
    public void GetRefreshTotTime(int[] param)
    {
        if (param[0] == 1)
            GameObject.Find("Page3/View/Content/UserInfo/TotalTime").GetComponent<Text>().text = BasicInformation.CurUser.TotLearnTime.ToString();
        else if (param[0] == 2)
        {
            GameObject Warning = (GameObject)Instantiate(Resources.Load("Prefabs/Warning"));
            Warning.name = "Warning";
            Warning.transform.SetParent(GameObject.Find("Canvas").transform);
            Warning.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenHeight);
            Warning.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
            GameObject.Find("Warning/Info/Text").GetComponent<Text>().text = "本次学习时长" + param[1] + "分钟";
            GameObject.Find("Warning/Info/Submit").GetComponent<Button>().onClick.AddListener(() => { Destroy(Warning); });
            GameObject.Find("Warning/Info/Cancel").GetComponent<Button>().onClick.AddListener(() => { Destroy(Warning); });
            GameObject.Find("Page3/View/Content/UserInfo/TotalTime").GetComponent<Text>().text = BasicInformation.CurUser.TotLearnTime.ToString();
        }
        else if (param[0] == 3)
        {
            GameObject Warning = (GameObject)Instantiate(Resources.Load("Prefabs/Warning"));
            Warning.name = "Warning";
            Warning.transform.SetParent(GameObject.Find("Canvas").transform);
            Warning.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenHeight);
            Warning.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
            GameObject.Find("Warning/Info/Text").GetComponent<Text>().text = "你已中途退出，本次学习时长" + param[1] + "分钟";
            GameObject.Find("Warning/Info/Submit").GetComponent<Button>().onClick.AddListener(() => { Destroy(Warning); });
            GameObject.Find("Warning/Info/Cancel").GetComponent<Button>().onClick.AddListener(() => { Destroy(Warning); });
            GameObject.Find("Page3/View/Content/UserInfo/TotalTime").GetComponent<Text>().text = BasicInformation.CurUser.TotLearnTime.ToString();
        }
        else if (param[0] == 4)
        {
            GameObject Warning = (GameObject)Instantiate(Resources.Load("Prefabs/Warning"));
            Warning.name = "Warning";
            Warning.transform.SetParent(GameObject.Find("Canvas").transform);
            Warning.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenHeight);
            Warning.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
            GameObject.Find("Warning/Info/Text").GetComponent<Text>().text = "时间不早了休息一下吧，本次学习时长" + param[1] + "分钟";
            GameObject.Find("Warning/Info/Submit").GetComponent<Button>().onClick.AddListener(() => { Destroy(Warning); });
            GameObject.Find("Warning/Info/Cancel").GetComponent<Button>().onClick.AddListener(() => { Destroy(Warning); });
            GameObject.Find("Page3/View/Content/UserInfo/TotalTime").GetComponent<Text>().text = BasicInformation.CurUser.TotLearnTime.ToString();
        }
        else if (param[0] == 5)
        {
            GameObject Warning = (GameObject)Instantiate(Resources.Load("Prefabs/Warning"));
            Warning.name = "Warning";
            Warning.transform.SetParent(GameObject.Find("Canvas").transform);
            Warning.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenHeight);
            Warning.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
            GameObject.Find("Warning/Info/Text").GetComponent<Text>().text = "主播已下线，本次学习时长" + param[1] + "分钟";
            GameObject.Find("Warning/Info/Submit").GetComponent<Button>().onClick.AddListener(() => { Destroy(Warning); });
            GameObject.Find("Warning/Info/Cancel").GetComponent<Button>().onClick.AddListener(() => { Destroy(Warning); });
            GameObject.Find("Page3/View/Content/UserInfo/TotalTime").GetComponent<Text>().text = BasicInformation.CurUser.TotLearnTime.ToString();
        }
    }

    /// <summary>
    /// 获取常用星球
    /// </summary>
    public void GetRecentPlanets(List<Star> stars)
    {
        if (stars.Count == 0)
        {
            GameObject.Find("Pages/Page1/View/Content/RecentStar/Note").GetComponent<Text>().text = "暂无常用星球";
            GameObject.Find("Pages/Page1/View/Content/RecentStar/Star1").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.Find("Pages/Page1/View/Content/RecentStar/Star2").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.Find("Pages/Page1/View/Content/RecentStar/Star3").GetComponent<CanvasGroup>().alpha = 0;
        }
        else if (stars.Count == 1)
        {
            GameObject.Find("Pages/Page1/View/Content/RecentStar/Note").GetComponent<Text>().text = "";
            GameObject.Find("Pages/Page1/View/Content/RecentStar/Star1").GetComponent<CanvasGroup>().alpha = 1;
            GameObject.Find("Pages/Page1/View/Content/RecentStar/Star2").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.Find("Pages/Page1/View/Content/RecentStar/Star3").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.Find("Pages/Page1/View/Content/RecentStar/Star1/Pic").GetComponent<Image>().sprite = Resources.Load<Sprite>("Pictures/Elements/Planet" + stars[0].Type);
            GameObject.Find("Pages/Page1/View/Content/RecentStar/Star1/Name").GetComponent<Text>().text = stars[0].Name;
            GameObject.Find("Pages/Page1/View/Content/RecentStar/Star1").GetComponent<Button>().onClick.RemoveAllListeners();
            Star s = stars[0];
            GameObject.Find("Pages/Page1/View/Content/RecentStar/Star1").GetComponent<Button>().onClick.AddListener(() => { OnStarItemClicked(s); });
        }
        else if (stars.Count == 2)
        {
            GameObject.Find("Pages/Page1/View/Content/RecentStar/Note").GetComponent<Text>().text = "";
            GameObject.Find("Pages/Page1/View/Content/RecentStar/Star1").GetComponent<CanvasGroup>().alpha = 1;
            GameObject.Find("Pages/Page1/View/Content/RecentStar/Star2").GetComponent<CanvasGroup>().alpha = 1;
            GameObject.Find("Pages/Page1/View/Content/RecentStar/Star3").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.Find("Pages/Page1/View/Content/RecentStar/Star1/Pic").GetComponent<Image>().sprite = Resources.Load<Sprite>("Pictures/Elements/Planet" + stars[0].Type);
            GameObject.Find("Pages/Page1/View/Content/RecentStar/Star1/Name").GetComponent<Text>().text = stars[0].Name;
            GameObject.Find("Pages/Page1/View/Content/RecentStar/Star2/Pic").GetComponent<Image>().sprite = Resources.Load<Sprite>("Pictures/Elements/Planet" + stars[1].Type);
            GameObject.Find("Pages/Page1/View/Content/RecentStar/Star2/Name").GetComponent<Text>().text = stars[1].Name;
            GameObject.Find("Pages/Page1/View/Content/RecentStar/Star1").GetComponent<Button>().onClick.RemoveAllListeners();
            Star s = stars[0];
            GameObject.Find("Pages/Page1/View/Content/RecentStar/Star1").GetComponent<Button>().onClick.AddListener(() => { OnStarItemClicked(s); });
            GameObject.Find("Pages/Page1/View/Content/RecentStar/Star2").GetComponent<Button>().onClick.RemoveAllListeners();
            Star s1 = stars[1];
            GameObject.Find("Pages/Page1/View/Content/RecentStar/Star2").GetComponent<Button>().onClick.AddListener(() => { OnStarItemClicked(s1); });
        }
        else if (stars.Count == 3)
        {
            GameObject.Find("Pages/Page1/View/Content/RecentStar/Note").GetComponent<Text>().text = "";
            GameObject.Find("Pages/Page1/View/Content/RecentStar/Star1").GetComponent<CanvasGroup>().alpha = 1;
            GameObject.Find("Pages/Page1/View/Content/RecentStar/Star2").GetComponent<CanvasGroup>().alpha = 1;
            GameObject.Find("Pages/Page1/View/Content/RecentStar/Star3").GetComponent<CanvasGroup>().alpha = 1;
            GameObject.Find("Pages/Page1/View/Content/RecentStar/Star1/Pic").GetComponent<Image>().sprite = Resources.Load<Sprite>("Pictures/Elements/Planet" + stars[0].Type);
            GameObject.Find("Pages/Page1/View/Content/RecentStar/Star1/Name").GetComponent<Text>().text = stars[0].Name;
            GameObject.Find("Pages/Page1/View/Content/RecentStar/Star2/Pic").GetComponent<Image>().sprite = Resources.Load<Sprite>("Pictures/Elements/Planet" + stars[1].Type);
            GameObject.Find("Pages/Page1/View/Content/RecentStar/Star2/Name").GetComponent<Text>().text = stars[1].Name;
            GameObject.Find("Pages/Page1/View/Content/RecentStar/Star3/Pic").GetComponent<Image>().sprite = Resources.Load<Sprite>("Pictures/Elements/Planet" + stars[2].Type);
            GameObject.Find("Pages/Page1/View/Content/RecentStar/Star3/Name").GetComponent<Text>().text = stars[2].Name;
            GameObject.Find("Pages/Page1/View/Content/RecentStar/Star1").GetComponent<Button>().onClick.RemoveAllListeners();
            Star s = stars[0];
            GameObject.Find("Pages/Page1/View/Content/RecentStar/Star1").GetComponent<Button>().onClick.AddListener(() => { OnStarItemClicked(s); });
            GameObject.Find("Pages/Page1/View/Content/RecentStar/Star2").GetComponent<Button>().onClick.RemoveAllListeners();
            Star s1 = stars[1];
            GameObject.Find("Pages/Page1/View/Content/RecentStar/Star2").GetComponent<Button>().onClick.AddListener(() => { OnStarItemClicked(s1); });
            GameObject.Find("Pages/Page1/View/Content/RecentStar/Star3").GetComponent<Button>().onClick.RemoveAllListeners();
            Star s2 = stars[2];
            GameObject.Find("Pages/Page1/View/Content/RecentStar/Star3").GetComponent<Button>().onClick.AddListener(() => { OnStarItemClicked(s2); });
        }
    }

    /// <summary>
    /// 获取倒计时个数
    /// </summary>
    public void GetCntCount(int count)
    {
        if (count == 3)
        {
            GameObject.Find("Pages/Page1/View/Content/CountDownArea/Note").GetComponent<Text>().text = "暂无倒计时，快去添加吧！";
            GameObject.Find("Pages/Page1/View/Content/CountDownArea/CntTile1").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.Find("Pages/Page1/View/Content/CountDownArea/CntTile2").GetComponent<CanvasGroup>().alpha = 0;
        }
        else if (count == 1)
        {
            GameObject.Find("Pages/Page1/View/Content/CountDownArea/Note").GetComponent<Text>().text = "";
            GameObject.Find("Pages/Page1/View/Content/CountDownArea/CntTile1").GetComponent<CanvasGroup>().alpha = 1;
            GameObject.Find("Pages/Page1/View/Content/CountDownArea/CntTile2").GetComponent<CanvasGroup>().alpha = 0;
        }
        else
        {
            GameObject.Find("Pages/Page1/View/Content/CountDownArea/Note").GetComponent<Text>().text = "";
            GameObject.Find("Pages/Page1/View/Content/CountDownArea/CntTile1").GetComponent<CanvasGroup>().alpha = 1;
            GameObject.Find("Pages/Page1/View/Content/CountDownArea/CntTile2").GetComponent<CanvasGroup>().alpha = 1;
        }
    }

    /// <summary>
    /// 获取倒计时1
    /// </summary>
    public void GetCntDown1(CntDown c)
    {
        GameObject.Find("Pages/Page1/View/Content/CountDownArea/CntTile1/Name").GetComponent<Text>().text = c.Name;
        GameObject.Find("Pages/Page1/View/Content/CountDownArea/CntTile1/State").GetComponent<Text>().text = c.Span < 0 ? "剩" + -c.Span + "天" : "已结束" + c.Span + "天";
    }

    /// <summary>
    /// 获取倒计时2
    /// </summary>
    public void GetCntDown2(CntDown c)
    {
        GameObject.Find("Pages/Page1/View/Content/CountDownArea/CntTile2/Name").GetComponent<Text>().text = c.Name;
        GameObject.Find("Pages/Page1/View/Content/CountDownArea/CntTile2/State").GetComponent<Text>().text = c.Span < 0 ? "剩" + -c.Span + "天" : "已结束" + c.Span + "天";
    }

    /// <summary>
    /// 接收清单个数
    /// </summary>
    public void GetTodoCount(int count)
    {
        if (count == 3)
        {
            GameObject.Find("Pages/Page1/View/Content/TodoListArea/Note").GetComponent<Text>().text = "暂无待办，快去添加吧！";
            GameObject.Find("Pages/Page1/View/Content/TodoListArea/TodoTile1").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.Find("Pages/Page1/View/Content/TodoListArea/TodoTile2").GetComponent<CanvasGroup>().alpha = 0;
        }
        else if (count == 1)
        {
            GameObject.Find("Pages/Page1/View/Content/TodoListArea/Note").GetComponent<Text>().text = "";
            GameObject.Find("Pages/Page1/View/Content/TodoListArea/TodoTile1").GetComponent<CanvasGroup>().alpha = 1;
            GameObject.Find("Pages/Page1/View/Content/TodoListArea/TodoTile2").GetComponent<CanvasGroup>().alpha = 0;
        }
        else
        {
            GameObject.Find("Pages/Page1/View/Content/TodoListArea/Note").GetComponent<Text>().text = "";
            GameObject.Find("Pages/Page1/View/Content/TodoListArea/TodoTile1").GetComponent<CanvasGroup>().alpha = 1;
            GameObject.Find("Pages/Page1/View/Content/TodoListArea/TodoTile2").GetComponent<CanvasGroup>().alpha = 1;
        }
    }

    /// <summary>
    /// 接收清单1
    /// </summary>
    public void GetTodo1(ArrayList arrayList)
    {
        GameObject.Find("Pages/Page1/View/Content/TodoListArea/TodoTile1/Name").GetComponent<Text>().text = (string)arrayList[0];
        GameObject.Find("Pages/Page1/View/Content/TodoListArea/TodoTile1/State").GetComponent<Text>().text = (int)arrayList[1] + "条待办";
    }

    /// <summary>
    /// 接收清单2
    /// </summary>
    public void GetTodo2(ArrayList arrayList)
    {
        GameObject.Find("Pages/Page1/View/Content/TodoListArea/TodoTile2/Name").GetComponent<Text>().text = (string)arrayList[0];
        GameObject.Find("Pages/Page1/View/Content/TodoListArea/TodoTile2/State").GetComponent<Text>().text = (int)arrayList[1] + "条待办";
    }

    /// <summary>
    /// 显示提示框
    /// </summary>
    /// <param name="message">信息</param>
    private void ShowMsgNote(string message)
    {
        GameObject MsgNote = (GameObject)Instantiate(Resources.Load("Prefabs/MsgNote"));
        MsgNote.name = "MsgNote";
        MsgNote.transform.SetParent(GameObject.Find("Canvas").transform);
        MsgNote.transform.localScale = new Vector3(0f, 0f, 0f);
        MsgNote.transform.localPosition = new Vector3(0f, 0f, 0f);
        MsgNote.transform.GetChild(0).GetComponent<Text>().text = message;
        MsgNote.transform.DOScale(new Vector3(1f, 1f, 1f), 0.5F).OnComplete(() =>
        {
            MsgNote.transform.DOScaleX(1, 1F).OnComplete(() =>
            {
                MsgNote.transform.DOScale(new Vector3(0f, 0f, 0f), 0.5F).OnComplete(() =>
                {
                    Destroy(MsgNote);
                });
            });
        });
    }

    /// <summary>
    /// 页面左滑
    /// </summary>
    private void PageSwiftLeft()
    {
        if (Page == 3) return;
        Page++;
        Pages.DOMoveX(-(Page - 1) * BasicInformation.ScreenWidth, 0.3f);
        PageChange();
    }

    /// <summary>
    /// 页面右滑
    /// </summary>
    private void PageSwiftRight()
    {
        if (Page == 1) return;
        Page--;
        Pages.DOMoveX(-(Page - 1) * BasicInformation.ScreenWidth, 0.3f);
        PageChange();
    }

    /// <summary>
    /// 底部导航
    /// </summary>
    private void OnNavigatorClicked(int id)
    {
        if (id == 1)
        {
            Pages.DOMoveX(0, 0.3f);
            Page = 1;
        }
        else if (id == 2)
        {
            Pages.DOMoveX(-BasicInformation.ScreenWidth, 0.3f);
            Page = 2;
        }
        else if (id == 3)
        {
            Pages.DOMoveX(-2 * BasicInformation.ScreenWidth, 0.3f);
            Page = 3;
        }
        PageChange();
    }

    /// <summary>
    /// 点击头像
    /// </summary>
    private void OnHeadShotClicked()
    {
        if (BasicInformation.CurUser.IsOnline)
            ShowPersonalInfoPage();
        else
            ShowLoginPage();
    }

    /// <summary>
    /// 跳转星系界面
    /// </summary>
    /// <param name="id"></param>
    private void OnGalaxyClicked(int id)
    {
        if (BasicInformation.CurUser.IsOnline)
        {
            GameObject StarListPage = (GameObject)Instantiate(Resources.Load("Prefabs/StarModule/StarListPage"));
            StarListPage.name = "StarListPage";
            StarListPage.AddComponent<StarListController>();
            StarListPage.SendMessage("GetMsg", id);
        }
        else
            ShowLoginPage();
    }

    /// <summary>
    /// 统计图1向前
    /// </summary>
    private void OnChart1LastClicked()
    {
        if (!BasicInformation.CurUser.IsOnline)
            return;
        Batch++;
        StartCoroutine(BasicInformation.OtherInterface.IF01(BasicInformation.CurUser.Id, Batch));
    }

    /// <summary>
    /// 统计图2向前
    /// </summary>
    private void OnChart2LastClicked()
    {
        if (!BasicInformation.CurUser.IsOnline)
            return;
        if (Category == 0)
            Category = 3;
        Category = Category - 1;
        StartCoroutine(BasicInformation.OtherInterface.IF02(BasicInformation.CurUser.Id, Category));
        switch (Category)
        {
            case 0:
                {
                    GameObject.Find("Pages/Page3/View/Content/TimeStatic2/Title").GetComponent<Text>().text = "时间分布（当日）";
                    break;
                }
            case 1:
                {
                    GameObject.Find("Pages/Page3/View/Content/TimeStatic2/Title").GetComponent<Text>().text = "时间分布（当周）";
                    break;
                }
            case 2:
                {
                    GameObject.Find("Pages/Page3/View/Content/TimeStatic2/Title").GetComponent<Text>().text = "时间分布（当月）";
                    break;
                }
        }
    }

    /// <summary>
    /// 统计图1向下
    /// </summary>
    private void OnChart1NextClicked()
    {
        if (!BasicInformation.CurUser.IsOnline)
            return;
        if (Batch == 0)
            return;
        Batch--;
        StartCoroutine(BasicInformation.OtherInterface.IF01(BasicInformation.CurUser.Id, Batch));
    }

    /// <summary>
    /// 统计图2向下
    /// </summary>
    private void OnChart2NextClicked()
    {
        if (!BasicInformation.CurUser.IsOnline)
            return;
        if (Category == 2)
            Category = -1;
        Category = Category + 1;
        StartCoroutine(BasicInformation.OtherInterface.IF02(BasicInformation.CurUser.Id, Category));
        switch (Category)
        {
            case 0:
                {
                    GameObject.Find("Pages/Page3/View/Content/TimeStatic2/Title").GetComponent<Text>().text = "时间分布（当日）";
                    break;
                }
            case 1:
                {
                    GameObject.Find("Pages/Page3/View/Content/TimeStatic2/Title").GetComponent<Text>().text = "时间分布（当周）";
                    break;
                }
            case 2:
                {
                    GameObject.Find("Pages/Page3/View/Content/TimeStatic2/Title").GetComponent<Text>().text = "时间分布（当月）";
                    break;
                }
        }
    }

    /// <summary>
    /// 跳转倒计时界面
    /// </summary>
    private void OnCntDownClicked()
    {
        if (BasicInformation.CurUser.IsOnline)
        {
            GameObject CntDownPage = (GameObject)Instantiate(Resources.Load("Prefabs/OtherModule/CntDownListPage"));
            CntDownPage.name = "CntDownPage";
            CntDownPage.AddComponent<CntDownController>();
            StartCoroutine(BasicInformation.OtherInterface.IF13_1(BasicInformation.CurUser.Id));
        }
        else
            ShowLoginPage();
    }

    /// <summary>
    /// 跳转待办清单界面
    /// 跳转待办清单界面
    /// </summary>
    private void OnTodoListClicked()
    {
        if (BasicInformation.CurUser.IsOnline)
        {
            GameObject TodoListPage = (GameObject)Instantiate(Resources.Load("Prefabs/OtherModule/TodoListPage"));
            TodoListPage.name = "TodoListPage";
            TodoListPage.AddComponent<TodoListController>();
            StartCoroutine(BasicInformation.OtherInterface.IF05_1(BasicInformation.CurUser.Id));
        }
        else
            ShowLoginPage();
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
    /// 跳转登录界面
    /// </summary>
    private void ShowLoginPage()
    {
        GameObject LoginPage = (GameObject)Instantiate(Resources.Load("Prefabs/UserModule/LoginPage"));
        LoginPage.name = "LoginPage";
        LoginPage.AddComponent<LoginController>();
    }

    /// <summary>
    /// 跳转个人信息界面
    /// </summary>
    private void ShowPersonalInfoPage()
    {
        GameObject PersonalPage = (GameObject)Instantiate(Resources.Load("Prefabs/UserModule/PersonalPage"));
        PersonalPage.name = "PersonalPage";
        PersonalPage.AddComponent<PersonalController>();
    }

    /// <summary>
    /// 搜索星球
    /// </summary>
    private void OnSearchClicked()
    {
        if (BasicInformation.CurUser.IsOnline)
        {
            string Keyword = GameObject.Find("Pages/Page2/View/Content/SearchBar/InputField").GetComponent<InputField>().text;
            if (Keyword == "")
                ShowMsgNote("请输入星球名称/编号搜索");
            else
                StartCoroutine(BasicInformation.StarInterface.IF05(Keyword));
        }
        else
        {
            GameObject LoginPage = (GameObject)Instantiate(Resources.Load("Prefabs/UserModule/LoginPage"));
            LoginPage.name = "LoginPage";
            LoginPage.AddComponent<LoginController>();
        }
    }

    /// <summary>
    /// 界面切换操作
    /// </summary>
    private void PageChange()
    {
        if (Page == 1)
        {
            GameObject.Find("NavigatorBtm/Nav_MyStars/Icon").GetComponent<Image>().color = new Color(0, 0, 0.5f);
            GameObject.Find("NavigatorBtm/Nav_Galaxy/Icon").GetComponent<Image>().color = new Color(1, 1, 1);
            GameObject.Find("NavigatorBtm/Nav_Statistics/Icon").GetComponent<Image>().color = new Color(1, 1, 1);
        }
        else if (Page == 2)
        {
            GameObject.Find("NavigatorBtm/Nav_MyStars/Icon").GetComponent<Image>().color = new Color(1, 1, 1);
            GameObject.Find("NavigatorBtm/Nav_Galaxy/Icon").GetComponent<Image>().color = new Color(0, 0, 0.5f);
            GameObject.Find("NavigatorBtm/Nav_Statistics/Icon").GetComponent<Image>().color = new Color(1, 1, 1);
        }
        else
        {
            GameObject.Find("NavigatorBtm/Nav_MyStars/Icon").GetComponent<Image>().color = new Color(1, 1, 1);
            GameObject.Find("NavigatorBtm/Nav_Galaxy/Icon").GetComponent<Image>().color = new Color(1, 1, 1);
            GameObject.Find("NavigatorBtm/Nav_Statistics/Icon").GetComponent<Image>().color = new Color(0, 0, 0.5f);
        }
    }

    /// <summary>
    /// 下载头像
    /// </summary>
    /// <param name="Url">地址</param>
    private IEnumerator DownloadHeadImage(string Url)
    {
        UnityWebRequest request = new UnityWebRequest(Url);
        DownloadHandlerTexture DownloadTex = new DownloadHandlerTexture(true);
        request.downloadHandler = DownloadTex;
        yield return request.SendWebRequest();
        if (request.isHttpError || request.isNetworkError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            Texture2D tex = DownloadTex.texture;
            GameObject.Find("Page3/View/Content/HeadShot/HeadShotMask/HeadShot").GetComponent<Image>().sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        }
    }
}