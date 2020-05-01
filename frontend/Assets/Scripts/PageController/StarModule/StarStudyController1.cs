using agora_gaming_rtc;
using DG.Tweening;
using Leguar.TotalJSON;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class StarStudyController1 : MonoBehaviour
{
    int StarId;

    int[] UserIds = new int[5];

    /// <summary>
    /// 座位是否为空
    /// </summary>
    bool[] IsEmptys = new bool[5] { true, true, true, true, true };

    /// <summary>
    /// Camera摄像头状态，Speaker扬声器状态，Audio音频状态，Video视频状态
    /// </summary>
    bool Camera, Speaker, Audio, Video, IsStudy = true, Setting = false, IsLateFlag = true;

    List<JSON> caminformation = new List<JSON>();

    float TimeSpent = 0f;

    DateTime StartTime;

    Text Record;

    static AgoraController app;

    ClientWebSocket WS = new ClientWebSocket();

    CancellationToken CT = new CancellationToken();

    /// <summary>
    /// 脚本加载
    /// </summary>
    private void Awake()
    {
        Camera = Speaker = Audio = Video = true;
        GameObject StarStudyPage = GameObject.Find("StarStudyPage");
        StarStudyPage.transform.SetParent(GameObject.Find("Canvas").transform);
        StarStudyPage.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenHeight);
        StarStudyPage.GetComponent<RectTransform>().localPosition = new Vector2(BasicInformation.ScreenWidth, 0);
        StarStudyPage.transform.DOMoveX(BasicInformation.ScreenWidth / 2, 0.1f);
        Record = GameObject.Find("StarStudyPage/Timer").GetComponent<Text>();

        GameObject.Find("StarStudyPage/MyMask").GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth / 2, BasicInformation.ScreenWidth / 2);
        GameObject.Find("StarStudyPage/MyMask/Head").GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth / 2, 9 * BasicInformation.ScreenWidth / 10);
        GameObject.Find("StarStudyPage/MyMask/NameTag/Name").GetComponent<Text>().text = BasicInformation.CurUser.Name;

        GameObject.Find("StarStudyPage/End").GetComponent<Button>().onClick.AddListener(() => { OnEndStudyClicked(); });
        GameObject.Find("StarStudyPage/Setting").GetComponent<Button>().onClick.AddListener(() => { OnShowControlPannelClicked(); });

        GameObject MyMask = GameObject.Find("StarStudyPage/MyMask");
        MyMask.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (MyMask.GetComponent<RectTransform>().sizeDelta[0] == BasicInformation.ScreenWidth / 2)
            {
                MyMask.transform.SetAsLastSibling();
                GameObject.Find("StarStudyPage/ControlPanel").transform.SetAsLastSibling();
                MyMask.GetComponent<RectTransform>().DOSizeDelta(new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenWidth * 3 / 2), 0.3f);
                MyMask.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 0), 0.3f);
                MyMask.transform.GetChild(0).gameObject.GetComponent<RectTransform>().DOSizeDelta(new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenWidth * 9 / 5), 0.3f);
            }
            else
            {
                MyMask.transform.SetSiblingIndex(transform.childCount - 2);
                MyMask.GetComponent<RectTransform>().DOSizeDelta(new Vector2(BasicInformation.ScreenWidth / 2, BasicInformation.ScreenWidth / 2), 0.3f);
                MyMask.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 0), 0.3f);
                MyMask.transform.GetChild(0).gameObject.GetComponent<RectTransform>().DOSizeDelta(new Vector2(BasicInformation.ScreenWidth / 2, BasicInformation.ScreenWidth * 9 / 10), 0.3f);
            }
        });
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
            Record.text = string.Format("已学习{0:D2}:{1:D2}:{2:D2}", hour, minute, second);
        else
            Record.text = string.Format("已学习{0:D2}:{1:D2}", minute, second);

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

        foreach (JSON info in caminformation)
        {
            if (info.GetBool("applied")) continue;
            int index = GetUserPlace(info.GetInt("userId"));
            if (index == -1) continue;
            VideoSurface videoSurface = GameObject.Find("StarStudyPage/HeadMask" + index + "/Head").GetComponent<VideoSurface>();
            videoSurface.EnableFilpTextureApply(info.GetBool("camstate"), true);
            info.Replace("applied", true);
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
    /// <param name="pause"></param>
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
        app = new AgoraController();
        app.createEngine(BasicInformation.AgoraId);
        app.joinChannel("Planet" + planetid, (uint)BasicInformation.CurUser.Id);
    }

    /// <summary>
    /// 接收当前用户加入成功
    /// </summary>
    public void GetMeJoinSuccess()
    {
        GameObject.Find("StarStudyPage/MyMask/Mask").GetComponent<CanvasGroup>().alpha = 0;
        GameObject.Find("StarStudyPage/ControlPanel/SwitchCam").GetComponent<Button>().onClick.AddListener(() => { OnSwitchCamClicked(); });
        GameObject.Find("StarStudyPage/ControlPanel/SwitchSpeaker").GetComponent<Button>().onClick.AddListener(() => { OnSwitchSpeakerClicked(); });
        GameObject.Find("StarStudyPage/ControlPanel/MuteMe").GetComponent<Button>().onClick.AddListener(() => { OnMuteMeAuClicked(); });
        GameObject.Find("StarStudyPage/ControlPanel/BlockMe").GetComponent<Button>().onClick.AddListener(() => { OnMuteMeViClicked(); });
        VideoSurface videoSurface = GameObject.Find("StarStudyPage/MyMask/Head").AddComponent<VideoSurface>();
        videoSurface.SetGameFps(60);
        videoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
        videoSurface.EnableFilpTextureApply(true, true);
        videoSurface.SetEnable(true);
    }

    /// <summary>
    /// 接收用户加入id
    /// </summary>
    /// <param name="user"></param>
    public void GetUserJoined(int id)
    {
        StartCoroutine(BasicInformation.UserInterface.IF04_2(id));
    }

    /// <summary>
    /// 接收用户退出id
    /// </summary>
    /// <param name="user"></param>
    public void GetUserLeft(int id)
    {
        for (int i = 0; i < 5; i++)
        {
            if (UserIds[i] == id)
            {
                Destroy(GameObject.Find("StarStudyPage/HeadMask" + i));
                IsEmptys[i] = true;
                return;
            }
        }
    }

    /// <summary>
    /// 接收用户信息
    /// </summary>
    /// <param name="user"></param>
    public void GetUserInfo(User user)
    {
        int Index = GetAnEmptyPlace();
        UserIds[Index] = user.Id;
        IsEmptys[Index] = false;
        GameObject HeadMask = (GameObject)Instantiate(Resources.Load("Prefabs/StarModule/StarHeadMask"));
        HeadMask.name = "HeadMask" + Index;
        HeadMask.transform.SetParent(GameObject.Find("StarStudyPage").transform);
        HeadMask.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth / 2, BasicInformation.ScreenWidth / 2);
        HeadMask.GetComponent<RectTransform>().anchoredPosition = new Vector2(((Index + 1) % 2) * (BasicInformation.ScreenWidth / 2), -((Index + 1) / 2) * (BasicInformation.ScreenWidth / 2));
        HeadMask.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (HeadMask.GetComponent<RectTransform>().sizeDelta[0] == BasicInformation.ScreenWidth / 2)
            {
                HeadMask.transform.SetAsLastSibling();
                GameObject.Find("StarStudyPage/ControlPanel").transform.SetAsLastSibling();
                HeadMask.GetComponent<RectTransform>().DOSizeDelta(new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenWidth * 3 / 2), 0.3f);
                HeadMask.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 0), 0.3f);
                HeadMask.transform.GetChild(0).gameObject.GetComponent<RectTransform>().DOSizeDelta(new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenWidth * 9 / 5), 0.3f);
            }
            else
            {
                HeadMask.GetComponent<RectTransform>().DOSizeDelta(new Vector2(BasicInformation.ScreenWidth / 2, BasicInformation.ScreenWidth / 2), 0.3f);
                HeadMask.GetComponent<RectTransform>().DOAnchorPos(new Vector2(((Index + 1) % 2) * (BasicInformation.ScreenWidth / 2), -((Index + 1) / 2) * (BasicInformation.ScreenWidth / 2)), 0.3f);
                HeadMask.transform.GetChild(0).gameObject.GetComponent<RectTransform>().DOSizeDelta(new Vector2(BasicInformation.ScreenWidth / 2, BasicInformation.ScreenWidth * 9 / 10), 0.3f);
            }
        });
        GameObject.Find("StarStudyPage/HeadMask" + Index + "/Head").GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth / 2, 15 * BasicInformation.ScreenWidth / 16);
        GameObject.Find("StarStudyPage/HeadMask" + Index + "/NameTag/Name").GetComponent<Text>().text = user.Name;
        GameObject.Find("StarStudyPage/HeadMask" + Index + "/Mask").GetComponent<CanvasGroup>().alpha = 0;
        VideoSurface videoSurface = GameObject.Find("StarStudyPage/HeadMask" + Index + "/Head").AddComponent<VideoSurface>();
        videoSurface.SetGameFps(60);
        videoSurface.SetForUser((uint)user.Id);
        videoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
        videoSurface.EnableFilpTextureApply(true, true);
        videoSurface.SetEnable(true);

    }

    /// <summary>
    /// 接收用户静音
    /// </summary>
    /// <param name="id"></param>
    public void GetUserMuteAudio(int id)
    {
        for (int i = 0; i < 5; i++)
        {
            if (UserIds[i] == id)
            {
                GameObject.Find("StarStudyPage/HeadMask" + i + "/NameTag/Mic").GetComponent<Image>().sprite = Resources.Load<Sprite>("Pictures/Icons/mic-off");
                return;
            }
        }
    }

    /// <summary>
    /// 接收用户结束静音
    /// </summary>
    /// <param name="id"></param>
    public void GetUserUnMuteAudio(int id)
    {
        for (int i = 0; i < 5; i++)
        {
            if (UserIds[i] == id)
            {
                GameObject.Find("StarStudyPage/HeadMask" + i + "/NameTag/Mic").GetComponent<Image>().sprite = Resources.Load<Sprite>("Pictures/Icons/mic");
                return;
            }
        }
    }

    /// <summary>
    /// 接收用户关闭摄像头
    /// </summary>
    /// <param name="id"></param>
    public void GetUserMuteVideo(int id)
    {
        for (int i = 0; i < 5; i++)
        {
            if (UserIds[i] == id)
            {
                GameObject.Find("StarStudyPage/HeadMask" + i + "/Mask").GetComponent<CanvasGroup>().alpha = 1;
                return;
            }
        }
    }

    /// <summary>
    /// 接收用户打开摄像头
    /// </summary>
    /// <param name="id"></param>
    public void GetUserUnMuteVideo(int id)
    {
        for (int i = 0; i < 5; i++)
        {
            if (UserIds[i] == id)
            {
                GameObject.Find("StarStudyPage/HeadMask" + i + "/Mask").GetComponent<CanvasGroup>().alpha = 0;
                return;
            }
        }
    }

    /// <summary>
    /// 获取一个空位置
    /// </summary>
    /// <returns>空位id</returns>
    private int GetAnEmptyPlace()
    {
        for (int i = 0; i < 5; i++)
            if (IsEmptys[i])
                return i;
        return -1;
    }

    /// <summary>
    /// 获取用户位置
    /// </summary>
    /// <returns>空位id</returns>
    private int GetUserPlace(int userid)
    {
        for (int i = 0; i < 5; i++)
            if (UserIds[i] == userid)
                return i;
        return -1;
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
        Uri url = new Uri("ws://106.13.41.151:8088/groupStudyServer/" + BasicInformation.CurUser.Id + "/" + StarId + "");
        await WS.ConnectAsync(url, CT);
        StartTime = DateTime.Now;
        Thread ConnectionTest = new Thread(new ThreadStart(async () =>
        {
            while (WS.State == WebSocketState.Open)
            {
                Thread.Sleep(5000);
                await WS.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes("{id:0}")), WebSocketMessageType.Binary, true, CT);
            }
        }));
        Thread GetMsg = new Thread(new ThreadStart(async () =>
        {
            while (WS.State == WebSocketState.Open)
            {
                var result = new byte[1024];
                await WS.ReceiveAsync(new ArraySegment<byte>(result), CT);
                string str = Encoding.UTF8.GetString(result, 0, result.Length);
                Debug.Log(str);
                try
                {
                    JSON data = JSON.ParseString(str);
                    if (data.GetInt("id") == 1)
                    {
                        JArray infos = data.GetJArray("info");
                        List<JSON> infolist = new List<JSON>();
                        for (int i = 0; i < infos.Length; i++)
                        {
                            JSON item = JSON.ParseString(infos[i].CreateString());
                            if (item.GetInt("userId") != BasicInformation.CurUser.Id)
                            {
                                item.Add("applied", false);
                                infolist.Add(item);
                            }
                        }
                        caminformation = infolist;
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }
        }));
        Thread Counter = new Thread(new ThreadStart(() =>
        {
            while (IsStudy)
            {
                TimeSpent = (float)(DateTime.Now - StartTime).TotalSeconds;
            }
        }));
        Counter.Start();
        ConnectionTest.Start();
        GetMsg.Start();
        Counter.IsBackground = true;
        ConnectionTest.IsBackground = true;
        GetMsg.IsBackground = true;
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
            app.leaveChannel();
            app.destroyEngine();
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
                else if (state == 1)
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
    /// 打开控制面板
    /// </summary>
    private void OnShowControlPannelClicked()
    {
        if (Setting)
        {
            GameObject.Find("StarStudyPage/Setting").GetComponent<Image>().color = Color.white;
            GameObject.Find("StarStudyPage/ControlPanel").GetComponent<CanvasGroup>().alpha = 0;
            Setting = false;
        }
        else
        {
            GameObject.Find("StarStudyPage/Setting").GetComponent<Image>().color = new Color(0, 0, 0.5f);
            GameObject.Find("StarStudyPage/ControlPanel").GetComponent<CanvasGroup>().alpha = 1;
            Setting = true;
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
        GameObject.Find("Warning/Info/Text").GetComponent<Text>().text = "即将退出研讨，是否确认？";
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

    /// <summary>
    /// 切换摄像头
    /// </summary>
    public async void OnSwitchCamClicked()
    {
        app.switchCamera();
        if (Camera)
        {
            VideoSurface videoSurface = GameObject.Find("StarStudyPage/MyMask/Head").GetComponent<VideoSurface>();
            videoSurface.EnableFilpTextureApply(false, true);
            JSON msg = new JSON();
            msg.Add("id", 1);
            msg.Add("userid", BasicInformation.CurUser.Id);
            msg.Add("camstate", false);
            await WS.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(msg.CreateString())), WebSocketMessageType.Binary, true, CT);
            GameObject.Find("StarStudyPage/ControlPanel/SwitchCam").GetComponent<Image>().color = new Color(0, 0, 0.5f);
        }
        else
        {
            VideoSurface videoSurface = GameObject.Find("StarStudyPage/MyMask/Head").GetComponent<VideoSurface>();
            videoSurface.EnableFilpTextureApply(true, true);
            JSON msg = new JSON();
            msg.Add("id", 1);
            msg.Add("userid", BasicInformation.CurUser.Id);
            msg.Add("camstate", true);
            await WS.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(msg.CreateString())), WebSocketMessageType.Binary, true, CT);
            GameObject.Find("StarStudyPage/ControlPanel/SwitchCam").GetComponent<Image>().color = Color.white;
        }
        Camera = !Camera;
    }

    /// <summary>
    /// 切换扬声器
    /// </summary>
    public void OnSwitchSpeakerClicked()
    {
        app.switchSpeaker(!Speaker);
        if (Speaker)
        {
            GameObject.Find("StarStudyPage/ControlPanel/SwitchSpeaker").GetComponent<Image>().sprite = Resources.Load<Sprite>("Pictures/Icons/speaker");
            GameObject.Find("StarStudyPage/ControlPanel/SwitchSpeaker").GetComponent<Image>().color = new Color(0, 0, 0.5f);
        }
        else
        {
            GameObject.Find("StarStudyPage/ControlPanel/SwitchSpeaker").GetComponent<Image>().sprite = Resources.Load<Sprite>("Pictures/Icons/phonecall");
            GameObject.Find("StarStudyPage/ControlPanel/SwitchSpeaker").GetComponent<Image>().color = Color.white;
        }
        Speaker = !Speaker;
    }

    /// <summary>
    /// 关闭本地语音
    /// </summary>
    public void OnMuteMeAuClicked()
    {
        app.muteLocalAudio(Audio);
        if (Audio)
        {
            GameObject.Find("StarStudyPage/ControlPanel/MuteMe").GetComponent<Image>().sprite = Resources.Load<Sprite>("Pictures/Icons/mic");
            GameObject.Find("StarStudyPage/ControlPanel/MuteMe").GetComponent<Image>().color = new Color(0, 0, 0.5f);
        }
        else
        {
            GameObject.Find("StarStudyPage/ControlPanel/MuteMe").GetComponent<Image>().sprite = Resources.Load<Sprite>("Pictures/Icons/mic-off");
            GameObject.Find("StarStudyPage/ControlPanel/MuteMe").GetComponent<Image>().color = Color.white;
        }
        Audio = !Audio;
    }

    /// <summary>
    /// 关闭本地视频
    /// </summary>
    public void OnMuteMeViClicked()
    {
        app.muteLocalVideo(Video);
        if (Video)
        {
            GameObject.Find("StarStudyPage/ControlPanel/BlockMe").GetComponent<Image>().sprite = Resources.Load<Sprite>("Pictures/Icons/cam");
            GameObject.Find("StarStudyPage/ControlPanel/BlockMe").GetComponent<Image>().color = new Color(0, 0, 0.5f);
            GameObject.Find("StarStudyPage/MyMask/Mask").GetComponent<CanvasGroup>().alpha = 1;
        }
        else
        {
            GameObject.Find("StarStudyPage/ControlPanel/BlockMe").GetComponent<Image>().sprite = Resources.Load<Sprite>("Pictures/Icons/cam-off");
            GameObject.Find("StarStudyPage/ControlPanel/BlockMe").GetComponent<Image>().color = Color.white;
            GameObject.Find("StarStudyPage/MyMask/Mask").GetComponent<CanvasGroup>().alpha = 0;
        }
        Video = !Video;
    }
}