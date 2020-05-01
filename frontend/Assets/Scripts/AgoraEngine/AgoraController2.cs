using agora_gaming_rtc;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgoraController2
{
    private IRtcEngine mRtcEngine;

    /// <summary>
    /// 创建引擎
    /// </summary>
    /// <param name="appId">appId</param>
    public void createEngine(string appId)
    {
        if (mRtcEngine == null)
        {
            mRtcEngine = IRtcEngine.GetEngine(appId);
            mRtcEngine.EnableVideo();
            mRtcEngine.EnableVideoObserver();
            //设置回调函数
            mRtcEngine.OnJoinChannelSuccess = onJoinSuccess;
            mRtcEngine.OnUserJoined = onUserJoined;
        }
    }

    /// <summary>
    /// 销毁引擎
    /// </summary>
    public void destroyEngine()
    {
        if (mRtcEngine != null)
        {
            IRtcEngine.Destroy();
            mRtcEngine = null;
        }
    }

    /// <summary>
    /// 加入频道
    /// </summary>
    /// <param name="channelName">频道名</param>
    public void joinChannel(string channelName, uint userID, bool isAnchor)
    {
        if (mRtcEngine != null)
        {
            mRtcEngine.SetChannelProfile(CHANNEL_PROFILE.CHANNEL_PROFILE_LIVE_BROADCASTING);
            if (isAnchor)
                mRtcEngine.SetClientRole(CLIENT_ROLE.BROADCASTER);
            else
                mRtcEngine.SetClientRole(CLIENT_ROLE.AUDIENCE);
            mRtcEngine.JoinChannel(channelName, null, userID);
        }
    }

    /// <summary>
    /// 离开频道
    /// </summary>
    public void leaveChannel()
    {
        if (mRtcEngine != null)
            mRtcEngine.LeaveChannel();
    }

    /// <summary>
    /// 切换摄像头
    /// </summary>
    public void switchCamera()
    {
        if (mRtcEngine != null)
        {
            mRtcEngine.SwitchCamera();
        }
    }

    /// <summary>
    /// 加入成功回调函数
    /// </summary>
    private void onJoinSuccess(string channelName, uint uid, int elapsed)
    {
        GameObject.Find("StarStudyPage").SendMessage("GetMeJoinSuccess");
    }

    /// <summary>
    /// 其他用户加入聊天室回调函数
    /// </summary>
    private void onUserJoined(uint uid, int elapsed)
    {
        GameObject.Find("StarStudyPage").SendMessage("GetUserJoined", uid);
    }
}
