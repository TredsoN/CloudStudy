using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UserInfoController : MonoBehaviour
{
    int UserID;

    /// <summary>
    /// 脚本加载
    /// </summary>
    private void Awake()
    {
        GameObject UserInfoPage = GameObject.Find("UserInfoPage");
        UserInfoPage.transform.SetParent(GameObject.Find("Canvas").transform);
        UserInfoPage.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenHeight);
        UserInfoPage.GetComponent<RectTransform>().localPosition = new Vector2(BasicInformation.ScreenWidth, 0);
        UserInfoPage.transform.DOMoveX(BasicInformation.ScreenWidth / 2, 0.1f);
        GameObject.Find("UserInfoPage/BigImage/Head").GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenWidth);
        GameObject.Find("UserInfoPage/Return").GetComponent<Button>().onClick.AddListener(()=> { OnReturnClicked(); });
        GameObject.Find("UserInfoPage/Message").GetComponent<Button>().onClick.AddListener(() => { OnSendMessageClicked(); });
        GameObject.Find("UserInfoPage/HeadShotMask").GetComponent<Button>().onClick.AddListener(() => { OnShowHeadClicked(); });
        GameObject.Find("UserInfoPage/BigImage").GetComponent<Button>().onClick.AddListener(() => { OnHideHeadClicked(); }); 
    }

    /// <summary>
    /// 接收用户id
    /// </summary>
    /// <param name="id"></param>
    public void GetUserId(int id)
    {
        if (BasicInformation.CurUser.Id == id)
            Destroy(GameObject.Find("UserInfoPage/Message"));
        StartCoroutine(BasicInformation.UserInterface.IF04_1(id));
        UserID = id;
    }

    /// <summary>
    /// 接收用户信息
    /// </summary>
    /// <param name="user"></param>
    public void GetUserInfo(User user)
    {
        GameObject.Find("UserInfoPage/Name").GetComponent<Text>().text = user.Name;
        GameObject.Find("UserInfoPage/InfoArea/Gender/Text").GetComponent<Text>().text = user.Gender == 0 ? "男" : "女";
        GameObject.Find("UserInfoPage/InfoArea/Age/Text").GetComponent<Text>().text = user.Age.ToString();
        GameObject.Find("UserInfoPage/InfoArea/Time/Text").GetComponent<Text>().text = user.RegistTime;
        GameObject.Find("UserInfoPage/InfoArea/StudyTime/Text").GetComponent<Text>().text = user.TotLearnTime >= 60 ? user.TotLearnTime / 60 + "h" + user.TotLearnTime % 60 + "min" : user.TotLearnTime + "min";
        GameObject.Find("UserInfoPage/InfoArea/Introduce/Text").GetComponent<Text>().text = user.Introduce;
    }

    /// <summary>
    /// 接收用户头像
    /// </summary>
    /// <param name="head"></param>
    public void GetUserSprite(Sprite head)
    {
        GameObject.Find("UserInfoPage/HeadShotMask/HeadShot").GetComponent<Image>().sprite = head;
        GameObject.Find("UserInfoPage/BigImage/Head").GetComponent<Image>().sprite = head;
    }

    /// <summary>
    /// 返回
    /// </summary>
    private void OnReturnClicked()
    {
        GameObject x = GameObject.Find("UserInfoPage");
        x.transform.DOMoveX(3 * BasicInformation.ScreenWidth / 2, 0.1f).OnComplete(() => { Destroy(x); });
    }

    /// <summary>
    /// 退出发送界面
    /// </summary>
    private void OnReturnMessageClicked()
    {
        Destroy(GameObject.Find("SendMessagePage~"));
    }

    /// <summary>
    /// 查看大头像
    /// </summary>
    private void OnShowHeadClicked()
    {
        GameObject.Find("UserInfoPage/BigImage").GetComponent<CanvasGroup>().DOFade(1f, 0.1f);
        GameObject.Find("UserInfoPage/BigImage").transform.SetAsLastSibling();
    }

    /// <summary>
    /// 关闭大头像
    /// </summary>
    private void OnHideHeadClicked()
    {
        GameObject.Find("UserInfoPage/BigImage").GetComponent<CanvasGroup>().DOFade(0f, 0.1f);
        GameObject.Find("UserInfoPage/BigImage").transform.SetAsFirstSibling();
    }

    /// <summary>
    /// 发送信息
    /// </summary>
    private void OnSendMessageClicked()
    {
        GameObject SendMessagePage = (GameObject)Instantiate(Resources.Load("Prefabs/UserModule/SendMessagePage"));
        SendMessagePage.name = "SendMessagePage~";
        SendMessagePage.transform.SetParent(GameObject.Find("Canvas").transform);
        SendMessagePage.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenHeight);
        SendMessagePage.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
        GameObject.Find("SendMessagePage~/InfoArea/Back").GetComponent<Button>().onClick.AddListener(() => { OnReturnMessageClicked(); });
        GameObject.Find("SendMessagePage~/InfoArea/Submit").GetComponent<Button>().onClick.AddListener(() => { OnSendSubmitClicked(); });
    }

    /// <summary>
    /// 确认发送信息
    /// </summary>
    private void OnSendSubmitClicked()
    {
        Text Note = GameObject.Find("SendMessagePage~/InfoArea/Note").GetComponent<Text>();
        string Message = GameObject.Find("SendMessagePage~/InfoArea/Input").GetComponent<InputField>().text;
        if (Message == "")
            Note.text = "消息不能为空";
        else if(Message.Length>=50)
            Note.text = "消息内容不能超过50个字哦";
        else
        {
            Note.text = "";
            StartCoroutine(BasicInformation.UserInterface.IF09(BasicInformation.CurUser.Id, UserID, Message));
        }
    }
}
