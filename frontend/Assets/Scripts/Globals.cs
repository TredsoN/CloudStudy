using System;
using System.Collections.Generic;
using UnityEngine;

public static class BasicInformation
{
    public static float ScreenWidth = 1080;

    public static float ScreenHeight = 2340;

    public static User CurUser = new User();

    public static UserInterface UserInterface = new UserInterface();

    public static StarInterface StarInterface = new StarInterface();

    public static OtherInterface OtherInterface = new OtherInterface();

    public static string HostIp = "";

    public static string AgoraId = "";
}

/// <summary>
/// 用户类
/// </summary>
public class User
{
    public bool IsOnline { get; set; }

    public int Id { get; set; }

    public string Name { get; set; }

    public int Gender { get; set; }

    public int Age { get; set; }

    public string HeadShot { get; set; }

    public string Email { get; set; }

    public string Introduce { get; set; }

    public string RegistTime { get; set; }

    public int TotLearnTime { get; set; }

    public string Status { get; set; }

    public void UserRecordToLocal()
    {
        PlayerPrefs.SetInt("UserId", Id);
    }

    public User() { }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="id">用户id</param>
    /// <param name="name">昵称</param>
    /// <param name="gender">性别</param>
    /// <param name="age">年龄</param>
    /// <param name="headShot">头像地址</param>
    /// <param name="introduce">个人介绍</param>
    /// <param name="registTime">注册时间</param>
    /// <param name="totLearnTime">总学习时间</param>
    /// <param name="status">当前状态</param>
    public User(int id, string name, int gender, int age, string introduce, string registTime, int totLearnTime, string status, string headshot)
    {
        Id = id;
        Name = name;
        Gender = gender;
        Age = age;
        Introduce = introduce;
        RegistTime = registTime;
        TotLearnTime = totLearnTime;
        Status = status;
        HeadShot = headshot;
    }
}

/// <summary>
/// 星球类
/// </summary>
public class Star
{
    public int Id { get; set; }

    public string StarId { get; set; }

    public int CreatorId { get; set; }

    public string Name { get; set; }

    public int Type { get; set; }

    public int Galaxy { get; set; }

    public string Introduce { get; set; }

    public int TotMembers { get; set; }

    public string CreateTime { get; set; }

    public string Password { get; set; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="id">星球id</param>
    /// <param name="starId">星球编号</param>
    /// <param name="creatorId">创建者id</param>
    /// <param name="name">星球名</param>
    /// <param name="type">星球类型</param>
    /// <param name="galaxy">星球所在星系</param>
    /// <param name="introduce">星球简介</param>
    /// <param name="totMembers">星球人数</param>
    /// <param name="createTime">创建时间</param>
    public Star(int id, string starId, int creatorId, string name, int type, int galaxy, string introduce, int totMembers, string createTime)
    {
        Id = id;
        StarId = starId;
        CreatorId = creatorId;
        Name = name;
        Type = type;
        Galaxy = galaxy;
        Introduce = introduce;
        TotMembers = totMembers;
        CreateTime = createTime;
    }

    public Star(int id, string starId, int creatorId, string name, int type, int galaxy, string introduce, int totMembers, string createTime, string password) : this(id, starId, creatorId, name, type, galaxy, introduce, totMembers, createTime)
    {
        Password = password;
    }
}

/// <summary>
/// 用于保存消息内容
/// </summary>
public class Message
{
    public int SenderId { get; set; }

    public string SenderHeadShot { get; set; }

    public string Content { get; set; }

    public string SendTime { get; set; }

    public Message(int senderId, string senderHeadShot, string content, string sendTime)
    {
        SenderId = senderId;
        SenderHeadShot = senderHeadShot;
        Content = content;
        SendTime = sendTime;
    }
}

/// <summary>
/// 学习记录
/// </summary>
public class Record
{
    public int Id { get; set; }

    public string Time { get; set; }

    public string HeadShot { get; set; }

    public string State { get; set; }

    public Record(int id, string time, string headShot, string state)
    {
        Id = id;
        Time = time;
        HeadShot = headShot;
        State = state;
    }
}

/// <summary>
/// 直播弹幕
/// </summary>
class BroadMessage
{
    public string name { get; set; }
    public string content { get; set; }
    public string headshot { get; set; }
    public int userid { get; set; }

    public BroadMessage(string name, string content, string headshot, int userid)
    {
        this.name = name;
        this.content = content;
        this.headshot = headshot;
        this.userid = userid;
    }
}

/// <summary>
/// 倒计时
/// </summary>
public class CntDown
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Remark { get; set; }

    public DateTime EndTime { get; set; }

    public int Span { get; set; }

    public CntDown(int id, string name, string remark, DateTime endTime)
    {
        Id = id;
        Name = name;
        Remark = remark;
        EndTime = endTime;
        Span = (DateTime.Now - endTime).Days;
    }

    public void Refresh()
    {
        Span = (DateTime.Now - EndTime).Days;
    }
}

/// <summary>
/// 待办清单
/// </summary>
public class TodoList
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Time { get; set; }

    public List<TodoListItem> Items { get; set; }

    public TodoList(int id, string name, string time)
    {
        Id = id;
        Name = name;
        Time = time;
        Items = new List<TodoListItem>();
    }

    public int GetUnDone()
    {
        int count = 0;
        foreach (TodoListItem i in Items)
            if (!i.IsChecked)
                count++;
        return count;
    }
}

/// <summary>
/// 清单条目
/// </summary>
public class TodoListItem
{
    public int Id { get; set; }

    public string Content { get; set; }

    public int Prior { get; set; }

    public bool IsChecked { get; set; }

    public TodoListItem(int id, string content, int prior, bool isChecked)
    {
        Id = id;
        Content = content;
        Prior = prior;
        IsChecked = isChecked;
    }
}

/// <summary>
/// 用于各类输入文本的校验
/// </summary>
public class TextValidator
{
    public bool EmailTest(string text)
    {
        string expression = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
        return System.Text.RegularExpressions.Regex.IsMatch(text, expression) ? true : false;
    }

    public bool NameTest(string text)
    {
        return text.Length <= 8 ? true : false;
    }

    public bool PasswordTest(string text)
    {
        return text.Length >= 8 && text.Length <= 16 ? true : false;
    }

    public bool CodeTest(string text)
    {
        return text.Length == 6 ? true : false;
    }

    public bool AgeTest(string text)
    {
        try
        {
            int age = int.Parse(text);
            return age < 100 ? true : false;
        }
        catch
        {
            return false;
        }
    }

    public bool SigTest(string text)
    {
        return text.Length <= 45 ? true : false;
    }
}