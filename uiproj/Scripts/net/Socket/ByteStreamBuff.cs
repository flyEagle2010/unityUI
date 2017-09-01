using System;
using System.IO;
using System.Text;

/// <summary>
/// 二进制数据流
/// </summary>
public class ByteStreamBuff
{
    private MemoryStream stream = null;
    private BinaryWriter writer = null;
    private BinaryReader reader = null;

    public ByteStreamBuff()
    {
        stream = new MemoryStream();
        writer = new BinaryWriter(stream);
    }

    public ByteStreamBuff(byte[] data)
    {
        stream = new MemoryStream(data);
        reader = new BinaryReader(stream);
    }

    public void WriteByte(byte data)
    {
        writer.Write(data);
    }
    public void WriteBytes(byte[] data)
    {
        writer.Write(data.Length);
        writer.Write(data);
    }
    public void WriteInt(int data)
    {
        writer.Write(data);
    }
    public void WriteuInt(uint data)
    {
        writer.Write(data);
    }
    public void WriteShort(short data)
    {
        writer.Write(data);
    }
    public void WriteuShort(ushort data)
    {
        writer.Write(data);
    }
    public void WriteLong(long data)
    {
        writer.Write(data);
    }
    public void WriteuLong(ulong data)
    {
        writer.Write(data);
    }
    public void WriteBool(bool data)
    {
        writer.Write(data);
    }
    public void WriteFloat(float data)
    {
        byte[] temp = flip(BitConverter.GetBytes(data));
        writer.Write(temp.Length);
        writer.Write(BitConverter.ToSingle(temp, 0));
    }
    public void WriteDouble(double data)
    {
        byte[] temp = flip(BitConverter.GetBytes(data));
        writer.Write(temp.Length);
        writer.Write(BitConverter.ToDouble(temp, 0));
    }
    public void WriteUTF8String(string data)
    {
        byte[] temp = Encoding.UTF8.GetBytes(data);
        writer.Write(temp.Length);
        writer.Write(temp);
    }
    public void WriteUniCodeString(string data)
    {
        byte[] temp = Encoding.Unicode.GetBytes(data);
        writer.Write(temp.Length);
        writer.Write(temp);
    }

    public byte ReadByte()
    {
        return reader.ReadByte();
    }
    public byte[] ReadBytes()
    {
        int len = ReadInt();
        return reader.ReadBytes(len);
    }
    public int ReadInt()
    {
        return reader.ReadInt32();
    }
    public uint ReaduInt()
    {
        return reader.ReadUInt32();
    }
    public short ReadShort()
    {
        return reader.ReadInt16();
    }
    public ushort ReaduShort()
    {
        return reader.ReadUInt16();
    }
    public long ReadLong()
    {
        return reader.ReadInt64();
    }
    public ulong ReaduLong()
    {
        return reader.ReadUInt64();
    }
    public bool ReadBool()
    {
        return reader.ReadBoolean();
    }
    public float ReadFloat()
    {
        return BitConverter.ToSingle(flip(ReadBytes()), 0);
    }
    public double ReadDouble()
    {
        return BitConverter.ToDouble(flip(ReadBytes()), 0);
    }
    public string ReadUTF8String()
    {
        return Encoding.UTF8.GetString(ReadBytes());
    }
    public string ReadUniCodeString()
    {
        return Encoding.Unicode.GetString(ReadBytes());
    }

    private byte[] flip(byte[] data)
    {
        if (BitConverter.IsLittleEndian)
            Array.Reverse(data);
        return data;
    }
    public byte[] ToArray()
    {
        stream.Flush();
        return stream.ToArray();
    }

    public void Close()
    {
        if (stream != null)
        {
            stream.Close();
            stream = null;
        }
        if (writer != null)
        {
            writer.Close();
            writer = null;
        }
        if (reader != null)
        {
            reader.Close();
            reader = null;
        }
    }
}