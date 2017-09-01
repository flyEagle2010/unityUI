using System.IO;
using System;
using UnityEngine;

//常量数据
public class Constants
{
    //消息：数据总长度(4byte) + 数据类型(2byte) + 数据(N byte)
    public static int HEADDATALEN = 4;
    public static int HEADTYPELEN = 2;
    public static int HEADLEN//6byte
    {
        get { return HEADDATALEN + HEADTYPELEN; }
    }
}

/// <summary>
/// 网络数据结构
/// </summary>
[System.Serializable]
public struct SocketData
{
    public byte[] data;
    public ProtocalCommand protocallType;
    public int buffLength;
    public int dataLength;
}

/// <summary>
/// 网络数据缓存器，
/// </summary>
[System.Serializable]
public class DataBuffer
{//自动大小数据缓存器
    private int minBuffLen;
    private byte[] buff;
    private int curBuffPosition;
    private int buffLength = 0;
    private int dataLength;
    private UInt16 protocalType;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="minBuffLen">最小缓冲区大小</param>
    public DataBuffer(int minBuffLen = 1024)
    {
        if (minBuffLen <= 0)
        {
            this.minBuffLen = 1024;
        }
        else
        {
            this.minBuffLen = minBuffLen;
        }
        buff = new byte[this.minBuffLen];
    }

    /// <summary>
    /// 添加缓存数据
    /// </summary>
    /// <param name="data"></param>
    /// <param name="dataLen"></param>
    public void AddBuffer(byte[] data, int dataLen)
    {
        if (dataLen > buff.Length - curBuffPosition)//超过当前缓存
        {
            byte[] tmpBuff = new byte[curBuffPosition + dataLen];
            Array.Copy(buff, 0, tmpBuff, 0, curBuffPosition);
            Array.Copy(data, 0, tmpBuff, curBuffPosition, dataLen);
            buff = tmpBuff;
            tmpBuff = null;
        }
        else
        {
            Array.Copy(data, 0, buff, curBuffPosition, dataLen);
        }
        curBuffPosition += dataLen;//修改当前数据标记
    }

    /// <summary>
    /// 更新数据长度
    /// </summary>
    public void UpdateDataLength()
    {
        if (dataLength == 0 && curBuffPosition >= Constants.HEADLEN)
        {
            byte[] tmpDataLen = new byte[Constants.HEADDATALEN];
            Array.Copy(buff, 0, tmpDataLen, 0, Constants.HEADDATALEN);
            buffLength = BitConverter.ToInt32(tmpDataLen, 0);

            byte[] tmpProtocalType = new byte[Constants.HEADTYPELEN];
            Array.Copy(buff, Constants.HEADDATALEN, tmpProtocalType, 0, Constants.HEADTYPELEN);
            protocalType = BitConverter.ToUInt16(tmpProtocalType, 0);

            dataLength = buffLength - Constants.HEADLEN;
        }
    }

    /// <summary>
    /// 获取一条可用数据，返回值标记是否有数据
    /// </summary>
    /// <param name="tmpSocketData"></param>
    /// <returns></returns>
    public bool GetData(out SocketData socketData)
    {
        socketData = new SocketData();

        if (buffLength <= 0)
        {
            UpdateDataLength();
        }

        if (buffLength > 0 && curBuffPosition >= buffLength)
        {
            socketData.buffLength = buffLength;
            socketData.dataLength = dataLength;
            socketData.protocallType = (ProtocalCommand)protocalType;
            socketData.data = new byte[dataLength];
            Array.Copy(buff, Constants.HEADLEN, socketData.data, 0, dataLength);
            curBuffPosition -= buffLength;
            byte[] tmpBuff = new byte[curBuffPosition < minBuffLen ? minBuffLen : curBuffPosition];
            Array.Copy(buff, buffLength, tmpBuff, 0, curBuffPosition);
            buff = tmpBuff;


            buffLength = 0;
            dataLength = 0;
            protocalType = 0;
            return true;
        }
        return false;
    }
    
}