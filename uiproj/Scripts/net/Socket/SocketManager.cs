using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System;
using System.Text;
using System.IO;
using ProtoBuf;


public class SocketManager
{
    private static SocketManager instance;


    private string currIP;
    private int currPort;

    private bool isConnected = false;
    private Socket clientSocket = null;
    private Thread receiveThread = null;
	private Thread sendThread = null;

    private DataBuffer dataBuffer = new DataBuffer();
    private SocketData socketData = new SocketData();
	private byte[] tmpReceiveBuff = new byte[4096];

	public bool IsConnceted { get { return isConnected; } }


	public static SocketManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new SocketManager();
			}
			return instance;
		}
	}

    /// <summary>
    /// 断开
    /// </summary>
    private void close()
    {
		if (!isConnected) {
			return;
		}

        isConnected = false;

        if (receiveThread != null)
        {
            receiveThread.Abort();
			receiveThread.Join();
            receiveThread = null;
        }

		if (sendThread != null) {
			sendThread.Abort ();
			sendThread.Join ();
			sendThread = null;
		}

        if (clientSocket != null && clientSocket.Connected)
        {
            clientSocket.Close();
            clientSocket = null;
        }
    }

    private void ReConnect()
    {
    }

    /// <summary>
    /// 连接
    /// </summary>
    private void onConnet()
    {
        try
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//创建套接字
            IPAddress ipAddress = IPAddress.Parse(currIP);//解析IP地址
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, currPort);
            IAsyncResult result = clientSocket.BeginConnect(ipEndPoint, new AsyncCallback(onConnectSucess), clientSocket);//异步连接
            bool success = result.AsyncWaitHandle.WaitOne(5000, true);
            if (!success) //超时
            {
                onConnectOutTime();
            }
        }
        catch (System.Exception e)
		{
			Debug.Log ("连接失败:" + e);
            onConnectFail();
        }
    }

    private void onConnectSucess(IAsyncResult iar)
    {
        try
        {
			isConnected = true;

            Socket client = (Socket)iar.AsyncState;
            client.EndConnect(iar);

            receiveThread = new Thread(new ThreadStart(onReceiveSocket));
            receiveThread.IsBackground = true;
            receiveThread.Start();

			sendThread = new Thread(new ThreadStart(onSendSocket));
			sendThread.IsBackground = true;
			sendThread.Start();

            Debug.Log("连接成功");
        }
        catch (Exception e)
        {
            Close();
        }
    }

    private void onConnectOutTime()
    {
        close();
    }

    private void onConnectFail()
    {
        close();
    }

	private void onSendSocket(ProtocalCommand protocalType, ProtoBuf.IExtensible data){

		byte[] msgData = DataToBytes(protocalType, ProtoBufSerializer(data));

		lock (MessageCenter.Instance.SendMessageQueue)
		{
			MessageCenter.Instance.SendMessageQueue.Enqueue(msgData);
		}
	}

    /// <summary>
    /// 接受网络数据
    /// </summary>
    private void onReceiveSocket()
    {
        while (true)
        {
            if (!clientSocket.Connected)
            {
                isConnected = false;
                ReConnect();
                break;
            }
            try
            {
                int receiveLength = clientSocket.Receive(tmpReceiveBuff);
                if (receiveLength > 0)
                {
                    dataBuffer.AddBuffer(tmpReceiveBuff, receiveLength);//将收到的数据添加到缓存器中
                    while (dataBuffer.GetData(out socketData))//取出一条完整数据
                    {
                        NetMessageEvent netMessageData = new NetMessageEvent();
                        netMessageData.eventType = socketData.protocallType;
                        netMessageData.eventData = socketData.data;

                        //锁死消息中心消息队列，并添加数据
						lock (MessageCenter.Instance.RecvMessageQueue)
                        {
                            Debug.Log(netMessageData.eventType);
							MessageCenter.Instance.RecvMessageQueue.Enqueue(netMessageData);
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                clientSocket.Disconnect(true);
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
                break;
            }
        }
    }





    /// <summary>
    /// 数据转网络结构
    /// </summary>
    /// <param name="protocalType"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    private SocketData BytesToSocketData(ProtocalCommand protocalType, byte[] data)
    {
        SocketData tmpSocketData = new SocketData();
        tmpSocketData.buffLength = Constants.HEADLEN + data.Length;
        tmpSocketData.dataLength = data.Length;
        tmpSocketData.protocallType = protocalType;
        tmpSocketData.data = data;
        return tmpSocketData;
    }

    /// <summary>
    /// 网络结构转数据
    /// </summary>
    /// <param name="tmpSocketData"></param>
    /// <returns></returns>
    private byte[] SocketDataToBytes(SocketData tmpSocketData)
    {
        byte[] tmpBuff = new byte[tmpSocketData.buffLength];
        byte[] tmpBuffLength = BitConverter.GetBytes(tmpSocketData.buffLength);
        byte[] tmpDataLenght = BitConverter.GetBytes((UInt16)tmpSocketData.protocallType);

        Array.Copy(tmpBuffLength, 0, tmpBuff, 0, Constants.HEADDATALEN);//缓存总长度
        Array.Copy(tmpDataLenght, 0, tmpBuff, Constants.HEADDATALEN, Constants.HEADTYPELEN);//协议类型
        Array.Copy(tmpSocketData.data, 0, tmpBuff, Constants.HEADLEN, tmpSocketData.dataLength);//协议数据

        return tmpBuff;
    }

    /// <summary>
    /// 合并协议，数据
    /// </summary>
    /// <param name="protocalType"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    private byte[] DataToBytes(ProtocalCommand protocalType, byte[] data)
    {
        return SocketDataToBytes(BytesToSocketData(protocalType, data));
    }


    /// <summary>
    /// ProtoBuf序列化
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static byte[] ProtoBufSerializer(ProtoBuf.IExtensible data)
    {
        using (MemoryStream m = new MemoryStream())
        {
            byte[] buffer = null;
            Serializer.Serialize(m, data);
            m.Position = 0;
            int length = (int)m.Length;
            buffer = new byte[length];
            m.Read(buffer, 0, length);
            return buffer;
        }
    }

    /// <summary>
    /// ProtoBuf反序列化
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    public static T ProtoBufDeserialize<T>(byte[] data)
    {
        using (MemoryStream m = new MemoryStream(data))
        {
            return Serializer.Deserialize<T>(m);
        }
    }



    /// <summary>
    /// 连接服务器
    /// </summary>
    /// <param name="currIP"></param>
    /// <param name="currPort"></param>
    public void Connect(string currIP, int currPort)
    {
        if (!IsConnceted)
        {
            this.currIP = currIP;
            this.currPort = currPort;
            onConnet();
        }
    }


	public void SendMsg (byte[] msgData){
		if (clientSocket == null || !clientSocket.Connected)
		{
			ReConnect();
			return;
		}
		clientSocket.BeginSend (msgData, 0, msgData.Length, SocketFlags.None, new AsyncCallback (onSendMsg), clientSocket);
	}

	/// <summary>
	/// 发送消息结果回掉，可判断当前网络状态
	/// </summary>
	/// <param name="asyncSend"></param>
	private void onSendMsg(IAsyncResult asyncSend)
	{
		try
		{
			Socket client = (Socket)asyncSend.AsyncState;
			client.EndSend(asyncSend);
		}
		catch (Exception e)
		{
			Debug.Log("send msg exception:" + e.StackTrace);
		}
	}

	/*
    /// <summary>
    /// 发送消息基本方法
    /// </summary>
    /// <param name="protocalType"></param>
    /// <param name="data"></param>
    private void SendMsgBase(ProtocalCommand protocalType, byte[] data)
    {
        if (clientSocket == null || !clientSocket.Connected)
        {
            ReConnect();
            return;
        }

        byte[] msgdata = DataToBytes(protocalType, data);
        clientSocket.BeginSend(msgdata, 0, msgdata.Length, SocketFlags.None, new AsyncCallback(onSendMsg), clientSocket);
    }




    /// <summary>
    /// 以二进制方式发送
    /// </summary>
    /// <param name="protocalType"></param>
    /// <param name="byteStreamBuff"></param>
    public void SendMsg(ProtocalCommand protocalType, ByteStreamBuff byteStreamBuff)
    {
        SendMsgBase(protocalType, byteStreamBuff.ToArray());
    }

    /// <summary>
    /// 以ProtoBuf方式发送
    /// </summary>
    /// <param name="protocalType"></param>
    /// <param name="data"></param>
    public void SendMsg(ProtocalCommand protocalType, ProtoBuf.IExtensible data)
    {
        SendMsgBase(protocalType, ProtoBufSerializer(data));
    }

	*/
    public void Close()
    {
        close();
    }

}