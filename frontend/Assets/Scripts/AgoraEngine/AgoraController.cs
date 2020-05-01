using agora_gaming_rtc;
using UnityEngine;
using UnityEngine.UI;

public class AgoraController
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
            mRtcEngine.OnUserOffline = onUserOffline;
            mRtcEngine.OnUserMutedAudio = onUserMuteAudio;
            mRtcEngine.OnUserMuteVideo = onUserMuteVideo;
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
    public void joinChannel(string channelName, uint userID)
    {
        if (mRtcEngine != null)
            mRtcEngine.JoinChannel(channelName, null, userID);
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
    /// 控制本地音频
    /// </summary>
    /// <param name="mute">true为关闭，false为打开</param>
    public void muteLocalAudio(bool mute)
    {
        if (mRtcEngine != null)
            mRtcEngine.MuteLocalAudioStream(mute);
    }

    /// <summary>
    /// 控制本地视频
    /// </summary>
    /// <param name="mute">true为关闭，false为打开</param>
    public void muteLocalVideo(bool mute)
    {
        if (mRtcEngine != null)
            mRtcEngine.MuteLocalVideoStream(mute);
    }

    /// <summary>
    /// 切换扬声器
    /// </summary>
    /// <param name="mode">true为外放，false为听筒</param>
    public void switchSpeaker(bool mode)
    {
        if (mRtcEngine != null)
            mRtcEngine.SetEnableSpeakerphone(mode);
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
        GameObject.Find("StarStudyPage").SendMessage("GetUserJoined", (int)uid);
    }

    /// <summary>
    /// 其他用户离开聊天室回调函数
    /// </summary>
    private void onUserOffline(uint uid, USER_OFFLINE_REASON reason)
    {
        GameObject.Find("StarStudyPage").SendMessage("GetUserLeft", (int)uid);
    }

    /// <summary>
    /// 其他用户静音回调函数
    /// </summary>
    private void onUserMuteAudio(uint uid, bool muted)
    {
        if (muted)
            GameObject.Find("StarStudyPage").SendMessage("GetUserMuteAudio", (int)uid);
        else
            GameObject.Find("StarStudyPage").SendMessage("GetUserUnMuteAudio", (int)uid);
    }

    /// <summary>
    /// 其他用户关闭视频回调函数
    /// </summary>
    private void onUserMuteVideo(uint uid, bool muted)
    {
        if (muted)
            GameObject.Find("StarStudyPage").SendMessage("GetUserMuteVideo", (int)uid);
        else
            GameObject.Find("StarStudyPage").SendMessage("GetUserUnMuteVideo", (int)uid);
    }
}
