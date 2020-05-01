using agora_gaming_rtc;
using DG.Tweening;
using System;
using Leguar.TotalJSON;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

public class StarStudyController2 : MonoBehaviour
{
    int StarId, Index = 0;

    bool IsAnchor, Camera = true, newmessage = false, anchorleft = false, camalterd = false, anchorcamstate = true, IsStudy = true, IsLateFlag = true;

    float ContentHeight = 0f, ItemPos = 0f, TimeSpent = 0f;

    BroadMessage message;

    DateTime StartTime;

    Text Record;

    static AgoraController2 app;

    ClientWebSocket WS = new ClientWebSocket();

    RectTransform Content;

    CancellationToken CT = new CancellationToken();

    /// <summary>
    /// 脚本加载
    /// </summary>
    private void Awake()
    {
        Content = GameObject.Find("StarStudyPage/Messages/View/Content").GetComponent<RectTransform>();

        GameObject StarStudyPage = GameObject.Find("StarStudyPage");
        StarStudyPage.transform.SetParent(GameObject.Find("Canvas").transform);
        StarStudyPage.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenHeight);
        StarStudyPage.GetComponent<RectTransform>().localPosition = new Vector2(BasicInformation.ScreenWidth, 0);
        StarStudyPage.transform.DOMoveX(BasicInformation.ScreenWidth / 2, 0.1f);
        Record = GameObject.Find("StarStudyPage/Timer").GetComponent<Text>();

