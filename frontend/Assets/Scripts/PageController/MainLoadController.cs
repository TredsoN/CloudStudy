using DG.Tweening;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;

public class MainLoadController : MonoBehaviour
{
    public GameObject Title1, Title2;

    int ID = -1;

    /// <summary>
    /// 界面开始
    /// </summary>
    private void Start()
    {
        BasicInformation.ScreenHeight = Screen.height;
        BasicInformation.ScreenWidth = Screen.width;
        Invoke("StartRequest", 0.5f);
    }

    /// <summary>
    /// 界面更新
    /// </summary>
    private void Update()
    {
        if (ID == 0)
        {
            Permission.RequestUserPermission(Permission.Camera);
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
            Permission.RequestUserPermission(Permission.Microphone);
            if (Permission.HasUserAuthorizedPermission(Permission.Camera) && Permission.HasUserAuthorizedPermission(Permission.Microphone) && Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
                ID = 1;
        }
        if (ID == 1)
        {
            ID = -1;
            if (PlayerPrefs.HasKey("UserId"))
                Invoke("LoadMainPageWithInfo", 1.0f);
            else
                Invoke("LoadMainPage", 1.0f);
        }
    }

    /// <summary>
    /// 开始请求权限
    /// </summary>
    private void StartRequest()
    {
        ID = 0;
    }

    /// <summary>
    /// 直接加载主界面
    /// </summary>
    private void LoadMainPage()
    {
        SceneManager.LoadScene("MainPage");
    }

    /// <summary>
    /// 获取网络信息后加载主界面
    /// </summary>
    private void LoadMainPageWithInfo()
    {
        StartCoroutine(BasicInformation.UserInterface.IF04_0(PlayerPrefs.GetInt("UserId")));
    }
}