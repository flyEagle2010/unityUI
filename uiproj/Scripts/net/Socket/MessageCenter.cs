/// <summary>
/// 网络消息处理中心
/// 
/// create at 2014.8.26 by sun
/// </summary>


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public struct GameLogicEvent
{
    public GameLogicEventType eventType;
    public object eventData;
}

public struct NetMessageEvent
{
    public ProtocalCommand eventType;
    public byte[] eventData;
}

public delegate void GameLogicHandle(object data);
public delegate void NetMsgHandle(byte[] data);

public class MessageCenter : SingletonMonoBehaviour<MessageCenter>
{
    private Dictionary<ProtocalCommand, NetMsgHandle> netMessageEvtList = new Dictionary<ProtocalCommand, NetMsgHandle>();
	private Dictionary<GameLogicEventType, GameLogicHandle> gameLogicEvtList = new Dictionary<GameLogicEventType, GameLogicHandle>();

    public Queue<NetMessageEvent> RecvMessageQueue = new Queue<NetMessageEvent>();
	public Queue<NetMessageEvent> SendMessageQueue = new Queue<NetMessageEvent>();
    public Queue<GameLogicEvent> GameLogicDataQueue = new Queue<GameLogicEvent>();

    //添加网络事件观察者
    public void addObsever(ProtocalCommand protocalType, NetMsgHandle handle)
    {
        if (netMessageEvtList.ContainsKey(protocalType))
        {
            netMessageEvtList[protocalType] += handle;
        }
        else
        {
            netMessageEvtList.Add(protocalType, handle);
        }
    }

    //删除网络事件观察者
    public void removeObserver(ProtocalCommand protocalType, NetMsgHandle handle)
    {
        if (netMessageEvtList.ContainsKey(protocalType))
        {
            netMessageEvtList[protocalType] -= handle;
            if (netMessageEvtList[protocalType] == null)
            {
                netMessageEvtList.Remove(protocalType);
            }
        }
    }
    
    //添加普通事件观察者
	public void AddEventListener(GameLogicEventType eventType, GameLogicHandle handle)
    {
        if (gameLogicEvtList.ContainsKey(eventType))
        {
            gameLogicEvtList[eventType] += handle;
        }
        else
        {
            gameLogicEvtList.Add(eventType, handle);
        }
    }

    //删除普通事件观察者
	public void RemoveEventListener(GameLogicEventType eventType, GameLogicHandle callback)
    {
        if (gameLogicEvtList.ContainsKey(eventType))
        {
            gameLogicEvtList[eventType] -= callback;
            if (gameLogicEvtList[eventType] == null)
            {
                gameLogicEvtList.Remove(eventType);
            }
        }
    }

    //推送消息
	public void PostEvent(GameLogicEventType eventType, object data = null)
    {
        if (gameLogicEvtList.ContainsKey(eventType))
        {
            gameLogicEvtList[eventType](data);
        }
    }
    
    void Update()
    {
        while (GameLogicDataQueue.Count > 0)
        {
            GameLogicEvent gameLogicEvt = GameLogicDataQueue.Dequeue();
            if (gameLogicEvtList.ContainsKey(gameLogicEvt.eventType))
            {
                gameLogicEvtList[gameLogicEvt.eventType](gameLogicEvt.eventData);
            }
        }

		while (SendMessageQueue.Count > 0) {
			lock (SendMessageQueue) {
				byte[] msgData = SendMessageQueue.Dequeue ();
				SocketManager.Instance.SendMsg (msgData);
				Debug.Log ("sendmessage............");
			}	
		}

		while (RecvMessageQueue.Count > 0)
        {
			lock (RecvMessageQueue)
            {
				NetMessageEvent netMessageEvt = RecvMessageQueue.Dequeue();
                if (netMessageEvtList.ContainsKey(netMessageEvt.eventType))
                {
                    netMessageEvtList[netMessageEvt.eventType](netMessageEvt.eventData);
                }
            }
        }

    }
}