        if (BasicInformation.ScreenWidth * 1.8f > BasicInformation.ScreenHeight)
            GameObject.Find("StarStudyPage/Head").GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenWidth * 1.8f);
        else
            GameObject.Find("StarStudyPage/Head").GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenHeight / 1.8f, BasicInformation.ScreenHeight);

        GameObject.Find("StarStudyPage/End").GetComponent<Button>().onClick.AddListener(() => { OnEndStudyClicked(); });
        GameObject.Find("StarStudyPage/SwitchCam").GetComponent<Button>().onClick.AddListener(() => { OnSwitchCamClicked(); });
        GameObject.Find("StarStudyPage/SendMsg").GetComponent<Button>().onClick.AddListener(() => { GameObject.Find("StarStudyPage/ChangeArea").transform.SetAsLastSibling(); });
        GameObject.Find("StarStudyPage/ChangeArea/Back").GetComponent<Button>().onClick.AddListener(() =>
        {
            GameObject.Find("StarStudyPage/ChangeArea/Input").GetComponent<InputField>().text = "";
            GameObject.Find("StarStudyPage/ChangeArea").transform.SetAsFirstSibling();
        });
        GameObject.Find("StarStudyPage/ChangeArea/Submit").GetComponent<Button>().onClick.AddListener(() => { OnSendMessgaeClicked(); });

        message = new BroadMessage(BasicInformation.CurUser.Name, "进入直播星球", BasicInformation.CurUser.HeadShot, BasicInformation.CurUser.Id);
        AddNewMsg();

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
            Record.text = IsAnchor ? string.Format("已直播{0:D2}:{1:D2}:{2:D2}", hour, minute, second) : string.Format("已学习{0:D2}:{1:D2}:{2:D2}", hour, minute, second);
        else
            Record.text = IsAnchor ? string.Format("已直播{0:D2}:{1:D2}", minute, second) : string.Format("已学习{0:D2}:{1:D2}", minute, second);

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

        if (anchorleft)
        {
            DisconnectSocket(3);
            anchorleft = false;
        }

        if (newmessage)
        {
            AddNewMsg();
            newmessage = false;
        }

        if (camalterd)
        {
            VideoSurface videoSurface = GameObject.Find("StarStudyPage/Head").GetComponent<VideoSurface>();
            if (videoSurface != null)
            {
                videoSurface.EnableFilpTextureApply(anchorcamstate, true);
                camalterd = false;
            }
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
    }

    /// <summary>
    /// 接收用户角色
    /// </summary>
    public void GetRole(bool IsAnchor)
    {
        this.IsAnchor = IsAnchor;
        if (IsAnchor)
        {
            GameObject.Find("StarStudyPage/End/Text").GetComponent<Text>().text = "结束直播";
            VideoSurface videoSurface = GameObject.Find("StarStudyPage/Head").AddComponent<VideoSurface>();
            videoSurface.SetGameFps(60);
            videoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
            videoSurface.EnableFilpTextureApply(true, true);
        }
        else
        {
            Destroy(GameObject.Find("StarStudyPage/SwitchCam"));
            GameObject.Find("StarStudyPage/End/Text").GetComponent<Text>().text = "结束学习";
        }
        ConnectSocket();
        app = new AgoraController2();
        app.createEngine(BasicInformation.AgoraId);
        app.joinChannel("Planet" + StarId, (uint)BasicInformation.CurUser.Id, IsAnchor);
    }

    /// <summary>
    /// 接收加入成功
    /// </summary>
    public void GetMeJoinSuccess()
    {
        GameObject.Find("StarStudyPage/Head").GetComponent<RawImage>().color = Color.white;
    }

    /// <summary>
    /// 接收用户加入id
    /// </summary>
    public void GetUserJoined(uint id)
    {
        if (!IsAnchor)
        {
            VideoSurface videoSurface = GameObject.Find("StarStudyPage/Head").AddComponent<VideoSurface>();
            videoSurface.SetGameFps(60);
            videoSurface.SetForUser(id);
            videoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
            videoSurface.EnableFilpTextureApply(true, true);
            videoSurface.SetEnable(true);
        }
    }

    /// <summary>
    /// 连接套接字，监测网络状态
    /// </summary>
    private async void ConnectSocket()
    {
        Uri url;
        if (IsAnchor)
            url = new Uri("ws://106.13.41.151:8088/broadcastServer/" +
                BasicInformation.CurUser.Id + "/" +
                StarId + "/" +
                BasicInformation.CurUser.Name + "/" +
                BasicInformation.CurUser.HeadShot.Substring(32, BasicInformation.CurUser.HeadShot.Length - 32) + "/" +
                1
                );
        else
            url = new Uri("ws://106.13.41.151:8088/broadcastServer/" +
                BasicInformation.CurUser.Id + "/" +
                StarId + "/" +
                BasicInformation.CurUser.Name + "/" +
                BasicInformation.CurUser.HeadShot.Substring(32, BasicInformation.CurUser.HeadShot.Length - 32) + "/" +
                0
                );
        Debug.Log(url.AbsoluteUri);
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
                        message = new BroadMessage(data.GetString("username"), "进入直播星球", data.GetString("headshot"), data.GetInt("userid"));
                        newmessage = true;
                    }
                    else if (data.GetInt("id") == 2)
                    {
                        message = new BroadMessage(data.GetString("username"), data.GetString("msg"), data.GetString("headshot"), data.GetInt("userid"));
                        newmessage = true;
                    }
                    else if (data.GetInt("id") == 3)
                        anchorleft = true;
                    else if (data.GetInt("id") == 4)
                    {
                        anchorcamstate = data.GetBool("camstate");
                        camalterd = true;
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
            if (IsAnchor)
                await WS.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes("{id:-1}")), WebSocketMessageType.Binary, true, CT);
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
                else if(state == 2)
                    GameObject.Find("EventSystem").SendMessage("GetRefreshRecent", 4);
                //主播离开
                else
                    GameObject.Find("EventSystem").SendMessage("GetRefreshRecent", 5);

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
        GameObject.Find("Warning/Info/Text").GetComponent<Text>().text = "即将退出直播，是否确认？";
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
    private async void OnSwitchCamClicked()
    {
        app.switchCamera();
        if (Camera)
        {
            anchorcamstate = false;
            camalterd = true;
            JSON msg = new JSON();
            msg.Add("id", 4);
            msg.Add("camstate", false);
            await WS.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(msg.CreateString())), WebSocketMessageType.Binary, true, CT);
            GameObject.Find("StarStudyPage/SwitchCam").GetComponent<Image>().color = new Color(0, 0, 0.5f);
        }
        else
        {
            anchorcamstate = true;
            camalterd = true;
            JSON msg = new JSON();
            msg.Add("id", 4);
            msg.Add("camstate", true);
            await WS.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(msg.CreateString())), WebSocketMessageType.Binary, true, CT);
            GameObject.Find("StarStudyPage/SwitchCam").GetComponent<Image>().color = Color.white;
        }
        Camera = !Camera;
    }

    /// <summary>
    /// 确认发送信息
    /// </summary>
    private async void OnSendMessgaeClicked()
    {
        Text note = GameObject.Find("StarStudyPage/ChangeArea/Note").GetComponent<Text>();
        string content = GameObject.Find("StarStudyPage/ChangeArea/Input").GetComponent<InputField>().text;
        if (content == "")
            note.text = "发送内容不能为空";
        else if (content.Length > 35)
            note.text = "发送内容不能超过35位";
        else
        {
            JSON msg = new JSON();
            msg.Add("id", 2);
            msg.Add("userid", BasicInformation.CurUser.Id);
            msg.Add("username", BasicInformation.CurUser.Name);
            msg.Add("headshot", BasicInformation.CurUser.HeadShot.Substring(32, BasicInformation.CurUser.HeadShot.Length - 32));
            msg.Add("msg", content);
            note.text = "";
            await WS.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(msg.CreateString())), WebSocketMessageType.Binary, true, CT);

            message = new BroadMessage(BasicInformation.CurUser.Name, content, BasicInformation.CurUser.HeadShot, BasicInformation.CurUser.Id);
            AddNewMsg();

            GameObject.Find("StarStudyPage/ChangeArea/Input").GetComponent<InputField>().text = "";
            GameObject.Find("StarStudyPage/ChangeArea").transform.SetAsFirstSibling();
        }
    }

    /// <summary>
    /// 添加新信息
    /// </summary>
    /// <param name="name">发送者</param>
    /// <param name="content">内容</param>
    /// <param name="headshot">头像</param>
    /// <param name="userid">用户id</param>
    private void AddNewMsg()
    {
        ContentHeight += 200;
        Content.sizeDelta = new Vector2(600f, ContentHeight);
        Content.DOAnchorPos(new Vector2(0f, ContentHeight > 600f ? ContentHeight - 600f : 0f), 0.1f);
        GameObject MsgCube = (GameObject)Instantiate(Resources.Load("Prefabs/StarModule/BroadMsgCube"));
        MsgCube.name = "MsgCube" + Index;
        MsgCube.transform.SetParent(GameObject.Find("StarStudyPage/Messages/View/Content").transform);
        MsgCube.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, ItemPos);
        MsgCube.GetComponent<RectTransform>().sizeDelta = new Vector2(600f, 200f);
        GameObject.Find("StarStudyPage/Messages/View/Content/" + MsgCube.name + "/Msg").GetComponent<Text>().text = message.content;
        GameObject.Find("StarStudyPage/Messages/View/Content/" + MsgCube.name + "/Name").GetComponent<Text>().text = message.name;
        StartCoroutine(DownloadHeadImage(message.headshot, Index, message.userid));
        ItemPos = ItemPos - 200;
        Index++;
    }

    /// <summary>
    /// 点击用户查看信息
    /// </summary>
    private void OnUserClicked(int id, Sprite head)
    {
        GameObject UserInfoPage = (GameObject)Instantiate(Resources.Load("Prefabs/UserModule/UserInfoPage"));
        UserInfoPage.name = "UserInfoPage";
        UserInfoPage.AddComponent<UserInfoController>();
        UserInfoPage.SendMessage("GetUserId", id);
        UserInfoPage.SendMessage("GetUserSprite", head);
    }

    /// <summary>
    /// 下载图片
    /// </summary>
    /// <param name="Url">图片地址</param>
    private IEnumerator DownloadHeadImage(string Url, int id, int UserId)
    {
        UnityWebRequest request = new UnityWebRequest(Url);
        DownloadHandlerTexture DownloadTex = new DownloadHandlerTexture(true);
        request.downloadHandler = DownloadTex;
        yield return request.SendWebRequest();
        if (request.isHttpError || request.isNetworkError)
            Debug.LogError(request.error);
        else
        {
            Texture2D tex = DownloadTex.texture;
            Sprite head = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            GameObject.Find("StarStudyPage/Messages/View/Content/MsgCube" + id + "/HeadShotMask/HeadShot").GetComponent<Image>().sprite = head;
            GameObject.Find("StarStudyPage/Messages/View/Content/MsgCube" + id + "/HeadShotMask").GetComponent<Button>().onClick.AddListener(() => { OnUserClicked(UserId, head); });
        }
    }
}