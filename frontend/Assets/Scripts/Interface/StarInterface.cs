using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Leguar.TotalJSON;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using System;

public class StarInterface
{
    /// <summary>
    /// 显示提示框
    /// </summary>
    /// <param name="message">信息</param>
    private void ShowMsgNote(string message)
    {
        GameObject MsgNote = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/MsgNote"));
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
                    UnityEngine.Object.Destroy(MsgNote);
                });
            });
        });
    }

    /// <summary>
    /// 显示加载界面
    /// </summary>
    private void ShowLoadPage()
    {
        GameObject.Find("LoadPage").transform.SetAsLastSibling();
    }

    /// <summary>
    /// 隐藏加载界面
    /// </summary>
    private void HideLoadPage()
    {
        GameObject.Find("LoadPage").transform.SetAsFirstSibling();
    }

    /// <summary>
    /// 创建星球接口
    /// </summary>
    /// <param name="creatorId">创建者id</param>
    /// <param name="name">星球名</param>
    /// <param name="introduction">星球简介</param>
    /// <param name="galaxy">星系（0情侣星系、1考研星系、2高考星系、3外语星系、4校园星系、5外太空）</param>
    /// <param name="category">星球种类（0普通星球、1讨论星球、2直播星球）</param>
    /// <param name="password">暗号</param>
    public IEnumerator IF01(int creatorId, string name, string introduction, int galaxy, int category, int password)
    {
        ShowLoadPage();
        WWWForm form = new WWWForm();
        form.AddField("creatorId", creatorId);
        form.AddField("name", name);
        form.AddField("introduction", introduction);
        form.AddField("galaxy", galaxy);
        form.AddField("category", category);
        if(password!=-1)
            form.AddField("password", password);
        using (UnityWebRequest request = UnityWebRequest.Post(BasicInformation.HostIp+"/galaxy/createPlanet", form))
        {
            yield return request.SendWebRequest();
            if (request.isHttpError || request.isNetworkError)
            {
                Debug.Log(request.error);
                ShowMsgNote("网络状态不佳");
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
                JSON data = JSON.ParseString(request.downloadHandler.text);
                int code = data.GetInt("code");
                if (code == 0)
                {
                    ShowMsgNote("创建成功");
                    UnityEngine.Object.Destroy(GameObject.Find("CreateStarPage~"));
                    GameObject.Find("StarListPage").SendMessage("GetNewStar", new Star(int.Parse(data.GetJArray("resData")[0].CreateString()), data.GetJArray("resData")[1].CreateString().Replace("\"",""), creatorId, name, category, galaxy, introduction, 1, DateTime.Now.ToString("yyyy.MM.dd hh:mm")));
                }
                else if(code == -3)
                {
                    UnityEngine.Object.Destroy(GameObject.Find("CreateStarPage~"));
                    GameObject Warning = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Warning"));
                    Warning.name = "Warning";
                    Warning.transform.SetParent(GameObject.Find("Canvas").transform);
                    Warning.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenHeight);
                    Warning.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
                    GameObject.Find("Warning/Info/Text").GetComponent<Text>().text = "学习时长不足哦，每满120min才能创建新的星球！";
                    GameObject.Find("Warning/Info/Submit").GetComponent<Button>().onClick.AddListener(() =>
                    {
                        UnityEngine.Object.Destroy(Warning);
                    });
                    GameObject.Find("Warning/Info/Cancel").GetComponent<Button>().onClick.AddListener(() =>
                    {
                        UnityEngine.Object.Destroy(Warning);
                    });
                }
                else
                    ShowMsgNote("网络状态不佳");
            }
        }
        HideLoadPage();
    }

    /// <summary>
    /// 修改星球信息接口
    /// </summary>
    /// <param name="id">星球id</param>
    /// <param name="name">星球名称</param>
    /// <param name="introduction">简介</param>
    public IEnumerator IF02(int id, string name, string introduction)
    {
        ShowLoadPage();
        WWWForm form = new WWWForm();
        form.AddField("id", id);
        form.AddField("name", name);
        form.AddField("introduction", introduction);
        using (UnityWebRequest request = UnityWebRequest.Post(BasicInformation.HostIp+"/user/modifyPlanetInfo", form))
        {
            yield return request.SendWebRequest();
            if (request.isHttpError || request.isNetworkError)
            {
                Debug.Log(request.error);
                ShowMsgNote("网络状态不佳");
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
                JSON data = JSON.ParseString(request.downloadHandler.text);
                int code = data.GetInt("code");
                if (code == 0)
                {
                    ShowMsgNote("修改成功");
                    UnityEngine.Object.Destroy(GameObject.Find("StarInfoChangePage~"));
                    GameObject StarInfoPage = GameObject.Find("StarInfoPage");
                    StarInfoPage.SendMessage("GetPlanetNameChange", name);
                    StarInfoPage.SendMessage("GetPlanetIntroChange", introduction);
                    try
                    {
                        GameObject StarListPage = GameObject.Find("StarListPage");
                        StarListPage.SendMessage("GetChanegdId", id);
                        StarListPage.SendMessage("GetChanegdName", name);
                        StarListPage.SendMessage("GetChanegdIntro", introduction);
                    }
                    catch { }
                    finally
                    {
                        GameObject.Find("EventSystem").SendMessage("GetRefreshRecent", 1);
                    }
                }
                else
                    ShowMsgNote("网络状态不佳");
            }
        }
        HideLoadPage();
    }

    /// <summary>
    /// 获取星球列表接口（初次加载）
    /// </summary>
    /// <param name="galaxy">星系</param>
    /// <param name="batchNum">批次</param>
    public IEnumerator IF03(int galaxy, int batchNum)
    {
        ShowLoadPage();
        WWWForm form = new WWWForm();
        form.AddField("galaxy", galaxy);
        form.AddField("batchNum", batchNum);
        using (UnityWebRequest request = UnityWebRequest.Post(BasicInformation.HostIp+"/galaxy/queryAllPlanet", form))
        {
            yield return request.SendWebRequest();
            if (request.isHttpError || request.isNetworkError)
            {
                Debug.Log(request.error);
                ShowMsgNote("网络状态不佳");
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
                JSON data = JSON.ParseString(request.downloadHandler.text);
                int code = data.GetInt("code");
                if (code == 0)
                {
                    List<Star> StarList = new List<Star>();
                    JArray starlist = data.GetJArray("resData");
                    for (int i = 0; i < starlist.Length; i++)
                    {
                        JValue staritemvalue = starlist[i];
                        JSON staritem = JSON.ParseString(staritemvalue.CreateString());
                        string Time = staritem.GetString("createTime");
                        Time = Time.Substring(0, 10).Replace('-', '.') + " " + Time.Substring(11, 8);
                        if (staritem.GetInt("category") == 1)
                            StarList.Add(new Star(
                                staritem.GetInt("id"),
                                staritem.GetString("code"),
                                staritem.GetInt("creatorId"),
                                staritem.GetString("name"),
                                staritem.GetInt("category"),
                                staritem.GetInt("galaxy"),
                                staritem.GetString("introduction"),
                                staritem.GetInt("population"),
                                Time,
                                staritem.GetInt("password").ToString()
                            ));
                        else
                            StarList.Add(new Star(
                                staritem.GetInt("id"),
                                staritem.GetString("code"),
                                staritem.GetInt("creatorId"),
                                staritem.GetString("name"),
                                staritem.GetInt("category"),
                                staritem.GetInt("galaxy"),
                                staritem.GetString("introduction"),
                                staritem.GetInt("population"),
                                Time
                            ));
                    }
                    GameObject StarListPage = GameObject.Find("StarListPage");
                    StarListPage.SendMessage("GetStars", StarList);
                }
                else if (code != -1)
                    ShowMsgNote("网络状态不佳");
            }
        }
        HideLoadPage();
    }

    /// <summary>
    /// 获取自己星球列表接口
    /// </summary>
    /// <param name="id">用户id</param>
    public IEnumerator IF04(int id)
    {
        ShowLoadPage();
        WWWForm form = new WWWForm();
        form.AddField("id", id);
        using (UnityWebRequest request = UnityWebRequest.Post(BasicInformation.HostIp+"/user/queryAllPlanet", form))
        {
            yield return request.SendWebRequest();
            if (request.isHttpError || request.isNetworkError)
            {
                Debug.Log(request.error);
                ShowMsgNote("网络状态不佳");
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
                JSON data = JSON.ParseString(request.downloadHandler.text);
                int code = data.GetInt("code");
                if (code == 0)
                {
                    List<Star> StarList = new List<Star>();
                    JArray starlist = data.GetJArray("resData");
                    for (int i = 0; i < starlist.Length; i++)
                    {
                        JValue staritemvalue = starlist[i];
                        JSON staritem = JSON.ParseString(staritemvalue.CreateString());
                        string Time = staritem.GetString("createTime");
                        Time = Time.Substring(0, 10).Replace('-', '.') + " " + Time.Substring(11, 5);
                        if (staritem.GetInt("category") == 1)
                            StarList.Add(new Star(
                                staritem.GetInt("id"),
                                staritem.GetString("code"),
                                staritem.GetInt("creatorId"),
                                staritem.GetString("name"),
                                staritem.GetInt("category"),
                                staritem.GetInt("galaxy"),
                                staritem.GetString("introduction"),
                                staritem.GetInt("population"),
                                Time,
                                staritem.GetInt("password").ToString()
                            ));
                        else
                            StarList.Add(new Star(
                                staritem.GetInt("id"),
                                staritem.GetString("code"),
                                staritem.GetInt("creatorId"),
                                staritem.GetString("name"),
                                staritem.GetInt("category"),
                                staritem.GetInt("galaxy"),
                                staritem.GetString("introduction"),
                                staritem.GetInt("population"),
                                Time
                            ));
                    }
                    GameObject StarListPage = GameObject.Find("StarListPage");
                    StarListPage.SendMessage("GetStars", StarList);
                }
                else
                    ShowMsgNote("网络状态不佳");
            }
        }
        HideLoadPage();
    }

    /// <summary>
    /// 搜素星球接口
    /// </summary>
    /// <param name="keyword">关键词</param>
    public IEnumerator IF05(string keyword)
    {
        ShowLoadPage();
        WWWForm form = new WWWForm();
        form.AddField("keyword", keyword);
        using (UnityWebRequest request = UnityWebRequest.Post(BasicInformation.HostIp+"/galaxy/searchPlanet", form))
        {
            yield return request.SendWebRequest();
            if (request.isHttpError || request.isNetworkError)
            {
                Debug.Log(request.error);
                ShowMsgNote("网络状态不佳");
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
                JSON data = JSON.ParseString(request.downloadHandler.text);
                int code = data.GetInt("code");
                if (code == 0)
                {
                    List<Star> StarList = new List<Star>();
                    JArray starlist = data.GetJArray("resData");
                    for (int i = 0; i < starlist.Length; i++)
                    {
                        JValue staritemvalue = starlist[i];
                        JSON staritem = JSON.ParseString(staritemvalue.CreateString());
                        string Time = staritem.GetString("createTime");
                        Time = Time.Substring(0, 10).Replace('-', '.') + " " + Time.Substring(11, 5);
                        if (staritem.GetInt("category") == 1)
                            StarList.Add(new Star(
                                staritem.GetInt("id"),
                                staritem.GetString("code"),
                                staritem.GetInt("creatorId"),
                                staritem.GetString("name"),
                                staritem.GetInt("category"),
                                staritem.GetInt("galaxy"),
                                staritem.GetString("introduction"),
                                staritem.GetInt("population"),
                                Time,
                                staritem.GetInt("password").ToString()
                            ));
                        else
                            StarList.Add(new Star(
                                staritem.GetInt("id"),
                                staritem.GetString("code"),
                                staritem.GetInt("creatorId"),
                                staritem.GetString("name"),
                                staritem.GetInt("category"),
                                staritem.GetInt("galaxy"),
                                staritem.GetString("introduction"),
                                staritem.GetInt("population"),
                                Time
                            ));
                    }
                    GameObject StarListPage = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/StarModule/StarListPage"));
                    StarListPage.name = "StarListPage";
                    StarListPage.AddComponent<StarListController>();
                    StarListPage.SendMessage("GetMsg", 7);
                    StarListPage.SendMessage("GetStars", StarList);
                }
                else if (code == -1)
                    ShowMsgNote("无搜索结果");
            }
        }
        HideLoadPage();
    }

    /// <summary>
    /// 加入星球接口
    /// </summary>
    /// <param name="userId">用户id</param>
    /// <param name="planetId">星球id</param>
    /// <param name="password">暗号</param>
    public IEnumerator IF06(int userId, int planetId, int password)
    {
        ShowLoadPage();
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("planetId", planetId);
        form.AddField("password", password);
        using (UnityWebRequest request = UnityWebRequest.Post(BasicInformation.HostIp+"/galaxy/joinPlanet", form))
        {
            yield return request.SendWebRequest();
            if (request.isHttpError || request.isNetworkError)
            {
                Debug.Log(request.error);
                ShowMsgNote("网络状态不佳");
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
                JSON data = JSON.ParseString(request.downloadHandler.text);
                int code = data.GetInt("code");
                if (code == 0)
                {
                    ShowMsgNote("加入成功");
                    GameObject.Find("StarInfoPage").SendMessage("GetJoinPlanet");
                    if(password != 0) 
                        UnityEngine.Object.Destroy(GameObject.Find("CodeEnterPage~"));
                }
                else if (code == -3)
                    GameObject.Find("CodeEnterPage~/InfoArea/Note").GetComponent<Text>().text = "暗号错误";
                else if (code == -4)
                {
                    UnityEngine.Object.Destroy(GameObject.Find("CodeEnterPage~"));
                    GameObject Warning = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Warning"));
                    Warning.name = "Warning";
                    Warning.transform.SetParent(GameObject.Find("Canvas").transform);
                    Warning.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenHeight);
                    Warning.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
                    GameObject.Find("Warning/Info/Text").GetComponent<Text>().text = "当前星球人数已满";
                    GameObject.Find("Warning/Info/Submit").GetComponent<Button>().onClick.AddListener(() =>{ UnityEngine.Object.Destroy(Warning);});
                    GameObject.Find("Warning/Info/Cancel").GetComponent<Button>().onClick.AddListener(() =>{ UnityEngine.Object.Destroy(Warning);});
                }
                else
                    ShowMsgNote("网络状态不佳");
            }
        }
        HideLoadPage();
    }

    /// <summary>
    /// 进入星球接口
    /// </summary>
    /// <param name="id">星球id</param>
    public IEnumerator IF07(int planetid, int userid)
    {
        ShowLoadPage();
        WWWForm form = new WWWForm();
        form.AddField("planetId", planetid);
        form.AddField("userId", userid);
        using (UnityWebRequest request = UnityWebRequest.Post(BasicInformation.HostIp+"/galaxy/planet/enterPlanet", form))
        {
            yield return request.SendWebRequest();
            if (request.isHttpError || request.isNetworkError)
            {
                Debug.Log(request.error);
                ShowMsgNote("网络状态不佳");
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
                JSON data = JSON.ParseString(request.downloadHandler.text);
                int code = data.GetInt("code");
                if (code == 0)
                {
                    int UserPlanetRelation = 0;
                    List<User> UserList = new List<User>();
                    JArray result = data.GetJArray("resData");
                    JArray userlist = JArray.ParseString(result[1].CreateString());
                    for (int i = 0; i < userlist.Length; i++)
                    {
                        JValue useritemvalue = userlist[i];
                        JSON useritem = JSON.ParseString(useritemvalue.CreateString());
                        string Time = useritem.GetString("registerTime");
                        Time = Time.Substring(0, 10).Replace('-', '.');
                        if (useritem.GetInt("id") == BasicInformation.CurUser.Id)
                        {
                            if (i == 0)
                                UserPlanetRelation = 2;
                            else
                                UserPlanetRelation = 1;
                        }
                        UserList.Add(new User(
                            useritem.GetInt("id"),
                            useritem.GetString("name"),
                            0, 0, "", "", 0,
                            useritem.GetString("statusInfo"),
                            useritem.GetString("photo")
                            ));
                    }
                    GameObject StarInfoPage = GameObject.Find("StarInfoPage");
                    StarInfoPage.SendMessage("GetUsers", UserList);
                    StarInfoPage.SendMessage("GetUserPlanetRelation", UserPlanetRelation);
                }
            }
        }
        HideLoadPage();
    }

    /// <summary>
    /// 开始学习接口（普通星球）
    /// </summary>
    /// <param name="userId">用户id</param>
    /// <param name="planetId">星球id</param>
    public IEnumerator IF08_0(int userId, int planetId)
    {
        ShowLoadPage();
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("planetId", planetId);
        using (UnityWebRequest request = UnityWebRequest.Post(BasicInformation.HostIp+"/galaxy/planet/start", form))
        {
            yield return request.SendWebRequest();
            if (request.isHttpError || request.isNetworkError)
            {
                Debug.Log(request.error);
                ShowMsgNote("网络状态不佳");
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
                JSON data = JSON.ParseString(request.downloadHandler.text);
                int code = data.GetInt("code");
                if (code == 0)
                {
                    GameObject StarStudyPage = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/StarModule/StarStudyPage0"));
                    StarStudyPage.name = "StarStudyPage";
                    StarStudyPage.AddComponent<StarStudyController0>();
                    StarStudyPage.SendMessage("GetStarId", planetId);
                }
                else if (code == -1)
                    ShowMsgNote("您正在学习中哦");
                else
                    ShowMsgNote("网络状态不佳");
            }
        }
        HideLoadPage();
    }

    /// <summary>
    /// 开始学习接口（研讨星球）
    /// </summary>
    /// <param name="userId">用户id</param>
    /// <param name="planetId">星球id</param>
    public IEnumerator IF08_1(int userId, int planetId)
    {
        ShowLoadPage();
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("planetId", planetId);
        using (UnityWebRequest request = UnityWebRequest.Post(BasicInformation.HostIp+"/galaxy/planet/start", form))
        {
            yield return request.SendWebRequest();
            if (request.isHttpError || request.isNetworkError)
            {
                Debug.Log(request.error);
                ShowMsgNote("网络状态不佳");
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
                JSON data = JSON.ParseString(request.downloadHandler.text);
                int code = data.GetInt("code");
                if (code == 0)
                {
                    GameObject StarStudyPage = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/StarModule/StarStudyPage1"));
                    StarStudyPage.name = "StarStudyPage";
                    StarStudyPage.AddComponent<StarStudyController1>();
                    StarStudyPage.SendMessage("GetStarId", planetId);
                }
                else if (code == -1)
                    ShowMsgNote("您正在学习中哦");
                else
                    ShowMsgNote("网络状态不佳");
            }
        }
        HideLoadPage();
    }

    /// <summary>
    /// 获取星球动态接口
    /// </summary>
    /// <param name="id">星球id</param>
    /// <param name="batchNum">批次</param>
    public IEnumerator IF09(int id, int batchNum)
    {
        ShowLoadPage();
        WWWForm form = new WWWForm();
        form.AddField("id", id);
        form.AddField("batchNum", batchNum);
        using (UnityWebRequest request = UnityWebRequest.Post(BasicInformation.HostIp+"/galaxy/getPlanetFeed", form))
        {
            yield return request.SendWebRequest();
            if (request.isHttpError || request.isNetworkError)
            {
                Debug.Log(request.error);
                ShowMsgNote("网络状态不佳");
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
                JSON data = JSON.ParseString(request.downloadHandler.text);
                int code = data.GetInt("code");
                if(code == 0)
                {
                    JArray recordlist = data.GetJArray("resData");
                    List<Record> RecordList = new List<Record>();
                    for (int i = 0; i < recordlist.Length; i++)
                    {
                        JValue recorditemvalue = recordlist[i];
                        JArray recorditem = JArray.ParseString(recorditemvalue.CreateString());
                        JSON userinfo = JSON.ParseString(recorditem[0].CreateString());
                        string Time = recorditem[1].CreateString().Substring(1, 10).Replace('-', '.') + " " + recorditem[1].CreateString().Substring(12, 5);
                        string State;
                        int StateInt = int.Parse(recorditem[2].CreateString());
                        if (StateInt == 0)
                            State = "开始学习";
                        else if (StateInt == 1)
                            State = "结束学习";
                        else if (StateInt == 2)
                            State = "开始直播";
                        else
                            State = "结束直播";
                        RecordList.Add(new Record(
                            userinfo.GetInt("id"),
                            Time,
                            userinfo.GetString("photo"),
                            State
                            ));
                    }
                    GameObject StudyRecordPage = GameObject.Find("StudyRecordPage");
                    StudyRecordPage.SendMessage("GetRecords", RecordList);
                }
                else if(code == -1)
                {
                    GameObject StudyRecordPage = GameObject.Find("StudyRecordPage");
                    StudyRecordPage.SendMessage("GetRecords", new List<Record>());
                }
                else
                    ShowMsgNote("网络状态不佳");
            }
        }
        HideLoadPage();
    }

    /// <summary>
    /// 退出星球接口
    /// </summary>
    /// <param name="planetId">星球id</param>
    /// <param name="userId">用户id</param>
    public IEnumerator IF10(int planetId, int userId)
    {
        ShowLoadPage();
        WWWForm form = new WWWForm();
        form.AddField("planetId", planetId);
        form.AddField("userId", userId);
        using (UnityWebRequest request = UnityWebRequest.Post(BasicInformation.HostIp+"/galaxy/leavePlanet", form))
        {
            yield return request.SendWebRequest();
            if (request.isHttpError || request.isNetworkError)
            {
                Debug.Log(request.error);
                ShowMsgNote("网络状态不佳");
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
                JSON data = JSON.ParseString(request.downloadHandler.text);
                int code = data.GetInt("code");
                if (code == 0)
                {
                    ShowMsgNote("成功退出");
                    UnityEngine.Object.Destroy(GameObject.Find("Warning"));
                    UnityEngine.Object.Destroy(GameObject.Find("SmallStarInfoPage~"));
                    UnityEngine.Object.Destroy(GameObject.Find("StarInfoPage"));
                    try
                    {
                        GameObject.Find("StarListPage").SendMessage("GetQuitedId", planetId);
                    }
                    catch { }
                    finally
                    {
                        GameObject.Find("EventSystem").SendMessage("GetRefreshRecent", 1);
                    }
                }
            }
        }
        HideLoadPage();
    }

    /// <summary>
    /// 开始直播
    /// </summary>
    /// <param name="userId">用户id</param>
    /// <param name="planetId">星球id</param>
    public IEnumerator IF11(int userId, int planetId)
    {
        ShowLoadPage();
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("planetId", planetId);
        using (UnityWebRequest request = UnityWebRequest.Post(BasicInformation.HostIp+"/galaxy/planet/startBroadcast", form))
        {
            yield return request.SendWebRequest();
            if (request.isHttpError || request.isNetworkError)
            {
                Debug.Log(request.error);
                ShowMsgNote("网络状态不佳");
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
                JSON data = JSON.ParseString(request.downloadHandler.text);
                int code = data.GetInt("code");
                if (code == 0)
                {
                    GameObject StarStudyPage = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/StarModule/StarStudyPage2"));
                    StarStudyPage.name = "StarStudyPage";
                    StarStudyPage.AddComponent<StarStudyController2>();
                    StarStudyPage.SendMessage("GetStarId", planetId);
                    StarStudyPage.SendMessage("GetRole", true);
                }
                else if (code == -1)
                    ShowMsgNote("您正在学习中哦");
                else
                    ShowMsgNote("网络状态不佳");
            }
        }
        HideLoadPage();
    }

    /// <summary>
    /// 查看直播
    /// </summary>
    /// <param name="userId">用户id</param>
    /// <param name="planetId">星球id</param>
    public IEnumerator IF12(int userId, int planetId)
    {
        ShowLoadPage();
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("planetId", planetId);
        using (UnityWebRequest request = UnityWebRequest.Post(BasicInformation.HostIp+"/galaxy/planet/watchBroadcast", form))
        {
            yield return request.SendWebRequest();
            if (request.isHttpError || request.isNetworkError)
            {
                Debug.Log(request.error);
                ShowMsgNote("网络状态不佳");
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
                JSON data = JSON.ParseString(request.downloadHandler.text);
                int code = data.GetInt("code");
                if (code == 0)
                {
                    GameObject StarStudyPage = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/StarModule/StarStudyPage2"));
                    StarStudyPage.name = "StarStudyPage";
                    StarStudyPage.AddComponent<StarStudyController2>();
                    StarStudyPage.SendMessage("GetStarId", planetId);
                    StarStudyPage.SendMessage("GetRole", false);
                }
                else if (code == -1)
                    ShowMsgNote("您正在学习中哦");
                else if (code == -4)
                    ShowMsgNote("主播未上线");
                else
                    ShowMsgNote("网络状态不佳");
            }
        }
        HideLoadPage();
    }
}