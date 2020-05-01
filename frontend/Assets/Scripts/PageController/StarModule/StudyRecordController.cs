using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class StudyRecordController : MonoBehaviour
{
    int StarId, Batch = 0, Index = 0;

    float ViewHeight, ContentHeight = 0f, ItemPos = 0f;

    RectTransform Content;

    /// <summary>
    /// 脚本加载
    /// </summary>
    private void Awake()
    {
        ViewHeight = GameObject.Find("StudyRecordPage/Scroll").GetComponent<RectTransform>().rect.height;
        Content = GameObject.Find("StudyRecordPage/Scroll/View/Content").GetComponent<RectTransform>();

        GameObject StudyRecordPage = GameObject.Find("StudyRecordPage");
        StudyRecordPage.transform.SetParent(GameObject.Find("Canvas").transform);
        StudyRecordPage.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenHeight);
        StudyRecordPage.GetComponent<RectTransform>().localPosition = new Vector2(BasicInformation.ScreenWidth, 0);
        StudyRecordPage.transform.DOMoveX(BasicInformation.ScreenWidth / 2, 0.1f);
        GameObject.Find("StudyRecordPage/Scroll").GetComponent<ScrollRect>().onValueChanged.AddListener((Vector2 x) => { OnScrollToEnd(); });
        GameObject.Find("StudyRecordPage/Return").GetComponent<Button>().onClick.AddListener(() => { OnReturnClicked(); });
    }

    public void GetStarId(int starid)
    {
        StarId = starid;
        StartCoroutine(BasicInformation.StarInterface.IF09(starid, Batch));
        Batch++;
    }

    /// <summary>
    /// 接收动态列表
    /// </summary>
    /// <param name="recordlist"></param>
    public void GetRecords(List<Record> recordlist)
    {
        if (recordlist.Count == 0)
        {
            Batch = -1;
            return;
        }
        Destroy(GameObject.Find("StudyRecordPage/Scroll/Warn"));
        if (recordlist.Count < 10)
            Batch = -1;
        AddNewRecords(recordlist);
    }

    /// <summary>
    /// 返回
    /// </summary>
    private void OnReturnClicked()
    {
        GameObject x = GameObject.Find("StudyRecordPage");
        x.transform.DOMoveX(3 * BasicInformation.ScreenWidth / 2, 0.1f).OnComplete(() => { Destroy(x); });
    }

    /// <summary>
    /// 动态添加动态
    /// </summary>
    /// <param name="recordlist"></param>
    private void AddNewRecords(List<Record> recordlist)
    {
        ContentHeight += 220f * recordlist.Count;
        Content.sizeDelta = new Vector2(BasicInformation.ScreenWidth, ContentHeight);
        for (int i = 0; i < recordlist.Count; i++)
        {
            GameObject RecordCube = (GameObject)Instantiate(Resources.Load("Prefabs/StarModule/StudyRecordCube"));
            RecordCube.name = "StudyRecordCube" + Index;
            RecordCube.transform.SetParent(GameObject.Find("StudyRecordPage/Scroll/View/Content").transform);
            RecordCube.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, ItemPos);
            RecordCube.GetComponent<RectTransform>().sizeDelta = new Vector2(0.96f * BasicInformation.ScreenWidth, 200f);
            GameObject.Find("StudyRecordPage/Scroll/View/Content/" + RecordCube.name + "/Time").GetComponent<Text>().text = recordlist[i].Time;
            GameObject.Find("StudyRecordPage/Scroll/View/Content/" + RecordCube.name + "/State").GetComponent<Text>().text = recordlist[i].State;
            StartCoroutine(DownloadHeadImage(recordlist[i].HeadShot, Index, recordlist[i].Id));
            ItemPos = ItemPos - 220f;
            Index++;
        }
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
    /// 滚到底部刷新
    /// </summary>
    private void OnScrollToEnd()
    {
        if (Content.anchoredPosition.y == ContentHeight - ViewHeight && Batch >= 0)
        {
            StartCoroutine(BasicInformation.StarInterface.IF09(StarId, Batch));
            Batch++;
        }
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
            GameObject.Find("StudyRecordPage/Scroll/View/Content/StudyRecordCube" + id + "/HeadShotMask/HeadShot").GetComponent<Image>().sprite = head;
            GameObject.Find("StudyRecordPage/Scroll/View/Content/StudyRecordCube" + id + "/HeadShotMask").GetComponent<Button>().onClick.AddListener(() => { OnUserClicked(UserId, head); });
        }
    }
}
