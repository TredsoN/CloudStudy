using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CreateStarContoller : MonoBehaviour
{
    int Galaxy, Type;

    /// <summary>
    /// 脚本加载
    /// </summary>
    private void Awake()
    {
        GameObject CreateStarPage = GameObject.Find("CreateStarPage~");
        CreateStarPage.transform.SetParent(GameObject.Find("Canvas").transform);
        CreateStarPage.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenHeight);
        CreateStarPage.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
        GameObject.Find("CreateStarPage~/InfoArea/Back").GetComponent<Button>().onClick.AddListener(() => { OnReturnClicked(); });
    }

    /// <summary>
    /// 接收星系信息
    /// </summary>
    public void GetGalaxy(int galaxy)
    {
        Galaxy = galaxy;
    }

    /// <summary>
    /// 接收星球种类
    /// </summary>
    public void GetType(int type)
    {
        Type = type;
        GameObject.Find("CreateStarPage~/InfoArea/Submit").GetComponent<Button>().onClick.AddListener(() => { OnSubmitClicked(Type, Galaxy); });
    }

    /// <summary>
    /// 返回
    /// </summary>
    private void OnReturnClicked()
    {
        Destroy(GameObject.Find("CreateStarPage~"));
    }

    /// <summary>
    /// 提交
    /// </summary>
    /// <param name="type"></param>
    private void OnSubmitClicked(int type, int glaxy)
    {
        Text Note1 = GameObject.Find("CreateStarPage~/InfoArea/Note1").GetComponent<Text>(),
            Note2 = GameObject.Find("CreateStarPage~/InfoArea/Note2").GetComponent<Text>();
        string Name = GameObject.Find("CreateStarPage~/InfoArea/Name").GetComponent<InputField>().text,
                Intro = GameObject.Find("CreateStarPage~/InfoArea/Introduce").GetComponent<InputField>().text;
        if (type == 0 || type == 2)
        {
            if (Name == "")
                Note1.text = "星球名称不能为空";
            else if (Name.Length > 15)
                Note1.text = "星球名称不能超过15位";
            else
            {
                Note1.text = "";
                if (Intro == "")
                    Note2.text = "星球简介不能为空";
                else if (Intro.Length > 25)
                    Note2.text = "星球简介不能超过25位";
                else
                {
                    Note2.text = "";
                    StartCoroutine(BasicInformation.StarInterface.IF01(BasicInformation.CurUser.Id, Name, Intro, Galaxy, type, -1));
                }
            }
        }
        else if (type == 1)
        {
            Text Note3 = GameObject.Find("CreateStarPage~/InfoArea/Note3").GetComponent<Text>();
            string Code = GameObject.Find("CreateStarPage~/InfoArea/Code").GetComponent<InputField>().text;
            if (Name == "")
                Note1.text = "星球名称不能为空";
            else if (Name.Length > 15)
                Note1.text = "星球名称不能超过15位";
            else
            {
                Note1.text = "";
                if (Intro == "")
                    Note2.text = "星球简介不能为空";
                else if (Intro.Length > 25)
                    Note2.text = "星球简介不能超过25位";
                else
                {
                    Note2.text = "";
                    if (Code == "")
                        Note3.text = "星球暗号不能为空";
                    else if (Code.Length != 6)
                        Note3.text = "星球暗号必须为6位";
                    else
                    {
                        Note3.text = "";
                        StartCoroutine(BasicInformation.StarInterface.IF01(BasicInformation.CurUser.Id, Name, Intro, Galaxy, type, int.Parse(Code)));
                    }
                }
            }
        }
    }
}
