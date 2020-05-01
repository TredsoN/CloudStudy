using UnityEngine;
using UnityEngine.UI;

public class SelectTypeController : MonoBehaviour
{
    int Galaxy;

    /// <summary>
    /// 脚本加载
    /// </summary>
    private void Awake()
    {
        GameObject SelectTypePage = GameObject.Find("SelectTypePage~");
        SelectTypePage.transform.SetParent(GameObject.Find("Canvas").transform);
        SelectTypePage.GetComponent<RectTransform>().sizeDelta = new Vector2(BasicInformation.ScreenWidth, BasicInformation.ScreenHeight);
        SelectTypePage.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
        GameObject.Find("SelectTypePage~/SelectArea/Type0").GetComponent<Button>().onClick.AddListener(() => { OnCreateClicked(0); });
        GameObject.Find("SelectTypePage~/SelectArea/Type1").GetComponent<Button>().onClick.AddListener(() => { OnCreateClicked(1); });
        GameObject.Find("SelectTypePage~/SelectArea/Type2").GetComponent<Button>().onClick.AddListener(() => { OnCreateClicked(2); });
        GameObject.Find("SelectTypePage~/SelectArea/Back").GetComponent<Button>().onClick.AddListener(() => { OnReturnClicked(); });
    }

    /// <summary>
    /// 接收初始化信息
    /// </summary>
    public void GetMsg(int id)
    {
        Galaxy = id;
    }

    /// <summary>
    /// 返回
    /// </summary>
    private void OnReturnClicked()
    {
        Destroy(GameObject.Find("SelectTypePage~"));
    }

    /// <summary>
    /// 跳转创建星球界面
    /// </summary>
    /// <param name="type">星球种类</param>
    private void OnCreateClicked(int type)
    {
        Destroy(GameObject.Find("SelectTypePage~"));
        GameObject CreateStar = (GameObject)Instantiate(Resources.Load("Prefabs/StarModule/CreateStar" + type));
        CreateStar.name = "CreateStarPage~";
        CreateStar.AddComponent<CreateStarContoller>();
        CreateStar.SendMessage("GetGalaxy", Galaxy);
        CreateStar.SendMessage("GetType", type);
    }
}
