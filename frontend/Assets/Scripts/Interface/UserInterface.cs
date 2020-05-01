using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using DG.Tweening;
using Leguar.TotalJSON;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class UserInterface
{
    /// <summary>
    /// 显示提示框
    /// </summary>
    /// <param name="message">信息</param>
    private void ShowMsgNote(string message)
    {
        GameObject MsgNote = (GameObject)Object.Instantiate(Resources.Load("Prefabs/MsgNote"));
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
                    Object.Destroy(MsgNote);
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
    /// 获取验证码接口（注册时）
    /// </summary>
    /// <param name="email">邮箱</param>
    public IEnumerator IF01_0(string email)
    {
        ShowLoadPage();
        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("type", 0);
        using (UnityWebRequest request = UnityWebRequest.Post(BasicInformation.HostIp+"/user/sendValidateCode", form))
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
                    GameObject SendBtn = GameObject.Find("RegisterPage/RegisterArea/View/Content/SendCode/Text");
                    SendBtn.GetComponent<Text>().text = "已发送";
                    SendBtn.GetComponent<Text>().color = Color.gray;
                }
                else if (code == -1)
                {
                    Text EmailNote = GameObject.Find("RegisterPage/RegisterArea/View/Content/EmailNote").GetComponent<Text>();
                    EmailNote.text = "邮箱已被注册";
                }
                else
                    ShowMsgNote("网络状态不佳");
            }
        }
        HideLoadPage();
    }

    /// <summary>
    /// 获取验证码接口（找回密码时）
    /// </summary>
    /// <param name="email">邮箱</param>
    public IEnumerator IF01_1(string email)
    {
        ShowLoadPage();
        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("type", 1);
        using (UnityWebRequest request = UnityWebRequest.Post(BasicInformation.HostIp+"/user/sendValidateCode", form))
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
                    GameObject.Find("PasswordFindPage1/FindArea/Email").GetComponent<InputField>().interactable = false;
                    GameObject SendBtn = GameObject.Find("PasswordFindPage1/FindArea/SendCode/Text");
                    SendBtn.GetComponent<Text>().text = "已发送";
                    SendBtn.GetComponent<Text>().color = Color.gray;
                }
                else if (code == -2)
                {
                    Text EmailNote = GameObject.Find("PasswordFindPage1/FindArea/EmailNote").GetComponent<Text>();
                    EmailNote.text = "邮箱未被注册";
                }
                else
                    ShowMsgNote("网络状态不佳");
            }
        }
        HideLoadPage();
    }

    /// <summary>
    /// 用户注册接口
    /// </summary>
    /// <param name="name">用户名</param>
    /// <param name="password">密码</param>
    /// <param name="gender">性别</param>
    /// <param name="age">年龄</param>
    /// <param name="intro">个人介绍</param>
    /// <param name="email">邮箱</param>
    /// <param name="code">验证码</param>
    public IEnumerator IF02(string name, string password, int gender, int age, string intro, string email, int code)
    {
        ShowLoadPage();
        WWWForm form = new WWWForm();
        form.AddField("name", name);
        form.AddField("password", password);
        form.AddField("gender", gender);
        form.AddField("age", age);
        form.AddField("email", email);
        form.AddField("code", code);
        form.AddField("introduction", intro);
        using (UnityWebRequest request = UnityWebRequest.Post(BasicInformation.HostIp+"/user/register", form))
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
                int codex = data.GetInt("code");
                if (codex == 0)
                {
                    ShowMsgNote("注册成功");
                    GameObject x = GameObject.Find("RegisterPage");
                    x.transform.DOMoveX(3 * BasicInformation.ScreenWidth / 2, 0.1f).OnComplete(() => { Object.Destroy(x); });
                    GameObject.Find("LoginPage/LoginArea/Email").GetComponent<InputField>().text = email;
                    GameObject.Find("LoginPage/LoginArea/Password").GetComponent<InputField>().text = password;
                }
                else
                {
                    Text CodeNote = GameObject.Find("RegisterPage/RegisterArea/View/Content/CodeNote").GetComponent<Text>();
                    CodeNote.text = "验证码错误";
                }
            }
        }
        HideLoadPage();
    }

    /// <summary>
    /// 用户登录接口
    /// </summary>
    /// <param name="email">邮箱</param>
    /// <param name="password">密码</param>
    public IEnumerator IF03(string email, string password)
    {
        ShowLoadPage();
        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("password", password);
        using (UnityWebRequest request = UnityWebRequest.Post(BasicInformation.HostIp+"/user/login", form))
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
                int codex = data.GetInt("code");
                if (codex == 0)
                {
                    JSON resData = data.GetJSON("resData");
                    BasicInformation.CurUser.Id = resData.GetInt("id");
                    BasicInformation.CurUser.Name = resData.GetString("name");
                    BasicInformation.CurUser.Gender = resData.GetInt("sex");
                    BasicInformation.CurUser.Age = resData.GetInt("age");
                    BasicInformation.CurUser.Introduce = resData.GetString("signature");
                    BasicInformation.CurUser.Email = resData.GetString("email");
                    BasicInformation.CurUser.TotLearnTime = resData.GetInt("studyTime");
                    BasicInformation.CurUser.RegistTime = resData.GetString("registerTime").Substring(0, 10).Replace('-', '.');
                    BasicInformation.CurUser.IsOnline = true;
                    BasicInformation.CurUser.HeadShot = resData.GetString("photo");
                    GameObject.Find("EventSystem").SendMessage("GetSignal");
                    BasicInformation.CurUser.UserRecordToLocal();
                    GameObject x = GameObject.Find("LoginPage");
                    x.transform.DOMoveX(3 * BasicInformation.ScreenWidth / 2, 0.1f).OnComplete(() => { Object.Destroy(x); });
                }
                else if (codex == -1)
                    ShowMsgNote("密码错误");
                else if (codex == -2)
                    ShowMsgNote("邮箱未注册");
            }
        }
        HideLoadPage();
    }

    /// <summary>
    /// 获取用户信息接口(个人）
    /// </summary>
    /// <param name="id">用户编号</param>
    public IEnumerator IF04_0(int id)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", id);
        using (UnityWebRequest request = UnityWebRequest.Post(BasicInformation.HostIp+"/user/findUser", form))
        {
            request.timeout = 5;
            yield return request.SendWebRequest();
            if (request.isHttpError || request.isNetworkError)
            {
                Debug.Log(request.error);
                SceneManager.LoadScene("MainPage");
                SceneManager.sceneLoaded += ((Scene scene, LoadSceneMode sceneType) =>
                {
                    ShowMsgNote("网络状态不佳");
                });
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
                JSON data = JSON.ParseString(request.downloadHandler.text);
                int code = data.GetInt("code");
                if (code == 0)
                {
                    JSON resData = data.GetJSON("resData");
                    BasicInformation.CurUser.Id = resData.GetInt("id");
                    BasicInformation.CurUser.Name = resData.GetString("name");
                    BasicInformation.CurUser.Gender = resData.GetInt("sex");
                    BasicInformation.CurUser.Age = resData.GetInt("age");
                    BasicInformation.CurUser.Introduce = resData.GetString("signature");
                    BasicInformation.CurUser.Email = resData.GetString("email");
                    BasicInformation.CurUser.TotLearnTime = resData.GetInt("studyTime");
                    BasicInformation.CurUser.RegistTime = resData.GetString("registerTime").Substring(0, 10).Replace('-','.');
                    BasicInformation.CurUser.HeadShot = resData.GetString("photo");
                    BasicInformation.CurUser.IsOnline = true;
                    SceneManager.LoadScene("MainPage");
                    SceneManager.sceneLoaded += ((Scene scene, LoadSceneMode sceneType) =>
                    {
                        GameObject.Find("EventSystem").SendMessage("GetSignal");
                    });
                }
                else
                {
                    SceneManager.LoadScene("MainPage");
                    SceneManager.sceneLoaded += ((Scene scene, LoadSceneMode sceneType) =>
                    {
                        ShowMsgNote("网络状态不佳");
                    });
                }
            }
        }
    }

    /// <summary>
    /// 获取用户信息接口(好友）
    /// </summary>
    /// <param name="id">用户编号</param>
    public IEnumerator IF04_1(int id)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", id);
        using (UnityWebRequest request = UnityWebRequest.Post(BasicInformation.HostIp+"/user/findUser", form))
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
                int codex = data.GetInt("code");
                if (codex == 0)
                {
                    JSON resData = data.GetJSON("resData");
                    GameObject.Find("UserInfoPage").SendMessage("GetUserInfo", new User(
                        resData.GetInt("id"),
                        resData.GetString("name"),
                        resData.GetInt("sex"),
                        resData.GetInt("age"),
                        resData.GetString("signature"),
                        resData.GetString("registerTime").Substring(0, 10).Replace('-', '.'),
                        resData.GetInt("studyTime"),
                        "",
                        resData.GetString("photo")
                    ));
                }
            }
        }
    }

    /// <summary>
    /// 获取用户信息接口(聊天）
    /// </summary>
    /// <param name="id">用户编号</param>
    public IEnumerator IF04_2(int id)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", id);
        using (UnityWebRequest request = UnityWebRequest.Post(BasicInformation.HostIp+"/user/findUser", form))
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
                int codex = data.GetInt("code");
                if (codex == 0)
                {
                    JSON resData = data.GetJSON("resData");
                    GameObject.Find("StarStudyPage").SendMessage("GetUserInfo", new User(
                        resData.GetInt("id"),
                        resData.GetString("name"), 0, 0, "", "", 0, "", ""
                    ));
                }
            }
        }
    }

    /// <summary>
    /// 修改用户信息接口
    /// </summary>
    /// <param name="id">用户编号</param>
    /// <param name="type">修改类型（0用户名、1性别、2年龄、3个人介绍）</param>
    /// <param name="content">修改内容</param>
    public IEnumerator IF05(int id, int type, string content)
    {
        ShowLoadPage();
        WWWForm form = new WWWForm();
        form.AddField("id", id);
        form.AddField("type", type);
        form.AddField("content", content);
        using (UnityWebRequest request = UnityWebRequest.Post(BasicInformation.HostIp+"/user/modifyUserInfo", form))
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
                int codex = data.GetInt("code");
                if (codex == 0)
                {
                    string PersonPageArea = "PersonalPage/PersonArea/View/Content";
                    GameObject x = GameObject.Find("InfoChangePage~");
                    Object.Destroy(x);
                    if (type == 0)
                    {
                        BasicInformation.CurUser.Name = content;
                        GameObject.Find("Page3/View/Content/UserInfo/UserName").GetComponent<Text>().text = content;
                        GameObject.Find(PersonPageArea + "/Name/Text").GetComponent<Text>().text = content;
                    }
                    else if (type == 1)
                    {
                        BasicInformation.CurUser.Gender = content == "男" ? 0 : 1;
                        GameObject.Find(PersonPageArea + "/Gender/Text").GetComponent<Text>().text = content == "0" ? "男" : "女";
                    }
                    else if (type == 2)
                    {
                        BasicInformation.CurUser.Age = int.Parse(content);
                        GameObject.Find(PersonPageArea + "/Age/Text").GetComponent<Text>().text = content;
                    }
                    else
                    {
                        BasicInformation.CurUser.Introduce = content;
                        GameObject.Find(PersonPageArea + "/Signature/Text").GetComponent<Text>().text = content.Length > 12 ? content.Substring(0, 12) + "..." : content;
                    }
                    ShowMsgNote("修改成功");
                }
                else
                    ShowMsgNote("网络状态不佳");
            }
        }
        HideLoadPage();
    }

    /// <summary>
    /// 修改密码接口
    /// </summary>
    /// <param name="id">用户编号</param>
    /// <param name="oldp">旧密码</param>
    /// <param name="newp">新密码</param>
    public IEnumerator IF06(int id, string oldp, string newp)
    {
        ShowLoadPage();
        WWWForm form = new WWWForm();
        form.AddField("id", id);
        form.AddField("oldPassword", oldp);
        form.AddField("newPassword", newp);
        using (UnityWebRequest request = UnityWebRequest.Post(BasicInformation.HostIp+"/user/changePassword", form))
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
                int codex = data.GetInt("code");
                if (codex == 0)
                {
                    GameObject x = GameObject.Find("PassChangePage~");
                    Object.Destroy(x);
                    ShowMsgNote("修改成功");
                }
                else if (codex == -1)
                {
                    Text Note1 = GameObject.Find("PassChangePage~/ChangeArea/Note1").GetComponent<Text>();
                    Note1.text = "原密码错误";
                }
                else
                    ShowMsgNote("网络状态不佳");
            }
        }
        HideLoadPage();
    }

    /// <summary>
    /// 找回密码（核对验证码）接口
    /// </summary>
    /// <param name="email">邮箱</param>
    /// <param name="code">验证码</param>
    public IEnumerator IF07(string email, int code)
    {
        ShowLoadPage();
        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("code", code);
        using (UnityWebRequest request = UnityWebRequest.Post(BasicInformation.HostIp+"/user/validateCode", form))
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
                int codex = data.GetInt("code");
                if (codex == 0)
                {
                    GameObject x = GameObject.Find("PasswordFindPage1");
                    x.transform.DOMoveX(3 * BasicInformation.ScreenWidth / 2, 0.1f).OnComplete(() => { Object.Destroy(x); });
                    GameObject FindPage2 = (GameObject)Object.Instantiate(Resources.Load("Prefabs/UserModule/PasswordFindPage2"));
                    FindPage2.name = "PasswordFindPage2";
                    FindPage2.AddComponent<FindPassController>();
                    FindPage2.SendMessage("GetMsg", 2);
                    FindPage2.SendMessage("GetEmail", email);
                }
                else
                {
                    Text CodeNote = GameObject.Find("PasswordFindPage1/FindArea/CodeNote").GetComponent<Text>();
                    CodeNote.text = "验证码错误";
                }
            }
        }
        HideLoadPage();
    }

    /// <summary>
    /// 找回密码（修改密码）接口
    /// </summary>
    /// <param name="email">邮箱</param>
    /// <param name="newp">新密码</param>
    public IEnumerator IF08(string email, string newp)
    {
        ShowLoadPage();
        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("newPassword", newp);
        using (UnityWebRequest request = UnityWebRequest.Post(BasicInformation.HostIp+"/user/findPassword", form))
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
                int codex = data.GetInt("code");
                if (codex == 0)
                {
                    ShowMsgNote("修改成功");
                    GameObject x = GameObject.Find("PasswordFindPage2");
                    x.transform.DOMoveX(3 * BasicInformation.ScreenWidth / 2, 0.1f).OnComplete(() => { Object.Destroy(x); });
                }
                else
                    ShowMsgNote("网络状态不佳");
            }
        }
        HideLoadPage();
    }

    /// <summary>
    /// 发送私信接口
    /// </summary>
    /// <param name="id1">发送者编号</param>
    /// <param name="id2">接收者编号</param>
    /// <param name="content">私信内容</param>
    public IEnumerator IF09(int id1, int id2, string content)
    {
        ShowLoadPage();
        WWWForm form = new WWWForm();
        form.AddField("senderId", id1);
        form.AddField("receiverId", id2);
        form.AddField("content", content);
        using (UnityWebRequest request = UnityWebRequest.Post(BasicInformation.HostIp+"/user/leaveMessage", form))
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
                int codex = data.GetInt("code");
                if (codex == 0)
                {
                    ShowMsgNote("发送成功");
                    Object.Destroy(GameObject.Find("SendMessagePage~"));
                }
                else
                    ShowMsgNote("网络状态不佳");
            }
        }
        HideLoadPage();
    }

    /// <summary>
    /// 查看留言列表接口
    /// </summary>
    /// <param name="id">用户编号</param>
    public IEnumerator IF10(int id)
    {
        ShowLoadPage();
        WWWForm form = new WWWForm();
        form.AddField("id", id);
        using (UnityWebRequest request = UnityWebRequest.Post(BasicInformation.HostIp+"/user/queryAllMessage", form))
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
                int codex = data.GetInt("code");
                if (codex == 0)
                {
                    JArray messages = data.GetJArray("resData");
                    List<Message> messagelist = new List<Message>();
                    for (int i = 0; i < messages.Length; i++)
                    {
                        JSON message = JSON.ParseString(messages[i].CreateString());
                        messagelist.Add(new Message(message.GetInt("senderId"), message.GetString("photo"), message.GetString("content"), message.GetString("createTime")));
                    }
                    GameObject MessageListPage = (GameObject)Object.Instantiate(Resources.Load("Prefabs/UserModule/MessageListPage"));
                    MessageListPage.name = "MessageListPage";
                    MessageListPage.AddComponent<MessageListController>();
                    MessageListPage.SendMessage("GetMessages", messagelist);
                }
                else if (codex == -1) {
                    GameObject MessageListPage = (GameObject)Object.Instantiate(Resources.Load("Prefabs/UserModule/MessageListPage"));
                    MessageListPage.name = "MessageListPage";
                    MessageListPage.AddComponent<MessageListController>();
                }
                else
                    ShowMsgNote("网络状态不佳");
            }
        }
        HideLoadPage();
    }

    /// <summary>
    /// 清空留言列表接口
    /// </summary>
    /// <param name="id">用户编号</param>
    public IEnumerator IF11(int id)
    {
        ShowLoadPage();
        WWWForm form = new WWWForm();
        form.AddField("id", id);
        using (UnityWebRequest request = UnityWebRequest.Post(BasicInformation.HostIp+"/user/clearMessage", form))
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
                int codex = data.GetInt("code");
                if (codex == 0)
                {
                    ShowMsgNote("清空成功");
                    GameObject.Find("MessageListPage").GetComponent<MessageListController>().SendMessage("GetClear");
                }
                else
                    ShowMsgNote("网络状态不佳");
            }
        }
        HideLoadPage();
    }

    /// <summary>
    /// 上传头像接口
    /// </summary>
    /// <param name="id">用户id</param>
    /// <param name="image">图片二进制</param>
    public IEnumerator IF12(int id, byte[] image)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", id);
        form.AddBinaryData("data", image);
        using (UnityWebRequest request = UnityWebRequest.Post(BasicInformation.HostIp+"/user/changeProfile", form))
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
                int codex = data.GetInt("code");
                if (codex == 0)
                {
                    BasicInformation.CurUser.HeadShot =  data.GetString("resData");
                    GameObject.Find("EventSystem").SendMessage("GetHeadshotUpdate");
                    GameObject.Find("PersonalPage").SendMessage("GetHeadshotUpdate");
                }
                else
                {
                    Text CodeNote = GameObject.Find("PasswordFindPage1/FindArea/CodeNote").GetComponent<Text>();
                    CodeNote.text = "验证码错误";
                }
            }
        }
    }
}