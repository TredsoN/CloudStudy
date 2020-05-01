using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmallStarInfoController : MonoBehaviour
{
    Star Star;

    int Relation;

    /// <summary>
    /// 脚本加载
    /// </summary>
    private void Awake()
    {
        GameObject SmallStarInfoPage = GameObject.Find("SmallStarInfoPage~");
        SmallStarInfoPage.transform.SetParent(GameObject.Find("Canvas").transform);
        SmallStarInfoPage.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenHeight);
        SmallStarInfoPage.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
        GameObject.Find("SmallStarInfoPage~/InfoArea/Back").GetComponent<Button>().onClick.AddListener(() => { OnReturnClicked(); });
    }

    /// <summary>
    /// 接收星球信息
    /// </summary>
    public void GetStar(Star star)
    {
        Star = star;
        GameObject.Find("SmallStarInfoPage~/InfoArea/Name").GetComponent<Text>().text = star.Name;
        if (star.Type == 0)
            GameObject.Find("SmallStarInfoPage~/InfoArea/Type/Text").GetComponent<Text>().text = "普通星球";
        else if (star.Type == 1)
        {
            GameObject.Find("SmallStarInfoPage~/InfoArea/Type/Text").GetComponent<Text>().text = "研讨星球";
            GameObject.Find("SmallStarInfoPage~/InfoArea/Code").GetComponent<Button>().onClick.AddListener(() => { OnShowCodeClicked(); });
        }
        else if (star.Type == 2)
            GameObject.Find("SmallStarInfoPage~/InfoArea/Type/Text").GetComponent<Text>().text = "直播星球";
        if (star.Galaxy == 0)
            GameObject.Find("SmallStarInfoPage~/InfoArea/Galaxy/Text").GetComponent<Text>().text = "情侣星系";
        else if (star.Galaxy == 1)
            GameObject.Find("SmallStarInfoPage~/InfoArea/Galaxy/Text").GetComponent<Text>().text = "考研星系";
        else if (star.Galaxy == 2)
            GameObject.Find("SmallStarInfoPage~/InfoArea/Galaxy/Text").GetComponent<Text>().text = "高考星系";
        else if (star.Galaxy == 3)
            GameObject.Find("SmallStarInfoPage~/InfoArea/Galaxy/Text").GetComponent<Text>().text = "外语星系";
        else if (star.Galaxy == 4)
            GameObject.Find("SmallStarInfoPage~/InfoArea/Galaxy/Text").GetComponent<Text>().text = "校园星系";
        else if (star.Galaxy == 5)
            GameObject.Find("SmallStarInfoPage~/InfoArea/Galaxy/Text").GetComponent<Text>().text = "外太空";
        GameObject.Find("SmallStarInfoPage~/InfoArea/Time/Text").GetComponent<Text>().text = star.CreateTime;
        GameObject.Find("SmallStarInfoPage~/InfoArea/Introduce/Text").GetComponent<Text>().text = star.Introduce;
    }

    /// <summary>
    /// 接收用户与星球关系
    /// </summary>
    public void GetRelation(int relation)
    {
        Relation = relation;
        if (relation == 1)
        {
            Destroy(GameObject.Find("SmallStarInfoPage~/InfoArea/Edit"));
            GameObject.Find("SmallStarInfoPage~/InfoArea/Destroy").GetComponent<Button>().onClick.AddListener(() => { OnLeaveCicked(); });
        }
        else if (relation == 2)
        {
            Destroy(GameObject.Find("SmallStarInfoPage~/InfoArea/Destroy"));
            GameObject.Find("SmallStarInfoPage~/InfoArea/Edit").GetComponent<Button>().onClick.AddListener(() => { OnEditCicked(); });
        }
        else
        {
            Destroy(GameObject.Find("SmallStarInfoPage~/InfoArea/Edit"));
            Destroy(GameObject.Find("SmallStarInfoPage~/InfoArea/Destroy"));
        }
    }

    /// <summary>
    /// 返回
    /// </summary>
    private void OnReturnClicked()
    {
        Destroy(GameObject.Find("SmallStarInfoPage~"));
    }

    /// <summary>
    /// 编辑星球信息
    /// </summary>
    private void OnEditCicked()
    {
        Destroy(GameObject.Find("SmallStarInfoPage~"));
        GameObject StarInfoChangePage = (GameObject)Instantiate(Resources.Load("Prefabs/StarModule/StarInfoChange"));
        StarInfoChangePage.name = "StarInfoChangePage~";
        StarInfoChangePage.AddComponent<StarInfoChangeController>();
        StarInfoChangePage.SendMessage("GetStar", Star);
    }

    /// <summary>
    /// 点击查看暗号
    /// </summary>
    private void OnShowCodeClicked()
    {
        if (Relation == 0)
        {
            GameObject Warning = (GameObject)Instantiate(Resources.Load("Prefabs/Warning"));
            Warning.name = "Warning";
            Warning.transform.SetParent(GameObject.Find("Canvas").transform);
            Warning.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenHeight);
            Warning.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
            GameObject.Find("Warning/Info/Text").GetComponent<Text>().text = "加入后才能查看星球暗号哦";
            GameObject.Find("Warning/Info/Submit").GetComponent<Button>().onClick.AddListener(() =>
            {
                Destroy(Warning);
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
            GameObject.Find("Warning/Info/Text").GetComponent<Text>().text = "星球暗号为\n" + Star.Password;
            GameObject.Find("Warning/Info/Submit/Text").GetComponent<Text>().text = "复制暗号";
            GameObject.Find("Warning/Info/Submit").GetComponent<Button>().onClick.AddListener(() =>
            {
                BlankOperationClipboard.SetValue(Star.Password);
                GameObject MsgNote = (GameObject)Instantiate(Resources.Load("Prefabs/MsgNote"));
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
                Destroy(Warning);
            });
            GameObject.Find("Warning/Info/Cancel").GetComponent<Button>().onClick.AddListener(() =>
            {
                Destroy(Warning);
            });
        }
    }

    /// <summary>
    /// 退出星球
    /// </summary>
    private void OnLeaveCicked()
    {
        GameObject Warning = (GameObject)Instantiate(Resources.Load("Prefabs/Warning"));
        Warning.name = "Warning";
        Warning.transform.SetParent(GameObject.Find("Canvas").transform);
        Warning.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenHeight);
        Warning.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
        GameObject.Find("Warning/Info/Text").GetComponent<Text>().text = "退出后所有该星球的学习记录将会消失，是否确认？";
        GameObject.Find("Warning/Info/Submit").GetComponent<Button>().onClick.AddListener(() =>
        {
            StartCoroutine(BasicInformation.StarInterface.IF10(Star.Id, BasicInformation.CurUser.Id));
        });
        GameObject.Find("Warning/Info/Cancel").GetComponent<Button>().onClick.AddListener(() =>
        {
            Destroy(Warning);
        });
    }
}