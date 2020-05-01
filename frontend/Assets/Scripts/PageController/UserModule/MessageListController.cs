using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MessageListController : MonoBehaviour
{
    private float ContentHeight = 25f, ItemPos = -25f;

    private RectTransform Content;

    /// <summary>
    /// 脚本加载
    /// </summary>
    private void Awake()
    {
        Content=GameObject.Find("MessageListPage/Scroll/View/Content").GetComponent<RectTransform>();
        GameObject MessageListPage = GameObject.Find("MessageListPage");
        MessageListPage.transform.SetParent(GameObject.Find("Canvas").transform);
        MessageListPage.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenHeight);
        MessageListPage.GetComponent<RectTransform>().localPosition = new Vector2(BasicInformation.ScreenWidth, 0);
        MessageListPage.transform.DOMoveX(BasicInformation.ScreenWidth / 2, 0.1f);
        GameObject.Find("MessageListPage/Return").GetComponent<Button>().onClick.AddListener(() => { OnReturnClicked(); });
    }

    /// <summary>
    /// 接收消息
    /// </summary>
    /// <param name="messages"></param>
    public void GetMessages(List<Message> messages)
    {
        GameObject.Find("MessageListPage/Scroll/Warn").GetComponent<CanvasGroup>().alpha = 0;
        GameObject.Find("MessageListPage/Clear").GetComponent<CanvasGroup>().alpha = 1;
        GameObject.Find("MessageListPage/Clear").GetComponent<Button>().onClick.AddListener(() => { OnClearClicked(); });
        ContentHeight += 525f * messages.Count;
        Content.sizeDelta = new Vector2(BasicInformation.ScreenWidth, ContentHeight);
        GameObject MessageCube;
        for (int i = 0; i < messages.Count; i++)
        {
            MessageCube = (GameObject)Instantiate(Resources.Load("Prefabs/UserModule/MessageCube"));
            MessageCube.name = "MessageCube" + i;
            MessageCube.transform.SetParent(GameObject.Find("MessageListPage/Scroll/View/Content").transform);
            MessageCube.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, ItemPos);
            MessageCube.GetComponent<RectTransform>().sizeDelta = new Vector2(0.96f * BasicInformation.ScreenWidth, 500f);
            GameObject.Find("MessageListPage/Scroll/View/Content/" + MessageCube.name + "/Content/Text").GetComponent<Text>().text = messages[i].Content;
            GameObject.Find("MessageListPage/Scroll/View/Content/" + MessageCube.name + "/Time").GetComponent<Text>().text = messages[i].SendTime;
            int id = messages[i].SenderId;
            string headshot = messages[i].SenderHeadShot;
            StartCoroutine(DownloadHeadImage(headshot, i, messages[i].SenderId));
            ItemPos = ItemPos - 525f;
        }
    }

    /// <summary>
    /// 接收清空消息
    /// </summary>
    public void GetClear()
    {
        GameObject.Find("MessageListPage/Scroll/Warn").GetComponent<CanvasGroup>().alpha = 1;
        GameObject.Find("MessageListPage/Clear").GetComponent<CanvasGroup>().alpha = 0;
        Transform list = GameObject.Find("MessageListPage/Scroll/View/Content").transform;
        for (int i = 0; i < list.childCount; i++)
            Destroy(list.GetChild(i).gameObject);
    }

    /// <summary>
    /// 返回
    /// </summary>
    private void OnReturnClicked()
    {
        GameObject x = GameObject.Find("MessageListPage");
        x.transform.DOMoveX(3 * BasicInformation.ScreenWidth / 2, 0.1f).OnComplete(() => { Destroy(x); });
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
    /// 清空留言列表
    /// </summary>
    private void OnClearClicked()
    {
        GameObject Warning = (GameObject)Instantiate(Resources.Load("Prefabs/Warning"));
        Warning.name = "Warning";
        Warning.transform.SetParent(GameObject.Find("Canvas").transform);
        Warning.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenHeight);
        Warning.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
        GameObject.Find("Warning/Info/Text").GetComponent<Text>().text = "即将清空留言列表，是否确认？";
        GameObject.Find("Warning/Info/Submit/Text").GetComponent<Text>().text = "确定";
        GameObject.Find("Warning/Info/Submit").GetComponent<Button>().onClick.AddListener(() =>
        {
            Destroy(Warning);
            StartCoroutine(BasicInformation.UserInterface.IF11(BasicInformation.CurUser.Id));
        });
        GameObject.Find("Warning/Info/Cancel").GetComponent<Button>().onClick.AddListener(() =>
        {
            Destroy(Warning);
        });
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
        {
            Debug.LogError(request.error);
        }
        else
        {
            Texture2D tex = DownloadTex.texture;
            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            GameObject.Find("MessageListPage/Scroll/View/Content/MessageCube" + id + "/HeadShotMask/HeadShot").GetComponent<Image>().sprite = sprite;
            GameObject.Find("MessageListPage/Scroll/View/Content/MessageCube" + id + "/HeadShotMask").GetComponent<Button>().onClick.AddListener(() => { OnUserClicked(UserId, sprite); });
        }
    }
}