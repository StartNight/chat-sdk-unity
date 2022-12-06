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
using System.Text;
using System.Collections.Generic;
public class MsgGetMessageExtensions : MonoBehaviour
{

  public Text Header;
  public InputField MsgID;
  public Text Result;
  public Button Submit;
  public Button Copy;
  void Start()
  {
    Header = GameObject.Find("HeaderText").GetComponent<Text>();
    MsgID = GameObject.Find("MsgID").GetComponent<InputField>();
    Result = GameObject.Find("ResultText").GetComponent<Text>();
    Submit = GameObject.Find("Submit").GetComponent<Button>();
    Copy = GameObject.Find("Copy").GetComponent<Button>();
    Submit.onClick.AddListener(MsgFindMessages);
    Copy.GetComponentInChildren<Text>().text = Utils.t("Copy");
    Copy.onClick.AddListener(CopyText);
    if (CurrentSceneInfo.info != null)
    {
      Header.text = Utils.IsCn() ? CurrentSceneInfo.info.apiText + " " + CurrentSceneInfo.info.apiName : CurrentSceneInfo.info.apiName;
      Submit.GetComponentInChildren<Text>().text = CurrentSceneInfo.info.apiName;
    }
  }
  void MsgFindMessages()
  {
    TIMResult res = TencentIMSDK.MsgFindMessages(new List<string> { MsgID.text }, Utils.addAsyncStringDataToScreen(GetMessage));
    print("MsgFindMessages: " + res);
  }

  void MsgGetMessageExtensionsSDK(Message msg)
  {
    TIMResult res = TencentIMSDK.MsgGetMessageExtensions(msg, Utils.addAsyncStringDataToScreen(GetResult));
    Result.text = Utils.SynchronizeResult(res);
  }

  void GetMessage(params object[] parameters)
  {
    string text = (string)parameters[1];
    print(text);
    var list = Utils.FromJson<List<Message>>(text);
    if (list.Count > 0)
    {
      MsgGetMessageExtensionsSDK(list[0]);
    }
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