using Leguar.TotalJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using DG.Tweening;
using System;
using System.Globalization;

public class OtherInterface
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
    /// 获取学习记录1接口
    /// </summary>
    public IEnumerator IF01(int id, int batch)
    {
        GameObject.Find("Pages/Page3/View/Content/TimeStatic/Note").GetComponent<CanvasGroup>().alpha = 0;
        GameObject.Find("Pages/Page3/View/Content/TimeStatic/Loading").GetComponent<CanvasGroup>().alpha = 1;
        GameObject.Find("Pages/Page3/View/Content/TimeStatic/Loading").transform.SetAsLastSibling();
        GameObject.Find("Pages/Page3/View/Content/TimeStatic/ChartMask").GetComponent<CanvasGroup>().alpha = 0;
        WWWForm form = new WWWForm();
        form.AddField("id", id);
        form.AddField("batchNum", batch);
        using (UnityWebRequest request = UnityWebRequest.Post(BasicInformation.HostIp+"/other/queryStatistics1", form))
        {
            request.timeout = 5;
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
                    List<int> times = new List<int>();
                    JArray resData = data.GetJArray("resData");
                    for (int i = 0; i < resData.Length; i++)
                        times.Add(int.Parse(resData[i].CreateString()));
                    GameObject.Find("EventSystem").SendMessage("GetDrawChart1", times);
                }
                else
                {
                    ShowMsgNote("网络状态不佳");
                    GameObject.Find("Pages/Page3/View/Content/TimeStatic/Loading").GetComponent<CanvasGroup>().alpha = 0;
                    GameObject.Find("Pages/Page3/View/Content/TimeStatic/Loading").transform.SetAsFirstSibling();
                    GameObject.Find("Pages/Page3/View/Content/TimeStatic/ChartMask").GetComponent<CanvasGroup>().alpha = 1;
                }
            }
        }
    }

    /// <summary>
    /// 获取学习记录2接口
    /// </summary>
    public IEnumerator IF02(int id, int category)
    {
        GameObject.Find("Pages/Page3/View/Content/TimeStatic2/Note").GetComponent<CanvasGroup>().alpha = 0;
        GameObject.Find("Pages/Page3/View/Content/TimeStatic2/Loading").GetComponent<CanvasGroup>().alpha = 1;
        GameObject.Find("Pages/Page3/View/Content/TimeStatic2/Loading").transform.SetAsLastSibling();
        GameObject.Find("Pages/Page3/View/Content/TimeStatic2/Chart2").GetComponent<CanvasGroup>().alpha = 0;
        WWWForm form = new WWWForm();
        form.AddField("id", id);
        form.AddField("category", category);
        using (UnityWebRequest request = UnityWebRequest.Post(BasicInformation.HostIp+"/other/queryStatistics2", form))
        {
            request.timeout = 5;
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
                    List<int> times = new List<int>();
                    JSON resData = data.GetJSON("resData");
                    times.Add(resData.GetInt("0"));
                    times.Add(resData.GetInt("1"));
                    times.Add(resData.GetInt("2"));
                    times.Add(resData.GetInt("3"));
                    times.Add(resData.GetInt("4"));
                    times.Add(resData.GetInt("5"));
                    GameObject.Find("EventSystem").SendMessage("GetDrawChart2", times);
                }
                else
                {
                    ShowMsgNote("网络状态不佳");
                    GameObject.Find("Pages/Page3/View/Content/TimeStatic2/Loading").GetComponent<CanvasGroup>().alpha = 0;
                    GameObject.Find("Pages/Page3/View/Content/TimeStatic2/Loading").transform.SetAsFirstSibling();
                    GameObject.Find("Pages/Page3/View/Content/TimeStatic2/Chart2").GetComponent<CanvasGroup>().alpha = 1;
                }
            }
        }
    }

    /// <summary>
    /// 创建清单接口
    /// </summary>
    /// <param name="id">用户id</param>
    /// <param name="name">清单名</param>
    public IEnumerator IF03(int id, string name)
    {
        ShowLoadPage();
        WWWForm form = new WWWForm();
        form.AddField("id", id);
        form.AddField("name", name);
        using (UnityWebRequest request = UnityWebRequest.Post(BasicInformation.HostIp+"/other/createList", form))
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
                    UnityEngine.Object.Destroy(GameObject.Find("CreateTodoListPage~"));
                    ShowMsgNote("创建成功");
                    GameObject.Find("TodoListPage").SendMessage("GetNewList", new TodoList(data.GetInt("resData"), name, DateTime.Now.ToString("yyyy -MM-dd")));
                }
            }
        }
        HideLoadPage();
    }

    /// <summary>
    /// 添加清单条目接口
    /// </summary>
    /// <param name="id">清单id</param>
    /// <param name="content">内容</param>
    /// <param name="prior">优先级</param>
    public IEnumerator IF04(int id, string content, int prior)
    {
        ShowLoadPage();
        WWWForm form = new WWWForm();
        form.AddField("id", id);
        form.AddField("content", content);
        form.AddField("priority", prior);
        using (UnityWebRequest request = UnityWebRequest.Post(BasicInformation.HostIp+"/other/todoList/addItem", form))
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
                    yield return IF05_1(BasicInformation.CurUser.Id);
                    GameObject.Find("TodoPage").SendMessage("GetNewItem", new TodoListItem(data.GetInt("resData"), content, prior, false));
                    UnityEngine.Object.Destroy(GameObject.Find("CreateTodoItemPage~"));
                    ShowMsgNote("添加成功");
                }
                else
                    ShowMsgNote("网络状态不佳");
            }
        }
        HideLoadPage();
    }

    /// <summary>
    /// 获取个人清单接口（登陆时）
    /// </summary>
    /// <param name="id">用户id</param>
    public IEnumerator IF05_0(int id)
    {
        GameObject.Find("Pages/Page1/View/Content/TodoListArea/Note").GetComponent<Text>().text = "加载中";
        WWWForm form = new WWWForm();
        form.AddField("id", id);
        using (UnityWebRequest request = UnityWebRequest.Post(BasicInformation.HostIp+"/other/getList", form))
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
                    JArray info = data.GetJArray("resData");
                    List<TodoList> todoLists = new List<TodoList>();
                    for (int i = 0; i < info.Length; i++)
                    {
                        JArray todolist = JArray.ParseString(info[i].CreateString());
                        JSON listinfo = JSON.ParseString(todolist[0].CreateString());
                        JArray items = JArray.ParseString(todolist[1].CreateString());
                        TodoList TodoList = new TodoList(listinfo.GetInt("id"), listinfo.GetString("name"), listinfo.GetString("createTime").Substring(0, 10));
                        for (int j = 0; j < items.Length; j++)
                        {
                            JSON item = JSON.ParseString(items[j].CreateString());
                            TodoList.Items.Add(new TodoListItem(item.GetInt("id"), item.GetString("content"), item.GetInt("priority"), item.GetInt("isFinished") == 0 ? false : true));
                        }
                        todoLists.Add(TodoList);
                    }
                    todoLists.Sort((left, right) =>
                    {
                        if (left.GetUnDone() < right.GetUnDone())
                            return 1;
                        else if (left.GetUnDone() == right.GetUnDone())
                            return 0;
                        else
                            return -1;
                    });
                    if (todoLists.Count >= 2)
                    {
                        GameObject.Find("EventSystem").SendMessage("GetTodoCount", 2);
                        GameObject.Find("EventSystem").SendMessage("GetTodo1", new ArrayList() { todoLists[0].Name, todoLists[0].GetUnDone() });
                        GameObject.Find("EventSystem").SendMessage("GetTodo2", new ArrayList() { todoLists[1].Name, todoLists[1].GetUnDone() });
                    }
                    else if (todoLists.Count == 1)
                    {
                        GameObject.Find("EventSystem").SendMessage("GetTodoCount", 1);
                        GameObject.Find("EventSystem").SendMessage("GetTodo1", new ArrayList() { todoLists[0].Name, todoLists[0].GetUnDone() });
                    }
                    else
                        GameObject.Find("EventSystem").SendMessage("GetTodoCount", 3);
                }
                else
                    GameObject.Find("Pages/Page1/View/Content/TodoListArea/Note").GetComponent<Text>().text = "网络状态不佳，加载失败";
            }
        }
    }

    /// <summary>
    /// 获取个人清单接口
    /// </summary>
    /// <param name="id">用户id</param>
    public IEnumerator IF05_1(int id)
    {
        ShowLoadPage();
        WWWForm form = new WWWForm();
        form.AddField("id", id);
        using (UnityWebRequest request = UnityWebRequest.Post(BasicInformation.HostIp+"/other/getList", form))
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
                    JArray info = data.GetJArray("resData");
                    List<TodoList> todoLists = new List<TodoList>();
                    for (int i = 0; i < info.Length; i++)
                    {
                        JArray todolist = JArray.ParseString(info[i].CreateString());
                        JSON listinfo = JSON.ParseString(todolist[0].CreateString());
                        JArray items = JArray.ParseString(todolist[1].CreateString());
                        TodoList TodoList = new TodoList(listinfo.GetInt("id"), listinfo.GetString("name"), listinfo.GetString("createTime").Substring(0, 10));
                        for(int j = 0; j < items.Length; j++)
                        {
                            JSON item = JSON.ParseString(items[j].CreateString());
                            TodoList.Items.Add(new TodoListItem(item.GetInt("id"), item.GetString("content"), item.GetInt("priority"), item.GetInt("isFinished") == 0 ? false : true));
                        }
                        todoLists.Add(TodoList);
                    }
                    GameObject.Find("TodoListPage").SendMessage("GetTodoLists", todoLists);
                }
                else
                    ShowMsgNote("网络状态不佳");
            }
        }
        HideLoadPage();
    }

    /// <summary>
    /// 勾选清单条目接口
    /// </summary>
    /// <param name="id">条目id</param>
    public IEnumerator IF06(int id)
    {
        ShowLoadPage();
        WWWForm form = new WWWForm();
        form.AddField("id", id);
        using (UnityWebRequest request = UnityWebRequest.Post(BasicInformation.HostIp+"/other/todoList/checkItem", form))
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
                    yield return IF05_1(BasicInformation.CurUser.Id);
                    GameObject.Find("TodoPage").SendMessage("GetCheckedId", new ArrayList() { id, true });
                }
                else
                    ShowMsgNote("网络状态不佳");
            }
        }
        HideLoadPage();
    }

    /// <summary>
    /// 反选清单条目接口
    /// </summary>
    /// <param name="id">条目id</param>
    public IEnumerator IF07(int id)
    {
        ShowLoadPage();
        WWWForm form = new WWWForm();
        form.AddField("id", id);
        using (UnityWebRequest request = UnityWebRequest.Post(BasicInformation.HostIp+"/other/todoList/uncheckItem", form))
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
                    yield return IF05_1(BasicInformation.CurUser.Id);
                    GameObject.Find("TodoPage").SendMessage("GetCheckedId", new ArrayList() { id, false });
                }
                else
                    ShowMsgNote("网络状态不佳");
            }
        }
        HideLoadPage();
    }

    /// <summary>
    /// 修改条目优先级接口
    /// </summary>
    /// <param name="id">条目id</param>
    /// <param name="prior">优先级</param>
    public IEnumerator IF08(int id, int prior)
    {
        ShowLoadPage();
        WWWForm form = new WWWForm();
        form.AddField("id", id);
        form.AddField("priority", prior);
        using (UnityWebRequest request = UnityWebRequest.Post(BasicInformation.HostIp+"/other/todoList/changePriority", form))
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
                    GameObject.Find("TodoPage").SendMessage("GetChangedId", new int[] { id, prior });
                else
                    ShowMsgNote("网络状态不佳");
            }
        }
        HideLoadPage();
    }

    /// <summary>
    /// 删除清单条目接口
    /// </summary>
    /// <param name="id">条目id</param>
    public IEnumerator IF09(int id)
    {
        ShowLoadPage();
        WWWForm form = new WWWForm();
        form.AddField("id", id);
        using (UnityWebRequest request = UnityWebRequest.Post(BasicInformation.HostIp+"/other/todoList/deleteItem", form))
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
                    yield return IF05_1(BasicInformation.CurUser.Id);
                    GameObject.Find("TodoPage").SendMessage("GetDeleteId", id);
                    ShowMsgNote("删除成功");
                }
                else
                    ShowMsgNote("网络状态不佳");
            }
        }
        HideLoadPage();
    }

    /// <summary>
    /// 删除清单接口
    /// </summary>
    /// <param name="id">清单id</param>
    public IEnumerator IF10(int id)
    {
        ShowLoadPage();
        WWWForm form = new WWWForm();
        form.AddField("id", id);
        using (UnityWebRequest request = UnityWebRequest.Post(BasicInformation.HostIp+"/other/deleteList", form))
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
                    ShowMsgNote("删除成功");
                    GameObject x = GameObject.Find("TodoPage");
                    x.transform.DOMoveX(3 * BasicInformation.ScreenWidth / 2, 0.1f).OnComplete(() => { UnityEngine.Object.Destroy(x); });
                    GameObject.Find("TodoListPage").SendMessage("GetDeleteId", id);
                }
                else
                    ShowMsgNote("网络状态不佳");
            }
        }
        HideLoadPage();
    }

    /// <summary>
    /// 创建倒计时接口
    /// </summary>
    /// <param name="id">用户id</param>
    /// <param name="name">倒计时名称</param>
    /// <param name="remark">备注</param>
    /// <param name="time">结束时间</param>
    public IEnumerator IF11(int id, string name, string remark, string time)
    {
        ShowLoadPage();
        WWWForm form = new WWWForm();
        form.AddField("userId", id);
        form.AddField("name", name);
        form.AddField("remark", remark);
        form.AddField("endTime", time);
        using (UnityWebRequest request = UnityWebRequest.Post(BasicInformation.HostIp+"/other/createCountDown", form))
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
                    UnityEngine.Object.Destroy(GameObject.Find("CreateCntDownPage~"));
                    DateTime dt;
                    DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
                    dtFormat.ShortDatePattern = "yyyy-MM-dd";
                    dt = Convert.ToDateTime(time, dtFormat);
                    GameObject.Find("CntDownPage").SendMessage("GetAddCnt", new CntDown(data.GetInt("resData"), name, remark, dt));
                }
                else
                    ShowMsgNote("网络状态不佳");
            }
        }
        HideLoadPage();
    }

    /// <summary>
    /// 修改倒计时接口
    /// </summary>
    /// <param name="id">用户id</param>
    /// <param name="name">倒计时名称</param>
    /// <param name="remark">备注</param>
    /// <param name="time">结束时间</param>
    public IEnumerator IF12(int id, string name, string remark, string time)
    {
        ShowLoadPage();
        WWWForm form = new WWWForm();
        form.AddField("id", id);
        form.AddField("name", name);
        form.AddField("remark", remark);
        form.AddField("endTime", time);
        using (UnityWebRequest request = UnityWebRequest.Post(BasicInformation.HostIp+"/other/modifyCountDown", form))
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
                    UnityEngine.Object.Destroy(GameObject.Find("EditCntDownPage~"));
                    DateTime dt;
                    DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
                    dtFormat.ShortDatePattern = "yyyy-MM-dd";
                    dt = Convert.ToDateTime(time, dtFormat);
                    GameObject.Find("CntDownPage").SendMessage("GetChangeCnt", new CntDown(id, name, remark, dt));
                }
                else
                    ShowMsgNote("网络状态不佳");
            }
        }
        HideLoadPage();
    }

    /// <summary>
    /// 获取倒计时接口（登录时）
    /// </summary>
    /// <param name="id">用户id</param>
    public IEnumerator IF13_0(int id)
    {
        GameObject.Find("Pages/Page1/View/Content/CountDownArea/Note").GetComponent<Text>().text = "加载中";
        WWWForm form = new WWWForm();
        form.AddField("userId", id);
        using (UnityWebRequest request = UnityWebRequest.Post(BasicInformation.HostIp+"/other/getCountDown", form))
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
                    List<CntDown> cntDowns = new List<CntDown>();
                    JArray info = data.GetJArray("resData");
                    for (int i = 0; i < info.Length; i++)
                    {
                        JSON cntdown = JSON.ParseString(info[i].CreateString());
                        DateTime dt;
                        DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
                        dtFormat.ShortDatePattern = "yyyy-MM-dd";
                        dt = Convert.ToDateTime(cntdown.GetString("endTime"), dtFormat);
                        cntDowns.Add(new CntDown(cntdown.GetInt("id"), cntdown.GetString("name"), cntdown.GetString("remark"), dt));
                    }
                    cntDowns.Sort((left, right) =>
                    {
                        if (left.Span < 0 && right.Span < 0)
                        {
                            if (left.Span < right.Span)
                                return 1;
                            else if (left.Span == right.Span)
                                return 0;
                            else
                                return -1;
                        }
                        else if (left.Span >= 0 && right.Span >= 0)
                        {
                            if (left.Span > right.Span)
                                return 1;
                            else if (left.Span == right.Span)
                                return 0;
                            else
                                return -1;
                        }
                        else
                        {
                            if (left.Span > right.Span)
                                return 1;
                            else if (left.Span == right.Span)
                                return 0;
                            else
                                return -1;
                        }
                    });
                    if (cntDowns.Count >= 2)
                    {
                        GameObject.Find("EventSystem").SendMessage("GetCntCount", 2);
                        CntDown c = cntDowns[0];
                        GameObject.Find("EventSystem").SendMessage("GetCntDown1", c);
                        c = cntDowns[1];
                        GameObject.Find("EventSystem").SendMessage("GetCntDown2", c);
                    }
                    else if (cntDowns.Count == 1)
                    {
                        GameObject.Find("EventSystem").SendMessage("GetCntCount", 1);
                        CntDown c = cntDowns[0];
                        GameObject.Find("EventSystem").SendMessage("GetCntDown1", c);
                    }
                    else
                        GameObject.Find("EventSystem").SendMessage("GetCntCount", 3);
                }
                else if (code==-1)
                    GameObject.Find("EventSystem").SendMessage("GetCntCount", 3);
                else
                    GameObject.Find("Pages/Page1/View/Content/CountDownArea/Note").GetComponent<Text>().text = "网络状态不佳，加载失败";
            }
        }
    }

    /// <summary>
    /// 获取倒计时接口
    /// </summary>
    /// <param name="id">用户id</param>
    public IEnumerator IF13_1(int id)
    {
        ShowLoadPage();
        WWWForm form = new WWWForm();
        form.AddField("userId", id);
        using (UnityWebRequest request = UnityWebRequest.Post(BasicInformation.HostIp+"/other/getCountDown", form))
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
                    List<CntDown> cntDowns = new List<CntDown>();
                    JArray info = data.GetJArray("resData");
                    for (int i = 0; i < info.Length; i++)
                    {
                        JSON cntdown = JSON.ParseString(info[i].CreateString());
                        DateTime dt;
                        DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
                        dtFormat.ShortDatePattern = "yyyy-MM-dd";
                        dt = Convert.ToDateTime(cntdown.GetString("endTime"), dtFormat);
                        cntDowns.Add(new CntDown(cntdown.GetInt("id"), cntdown.GetString("name"), cntdown.GetString("remark"), dt));
                    }
                    GameObject.Find("CntDownPage").SendMessage("GetCntDowns", cntDowns);
                }
                else if (code == -1)
                    GameObject.Find("CntDownPage").SendMessage("GetCntDowns", new List<CntDown>());
                else
                    ShowMsgNote("网络状态不佳");
            }
        }
        HideLoadPage();
    }

    /// <summary>
    /// 删除倒计时接口
    /// </summary>
    /// <param name="id">倒计时id</param>
    /// <returns></returns>
    public IEnumerator IF14(int id)
    {
        ShowLoadPage();
        WWWForm form = new WWWForm();
        form.AddField("id", id);
        using (UnityWebRequest request = UnityWebRequest.Post(BasicInformation.HostIp+"/other/deleteCountDown", form))
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
                    ShowMsgNote("删除成功");
                    UnityEngine.Object.Destroy(GameObject.Find("EditCntDownPage~"));
                    GameObject.Find("CntDownPage").SendMessage("GetDeleteId", id);
                }
                else
                    ShowMsgNote("网络状态不佳");
            }
        }
        HideLoadPage();
    }

    /// <summary>
    /// 获取最近常用的3个星球
    /// </summary>
    /// <param name="id">用户id</param>
    /// <returns></returns>
    public IEnumerator IF15(int id)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", id);
        using (UnityWebRequest request = UnityWebRequest.Post(BasicInformation.HostIp+"/other/queryThreeMost", form))
        {
            GameObject.Find("Pages/Page1/View/Content/RecentStar/Note").GetComponent<Text>().text = "加载中";
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
                    JArray info = data.GetJArray("resData");
                    List<Star> stars = new List<Star>();
                    for(int i = 0; i < info.Length; i++)
                    {
                        JSON staritem = JSON.ParseString(info[i].CreateString());
                        string Time = staritem.GetString("createTime");
                        Time = Time.Substring(0, 10).Replace('-', '.') + " " + Time.Substring(11, 8);
                        if (staritem.GetInt("category") == 1)
                            stars.Add(new Star(
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
                            stars.Add(new Star(
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
                    GameObject.Find("EventSystem").SendMessage("GetRecentPlanets", stars);
                }
                else
                    ShowMsgNote("网络状态不佳");
            }
        }
    }

    /// <summary>
    /// 获取学习总时间
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public IEnumerator IF16(int id, int type)
    {
        WWWForm form = new WWWForm();
        form.AddField("userId", id);
        using (UnityWebRequest request = UnityWebRequest.Post(BasicInformation.HostIp+"/other/getTotalStudyTime", form))
        {
            GameObject.Find("Pages/Page1/View/Content/RecentStar/Note").GetComponent<Text>().text = "加载中";
            yield return request.SendWebRequest();
            if (request.isHttpError || request.isNetworkError)
            {
                Debug.Log(request.error);
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
                JSON data = JSON.ParseString(request.downloadHandler.text);
                int code = data.GetInt("code");
                if (code == 0)
                {
                    int delta = data.GetInt("resData") - BasicInformation.CurUser.TotLearnTime;
                    BasicInformation.CurUser.TotLearnTime = data.GetInt("resData");
                    GameObject.Find("EventSystem").SendMessage("GetRefreshTotTime", new int[] { type, delta });
                }
            }
        }
    }
}