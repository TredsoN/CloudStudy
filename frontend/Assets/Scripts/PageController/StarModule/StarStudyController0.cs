using DG.Tweening;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class StarStudyController0 : MonoBehaviour
{
    int StarId;

    bool IsStudy = true, IsLateFlag = true;

    float TimeSpent = 0f;

    Text Record;

    DateTime StartTime;

    ClientWebSocket WS = new ClientWebSocket();

    CancellationToken CT = new CancellationToken();

    /// <summary>
    /// 脚本加载
    /// </summary>
    private void Awake()
    {
        GameObject StarStudyPage = GameObject.Find("StarStudyPage");
        StarStudyPage.transform.SetParent(GameObject.Find("Canvas").transform);
        StarStudyPage.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenHeight);
        StarStudyPage.GetComponent<RectTransform>().localPosition = new Vector2(BasicInformation.ScreenWidth, 0);
        StarStudyPage.transform.DOMoveX(BasicInformation.ScreenWidth / 2, 0.1f);
        Record = GameObject.Find("StarStudyPage/Timer").GetComponent<Text>();
        GameObject.Find("StarStudyPage/End").GetComponent<Button>().onClick.AddListener(() => { OnEndStudyClicked(); });
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    /// <summary>
    /// 脚本每帧更新
    /// </summary>
    private void Update()
    {
        int hour = (int)TimeSpent / 3600;
        int minute = (int)(TimeSpent - hour * 3600) / 60;
        int second = (int)TimeSpent - hour * 3600 - minute * 60;
        if (hour > 0)
            Record.text = string.Format("{0:D2}:{1:D2}:{2:D2}", hour, minute, second);
        else
            Record.text = string.Format("{0:D2}:{1:D2}", minute, second);

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            GameObject x = GameObject.Find("StarStudyPage");
            x.transform.DOMoveX(3 * BasicInformation.ScreenWidth / 2, 0.1f).OnComplete(() =>
            {
                GameObject Warning = (GameObject)Instantiate(Resources.Load("Prefabs/Warning"));
                Warning.name = "Warning";
                Warning.transform.SetParent(GameObject.Find("Canvas").transform);
                Warning.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenHeight);
                Warning.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
                GameObject.Find("Warning/Info/Text").GetComponent<Text>().text = "网络已断开，为保证学习成功记录，请重新开始学习";
                GameObject.Find("Warning/Info/Submit/Text").GetComponent<Text>().text = "确定";
                GameObject.Find("Warning/Info/Submit").GetComponent<Button>().onClick.AddListener(() => { Destroy(Warning); });
                GameObject.Find("Warning/Info/Cancel").GetComponent<Button>().onClick.AddListener(() => { Destroy(Warning); });
                Destroy(x);
            });
        }

        if (DateTime.Now.Hour == 23 && DateTime.Now.Minute == 59 && IsLateFlag)
        {
            IsLateFlag = false;
            DisconnectSocket(3);
        }
    }

    /// <summary>
    /// 脚本暂停
    /// </summary>
    private void OnApplicationPause(bool pause)
    {
        if (pause)
            DisconnectSocket(1);
    }

    /// <summary>
    /// 接收星球id
    /// </summary>
    /// <param name="planetid"></param>
    public void GetStarId(int planetid)
    {
        StarId = planetid;
        ConnectSocket();
    }

    /// <summary>
    /// 接收退出学习
    /// </summary>
    public void GetEndCall()
    {
        OnEndStudyClicked();
    }

    /// <summary>
    /// 连接套接字，监测网络状态
    /// </summary>
    private async void ConnectSocket()
    {
        Uri url = new Uri("ws://106.13.41.151:8088/websocket/" + BasicInformation.CurUser.Id + "/" + StarId + "");
        await WS.ConnectAsync(url, CT);
        StartTime = DateTime.Now;
        Thread ConnectionTest = new Thread(new ThreadStart(async () =>
        {
            while (WS.State == WebSocketState.Open)
            {
                Thread.Sleep(5000);
                await WS.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes("hello")), WebSocketMessageType.Binary, true, CT);
            }
        }));
        Thread Counter = new Thread(new ThreadStart(() =>
        {
            while (IsStudy)
            {
                TimeSpent = (float)(DateTime.Now - StartTime).TotalSeconds;
            }
        }));
        ConnectionTest.Start();
        Counter.Start();
        ConnectionTest.IsBackground = true;
        Counter.IsBackground = true;
    }

    /// <summary>
    /// 断开套接字
    /// </summary>
    private async void DisconnectSocket(int state)
    {
        GameObject.Find("LoadPage").transform.SetAsLastSibling();
        try
        {
            await WS.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CT);
        }
        catch { }
        finally
        {
            IsStudy = false;
            GameObject.Find("LoadPage").transform.SetAsFirstSibling();
            GameObject x = GameObject.Find("StarStudyPage");
            x.transform.DOMoveX(3 * BasicInformation.ScreenWidth / 2, 0.1f).OnComplete(() =>
            {
                //正常退出
                if (state == 0)
                    GameObject.Find("EventSystem").SendMessage("GetRefreshRecent", 2);
                //中途离开
                else if(state == 1)
                    GameObject.Find("EventSystem").SendMessage("GetRefreshRecent", 3);
                //时间太晚
                else
                    GameObject.Find("EventSystem").SendMessage("GetRefreshRecent", 4);

                Screen.sleepTimeout = SleepTimeout.SystemSetting;

                Destroy(x);
            });
        }
    }

    /// <summary>
    /// 退出学习状态
    /// </summary>
    private void OnEndStudyClicked()
    {
        GameObject Warning = (GameObject)Instantiate(Resources.Load("Prefabs/Warning"));
        Warning.name = "Warning";
        Warning.transform.SetParent(GameObject.Find("Canvas").transform);
        Warning.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenHeight);
        Warning.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
        GameObject.Find("Warning/Info/Text").GetComponent<Text>().text = "即将退出学习，是否确认？";
        GameObject.Find("Warning/Info/Submit").GetComponent<Button>().onClick.AddListener(() =>
        {
            Destroy(Warning);
            DisconnectSocket(0);
        });
        GameObject.Find("Warning/Info/Cancel").GetComponent<Button>().onClick.AddListener(() =>
        {
            Destroy(Warning);
        });
    }
}