using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.Android;

public class StarInfoController : MonoBehaviour
{
    string StarCreateTime;

    int StarType, UserType, StarId;

    float ContentHeight = 20f, ItemPos = -20f;

    RectTransform Content;

    Star star;

    /// <summary>
    /// 脚本加载
    /// </summary>
    private void Awake()
    {
        Content = GameObject.Find("StarInfoPage/Scroll/View/Content").GetComponent<RectTransform>();
        GameObject StarInfoPage = GameObject.Find("StarInfoPage");
        StarInfoPage.transform.SetParent(GameObject.Find("Canvas").transform);
        StarInfoPage.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenHeight);
        StarInfoPage.GetComponent<RectTransform>().localPosition = new Vector2(BasicInformation.ScreenWidth, 0);
        StarInfoPage.transform.DOMoveX(BasicInformation.ScreenWidth / 2, 0.1f);
        GameObject.Find("StarInfoPage/Return").GetComponent<Button>().onClick.AddListener(() => { OnReturnClicked(); });
    }

    /// <summary>
    /// 接收星球信息
    /// </summary>
    public void GetMsg(Star star)
    {
        StarCreateTime = star.CreateTime;
        GameObject.Find("StarInfoPage/Name").GetComponent<Text>().text = star.Name;
        StarType = star.Type;
        StarId = star.Id;
        StartCoroutine(BasicInformation.StarInterface.IF07(star.Id, BasicInformation.CurUser.Id));
        this.star = star;
        GameObject.Find("StarInfoPage/Info").GetComponent<Button>().onClick.AddListener(() => { OnInfoClicked(); });
        GameObject.Find("StarInfoPage/History").GetComponent<Button>().onClick.AddListener(() => { OnHistoryClicked(); });
    }

    /// <summary>
    /// 接收用户id
    /// </summary>
    public void GetUsers(List<User> userlist)
    {
        ContentHeight += 225f * userlist.Count;
        Content.sizeDelta = new Vector2(BasicInformation.ScreenWidth, ContentHeight);
        GameObject UserCube;
        for (int i = 0; i < userlist.Count; i++)
        {
            UserCube = (GameObject)Instantiate(Resources.Load("Prefabs/StarModule/StarUserCube"));
            UserCube.name = "StarUserCube" + i;
            if (i != 0)
                Destroy(GameObject.Find("StarUserCube" + i + "/Tag"));
            UserCube.transform.SetParent(GameObject.Find("StarInfoPage/Scroll/View/Content").transform);
            UserCube.GetComponent<RectTransform>().sizeDelta = new Vector2(0.97f * BasicInformation.ScreenWidth, 200f);
            UserCube.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, ItemPos);
            GameObject.Find("StarInfoPage/Scroll/View/Content/" + UserCube.name + "/UserName").GetComponent<Text>().text = userlist[i].Name;
            GameObject.Find("StarInfoPage/Scroll/View/Content/" + UserCube.name + "/Status").GetComponent<Text>().text = userlist[i].Status;
            StartCoroutine(DownloadHeadImage(userlist[i].HeadShot, i, userlist[i].Id));
            ItemPos = ItemPos - 225;
        }
        GameObject.Find("StarInfoPage/Creator").GetComponent<Text>().text = "由 ·" + userlist[0].Name + "· 创建于 ·" + StarCreateTime + "·";
    }

    /// <summary>
    /// 接收用户和星球的关系
    /// </summary>
    public void GetUserPlanetRelation(int id)
    {
        UserType = id;
        if (id == 0)
        {
            GameObject.Find("StarInfoPage/Start/Text").GetComponent<Text>().text = "加入星球";
            GameObject.Find("StarInfoPage/Start").GetComponent<Button>().onClick.AddListener(() => { OnJoinClicked(); });
        }
        else if(id ==2 && StarType==2)
        {
            GameObject.Find("StarInfoPage/Start/Text").GetComponent<Text>().text = "开始直播";
            GameObject.Find("StarInfoPage/Share").GetComponent<CanvasGroup>().alpha = 1;
            GameObject.Find("StarInfoPage/Start").GetComponent<Button>().onClick.AddListener(() => { OnStartClicked(); });
            GameObject.Find("StarInfoPage/Share").GetComponent<Button>().onClick.AddListener(() => { OnShareClicked(); });
        }
        else
        {
            GameObject.Find("StarInfoPage/Start/Text").GetComponent<Text>().text = "开始学习";
            GameObject.Find("StarInfoPage/Share").GetComponent<CanvasGroup>().alpha = 1;
            GameObject.Find("StarInfoPage/Start").GetComponent<Button>().onClick.AddListener(() => { OnStartClicked(); });
            GameObject.Find("StarInfoPage/Share").GetComponent<Button>().onClick.AddListener(() => { OnShareClicked(); });
        }
    }

    /// <summary>
    /// 接收加入星球
    /// </summary>
    public void GetJoinPlanet()
    {
        UserType = 1;
        GameObject.Find("StarInfoPage/Share").GetComponent<CanvasGroup>().alpha = 1;
        GameObject.Find("StarInfoPage/Start/Text").GetComponent<Text>().text = "开始学习";
        GameObject.Find("StarInfoPage/Start").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("StarInfoPage/Start").GetComponent<Button>().onClick.AddListener(() => { OnStartClicked(); });
        ContentHeight += 225f;
        Content.sizeDelta = new Vector2(BasicInformation.ScreenWidth, ContentHeight);
        GameObject UserCube;
        UserCube = (GameObject)Instantiate(Resources.Load("Prefabs/StarModule/StarUserCube"));
        UserCube.name = "StarUserCube999";
        Destroy(GameObject.Find("StarUserCube999/Tag"));
        UserCube.transform.SetParent(GameObject.Find("StarInfoPage/Scroll/View/Content").transform);
        UserCube.GetComponent<RectTransform>().sizeDelta = new Vector2(0.97f * BasicInformation.ScreenWidth, 200f);
        UserCube.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, ItemPos);
        GameObject.Find("StarInfoPage/Scroll/View/Content/StarUserCube999/UserName").GetComponent<Text>().text = BasicInformation.CurUser.Name;
        GameObject.Find("StarInfoPage/Scroll/View/Content/StarUserCube999/Status").GetComponent<Text>().text = "休息中";
        StartCoroutine(DownloadHeadImage(BasicInformation.CurUser.HeadShot, 999, BasicInformation.CurUser.Id));
        ItemPos = ItemPos - 225;
    }

    /// <summary>
    /// 接收星球名修改
    /// </summary>
    public void GetPlanetNameChange(string name)
    {
        GameObject.Find("StarInfoPage/Name").GetComponent<Text>().text = name;
        star.Name = name;
    }

    /// <summary>
    /// 接收星球介绍修改
    /// </summary>
    public void GetPlanetIntroChange(string intro)
    {
        star.Introduce = intro;
    }

    /// <summary>
    /// 返回
    /// </summary>
    private void OnReturnClicked()
    {
        GameObject x = GameObject.Find("StarInfoPage");
        x.transform.DOMoveX(3 * BasicInformation.ScreenWidth / 2, 0.1f).OnComplete(() => { Destroy(x); });
    }

    /// <summary>
    /// 返回
    /// </summary>
    private void OnCodePageReturnClicked()
    {
        Destroy(GameObject.Find("CodeEnterPage~"));
    }

    /// <summary>
    /// 开始学习
    /// </summary>
    private void OnStartClicked()
    {
        if (StarType == 0)
        {
            GameObject Warning = (GameObject)Instantiate(Resources.Load("Prefabs/Warning"));
            Warning.name = "Warning";
            Warning.transform.SetParent(GameObject.Find("Canvas").transform);
            Warning.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenHeight);
            Warning.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
            GameObject.Find("Warning/Info/Text").GetComponent<Text>().text = "即将进入学习状态，是否确认？";
            GameObject.Find("Warning/Info/Submit").GetComponent<Button>().onClick.AddListener(() =>
            {
                Destroy(Warning);
                StartCoroutine(BasicInformation.StarInterface.IF08_0(BasicInformation.CurUser.Id, StarId));
            });
            GameObject.Find("Warning/Info/Cancel").GetComponent<Button>().onClick.AddListener(() =>
            {
                Destroy(Warning);
            });
        }
        else if (StarType == 1)
        {
            GameObject Warning = (GameObject)Instantiate(Resources.Load("Prefabs/Warning"));
            Warning.name = "Warning";
            Warning.transform.SetParent(GameObject.Find("Canvas").transform);
            Warning.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenHeight);
            Warning.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
            GameObject.Find("Warning/Info/Text").GetComponent<Text>().text = "即将加入星球研讨，是否确认？";
            GameObject.Find("Warning/Info/Submit").GetComponent<Button>().onClick.AddListener(() =>
            {
                Destroy(Warning);
                StartCoroutine(BasicInformation.StarInterface.IF08_1(BasicInformation.CurUser.Id, StarId));
            });
            GameObject.Find("Warning/Info/Cancel").GetComponent<Button>().onClick.AddListener(() =>
            {
                Destroy(Warning);
            });
        }
        else if (UserType == 2)
        {
            GameObject Warning = (GameObject)Instantiate(Resources.Load("Prefabs/Warning"));
            Warning.name = "Warning";
            Warning.transform.SetParent(GameObject.Find("Canvas").transform);
            Warning.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenHeight);
            Warning.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
            GameObject.Find("Warning/Info/Text").GetComponent<Text>().text = "即将开始直播，准备好了吗？";
            GameObject.Find("Warning/Info/Submit").GetComponent<Button>().onClick.AddListener(() =>
            {
                Destroy(Warning);
                StartCoroutine(BasicInformation.StarInterface.IF11(BasicInformation.CurUser.Id, StarId));
            });
            GameObject.Find("Warning/Info/Cancel").GetComponent<Button>().onClick.AddListener(() =>
            {
                Destroy(Warning);
            });
        }
        else
        {
            GameObject Warning = (GameObject)Instantiate(Resources.Load("Prefabs/Warning"));
            Warning.name = "Warning";
            Warning.transform.SetParent(GameObject.Find("Canvas").transform);
            Warning.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenHeight);
            Warning.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
            GameObject.Find("Warning/Info/Text").GetComponent<Text>().text = "即将观看星球直播，是否确认？";
            GameObject.Find("Warning/Info/Submit").GetComponent<Button>().onClick.AddListener(() =>
            {
                Destroy(Warning);
                StartCoroutine(BasicInformation.StarInterface.IF12(BasicInformation.CurUser.Id, StarId));
            });
            GameObject.Find("Warning/Info/Cancel").GetComponent<Button>().onClick.AddListener(() =>
            {
                Destroy(Warning);
            });
        }
    }

    /// <summary>
    /// 加入星球
    /// </summary>
    private void OnJoinClicked()
    {
        if (StarType == 0 || StarType == 2)
        {
            StartCoroutine(BasicInformation.StarInterface.IF06(BasicInformation.CurUser.Id, StarId, 0));
        }
        else
        {
            GameObject CodeEnterPage = (GameObject)Instantiate(Resources.Load("Prefabs/StarModule/CodeEnterPage"));
            CodeEnterPage.name = "CodeEnterPage~";
            CodeEnterPage.transform.SetParent(GameObject.Find("Canvas").transform);
            CodeEnterPage.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenHeight);
            CodeEnterPage.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
            GameObject.Find("CodeEnterPage~/InfoArea/Back").GetComponent<Button>().onClick.AddListener(() => { OnCodePageReturnClicked(); });
            GameObject.Find("CodeEnterPage~/InfoArea/Submit").GetComponent<Button>().onClick.AddListener(() => { OnCodeSubmitClicked(); });
        }
    }

    /// <summary>
    /// 提交暗号
    /// </summary>
    private void OnCodeSubmitClicked()
    {
        string Code = GameObject.Find("CodeEnterPage~/InfoArea/Input").GetComponent<InputField>().text;
        if (Code.Length != 6)
            GameObject.Find("CodeEnterPage~/InfoArea/Note").GetComponent<Text>().text = "请输入6位暗号";
        else
            StartCoroutine(BasicInformation.StarInterface.IF06(BasicInformation.CurUser.Id, StarId, int.Parse(Code)));
    }

    /// <summary>
    /// 点击用户查看信息
    /// </summary>
    private void OnUserClicked(int id , Sprite head)
    {
        GameObject UserInfoPage = (GameObject)Instantiate(Resources.Load("Prefabs/UserModule/UserInfoPage"));
        UserInfoPage.name = "UserInfoPage";
        UserInfoPage.AddComponent<UserInfoController>();
        UserInfoPage.SendMessage("GetUserId", id);
        UserInfoPage.SendMessage("GetUserSprite", head);
    }

    /// <summary>
    /// 点击查看星球信息
    /// </summary>
    private void OnInfoClicked()
    {
        if (star.Type == 1)
        {
            GameObject SmallStarInfoPage = (GameObject)Instantiate(Resources.Load("Prefabs/StarModule/SmallStarInfoPage2"));
            SmallStarInfoPage.name = "SmallStarInfoPage~";
            SmallStarInfoPage.AddComponent<SmallStarInfoController>();
            SmallStarInfoPage.SendMessage("GetStar", star);
            SmallStarInfoPage.SendMessage("GetRelation", UserType);
        }
        else
        {
            GameObject SmallStarInfoPage = (GameObject)Instantiate(Resources.Load("Prefabs/StarModule/SmallStarInfoPage"));
            SmallStarInfoPage.name = "SmallStarInfoPage~";
            SmallStarInfoPage.AddComponent<SmallStarInfoController>();
            SmallStarInfoPage.SendMessage("GetStar", star);
            SmallStarInfoPage.SendMessage("GetRelation", UserType);
        }
    }

    /// <summary>
    /// 点击查看星球动态
    /// </summary>
    private void OnHistoryClicked()
    {
        GameObject StudyRecordPage = (GameObject)Instantiate(Resources.Load("Prefabs/StarModule/StarRecordPage"));
        StudyRecordPage.name = "StudyRecordPage";
        StudyRecordPage.AddComponent<StudyRecordController>();
        StudyRecordPage.SendMessage("GetStarId", star.Id);
    }

    /// <summary>
    /// 点击分享
    /// </summary>
    private void OnShareClicked()
    {
        GameObject SharePage = (GameObject)Instantiate(Resources.Load("Prefabs/StarModule/SharePage"));
        SharePage.name = "SharePage~";
        SharePage.transform.SetParent(GameObject.Find("Canvas").transform);
        SharePage.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenHeight);
        SharePage.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
        GameObject.Find("SharePage~/InfoArea/Back").GetComponent<Button>().onClick.AddListener(() => { Destroy(SharePage); });
        GameObject.Find("SharePage~/InfoArea/Submit").GetComponent<Button>().onClick.AddListener(() => {
            BlankOperationClipboard.SetValue(GameObject.Find("SharePage~/InfoArea/Input").GetComponent<InputField>().text);
            GameObject MsgNote = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/MsgNote"));
            MsgNote.name = "MsgNote";
            MsgNote.transform.SetParent(GameObject.Find("Canvas").transform);
            MsgNote.transform.localScale = new Vector3(0f, 0f, 0f);
            MsgNote.transform.localPosition = new Vector3(0f, 0f, 0f);
            MsgNote.transform.GetChild(0).GetComponent<Text>().text = "复制成功，快分享给朋友吧";
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
            Destroy(SharePage);
        });
        if (star.Type == 1)
            GameObject.Find("SharePage~/InfoArea/Input").GetComponent<InputField>().text = "【星云自习】我正在星球<" + star.Name + ">自习，快点加入我吧！\n星球id：" + star.StarId + "\n星球暗号：" + star.Password;
        else
            GameObject.Find("SharePage~/InfoArea/Input").GetComponent<InputField>().text = "【星云自习】我正在星球<" + star.Name + ">自习，快点加入我吧！\n星球id：" + star.StarId;
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
            GameObject.Find("StarInfoPage/Scroll/View/Content/StarUserCube" + id + "/HeadShotMask/HeadShot").GetComponent<Image>().sprite = head;
            GameObject.Find("StarInfoPage/Scroll/View/Content/StarUserCube" + id).GetComponent<Button>().onClick.AddListener(() => { OnUserClicked(UserId, head); });
        }
    }
}