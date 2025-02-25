using UnityEngine;
using UnityEngine.UI;
using com.tencent.im.unity.demo.types;
using com.tencent.imsdk.unity;
using com.tencent.imsdk.unity.types;
using com.tencent.imsdk.unity.enums;
using System;
using com.tencent.im.unity.demo.utils;
using EasyUI.Toast;
using System.Collections;
using System.Collections.Generic;
public class ConvMarkConversation : MonoBehaviour
{
  public Text Header;
  public Dropdown SelectedConv;
  public Text Result;
  public Dropdown SelectedMarkType;
  public Toggle IsEnable;
  public Button Submit;
  public Button Copy;
  public int[] EnumConvMarkType = (int[])Enum.GetValues(typeof(TIMConversationMarkType));
  private List<ConvInfo> ConvList;
  void Start()
  {
    ConvGetConvListSDK();
    Header = GameObject.Find("HeaderText").GetComponent<Text>();
    SelectedConv = GameObject.Find("Dropdown").GetComponent<Dropdown>();
    SelectedMarkType = GameObject.Find("MarkType").GetComponent<Dropdown>();
    foreach (string name in Enum.GetNames(typeof(TIMConversationMarkType)))
    {
      Dropdown.OptionData option = new Dropdown.OptionData();
      option.text = name;
      SelectedMarkType.options.Add(option);
    }
    IsEnable = GameObject.Find("IsEnable").GetComponent<Toggle>();
    Result = GameObject.Find("ResultText").GetComponent<Text>();
    Submit = GameObject.Find("Submit").GetComponent<Button>();
    Copy = GameObject.Find("Copy").GetComponent<Button>();
    Copy.GetComponentInChildren<Text>().text = Utils.t("Copy");
    Submit.onClick.AddListener(ConvMarkConversationSDK);
    Copy.onClick.AddListener(CopyText);
    SelectedConv.interactable = true;
    if (CurrentSceneInfo.info != null)
    {
      Header.text = Utils.IsCn() ? CurrentSceneInfo.info.apiText + " " + CurrentSceneInfo.info.apiName : CurrentSceneInfo.info.apiName;
      Submit.GetComponentInChildren<Text>().text = CurrentSceneInfo.info.apiName;
    }
  }

  void GetConvList(params object[] parameters)
  {
    try
    {
      ConvList = new List<ConvInfo>();
      SelectedConv.ClearOptions();
      string text = (string)parameters[1];
      List<ConvInfo> List = Utils.FromJson<List<ConvInfo>>(text);
      foreach (ConvInfo item in List)
      {
        print(item.conv_id);
        ConvList.Add(item);
        Dropdown.OptionData option = new Dropdown.OptionData();
        option.text = item.conv_id;
        SelectedConv.options.Add(option);
      }
      if (List.Count > 0)
      {
        SelectedConv.captionText.text = List[SelectedConv.value].conv_id;
      }
    }
    catch (Exception ex)
    {
      Toast.Show(Utils.t("getConvListFailed"));
    }
  }

  void ConvGetConvListSDK()
  {
    TIMResult res = TencentIMSDK.ConvGetConvList(Utils.addAsyncStringDataToScreen(GetConvList));
    print($"ConvGetConvListSDK {res}");
  }

  void ConvMarkConversationSDK()
  {
    if (ConvList.Count == 0)
    {
      return;
    }

    print(ConvList[SelectedConv.value].conv_id);
    string conv_id = ConvList[SelectedConv.value].conv_id;

    if (!Utils.IsValidConvID(conv_id))
    {
      conv_id = Utils.SetConvIDPrefix(conv_id, ConvList[SelectedConv.value].conv_type);
    }

    TIMResult res = TencentIMSDK.ConvMarkConversation(new List<string> {
      conv_id
    }, (TIMConversationMarkType)EnumConvMarkType[SelectedMarkType.value], IsEnable.isOn, Utils.addAsyncStringDataToScreen(GetResult));
    Result.text = Utils.SynchronizeResult(res);
  }

  void GetResult(params object[] parameters)
  {
    Result.text += (string)parameters[0];
  }

  void CopyText()
  {
    Utils.Copy(Result.text);
  }
  void OnApplicationQuit()
  {
    TencentIMSDK.Uninit();
  }
}