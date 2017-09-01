using UnityEngine;
using System.Collections;
using System.Text;
using System;
using ProtoBuf;
using UnityEngine.UI;


public class Client : MonoBehaviour {

    public Transform BtnRoot;

    void Start()
    {
        RegeditControl();
    }
    

    void OnEnable()
    {
        MessageCenter.Instance.AddEventListener(GameLogicEventType.NoticeInfo, CallBackPoseEvent);
        MessageCenter.Instance.addObsever(ProtocalCommand.scprotobuflogin, CallBackProtoBuffLoginServer);
        MessageCenter.Instance.addObsever(ProtocalCommand.scbinarylogin, CallBackBinaryLoginServer);
    }

    void OnDisable()
    {
        MessageCenter.Instance.RemoveEventListener(GameLogicEventType.NoticeInfo, CallBackPoseEvent);
        MessageCenter.Instance.removeObserver(ProtocalCommand.scprotobuflogin, CallBackProtoBuffLoginServer);
        MessageCenter.Instance.removeObserver(ProtocalCommand.scbinarylogin, CallBackBinaryLoginServer);
    }

    void OnApplicationQuit()
    {
        SocketManager.Instance.Close();
    }

    private void RegeditControl()
    {
        BtnRoot.FindChild("BtnConnect").GetComponent<Button>().onClick.AddListener(OnButtonConnect);
        BtnRoot.FindChild("BtnDisConnect").GetComponent<Button>().onClick.AddListener(OnButtonDisConnect);
        BtnRoot.FindChild("BtnPostEventNoticeInfo").GetComponent<Button>().onClick.AddListener(OnButtonPostEvent);
        BtnRoot.FindChild("BtnSendMsgProtobuf").GetComponent<Button>().onClick.AddListener(OnButtonProtoBuffSendMsg);
        BtnRoot.FindChild("BtnSendMsgBinary").GetComponent<Button>().onClick.AddListener(OnButtonBinarySendMsg);
    }


    private void OnButtonConnect()
    {
        SocketManager.Instance.Connect(GameConst.IP, GameConst.Port);
    }

    private void OnButtonDisConnect()
    {
        SocketManager.Instance.Close();
    }

    private void OnButtonPostEvent()
    {
        string content = "GameLogicEvent";
        MessageCenter.Instance.PostEvent(GameLogicEventType.NoticeInfo, content);
    }

    private void OnButtonProtoBuffSendMsg()
    {
        gprotocol.CSLOGINSERVER csloginServer = new gprotocol.CSLOGINSERVER();
        csloginServer.account = "ProtoBufLogicData";
        csloginServer.password = "ProtoBuf123456";
        SocketManager.Instance.SendMsg(ProtocalCommand.scprotobuflogin, csloginServer);
    }
    
    private void OnButtonBinarySendMsg()
    {
        ByteStreamBuff tmpbuff = new ByteStreamBuff();
        tmpbuff.WriteInt(1314);
        tmpbuff.WriteFloat(99.99f);
        tmpbuff.WriteUniCodeString("Claine");
        tmpbuff.WriteUniCodeString("123456");
        SocketManager.Instance.SendMsg(ProtocalCommand.scbinarylogin, tmpbuff);
    }



    private void CallBackPoseEvent(object eventParam)
    {
        string content = (string)eventParam;
        Debug.Log(content);
    }

    private void CallBackProtoBuffLoginServer(byte[] msgData)
    {
        gprotocol.CSLOGINSERVER tmpLoginServer = SocketManager.ProtoBufDeserialize<gprotocol.CSLOGINSERVER>(msgData);
        Debug.Log(tmpLoginServer.account);
        Debug.Log(tmpLoginServer.password);
    }

    private void CallBackBinaryLoginServer(byte[] msgData)
    {
        ByteStreamBuff tmpbuff = new ByteStreamBuff(msgData);
        Debug.Log(tmpbuff.ReadInt());
        Debug.Log(tmpbuff.ReadFloat());
        Debug.Log(tmpbuff.ReadUniCodeString());
        Debug.Log(tmpbuff.ReadUniCodeString());
        tmpbuff.Close();
        tmpbuff = null;
    }
}
