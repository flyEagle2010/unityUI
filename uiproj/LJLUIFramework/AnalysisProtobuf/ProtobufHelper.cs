using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ProtoBuf;

namespace UIFrameworkForProtobuf
{
    class ProtobufHelper
    {
         //将对象序列化为字符串  
       public static string SerializerToString<T>(T t)  
        {  
            using (MemoryStream ms = new MemoryStream())  
            {  
                Serializer.Serialize(ms,t);  
                return ASCIIEncoding.UTF8.GetString(ms.ToArray());  
            }    
        }  
  
        //将对象序列化到文件  
       public static void SerializerToFile<T>(T t,string fileName)  
        {
            using (var file = File.Create(fileName))  
            {  
                Serializer.Serialize(file,t);  
           }  
        }  
  
  
       //将字符串转化为对象  
       public static T DederializerFromString<T>(string str)  
       {  
           //using作为语句，用于定义一个范围，在此范围的末尾将释放对象  
           //将字符串转化为内存流  
           using (MemoryStream ms=new MemoryStream(ASCIIEncoding.UTF8.GetBytes(str)))  
           {  
               T obj = Serializer.Deserialize<T>(ms);  
               return obj;  
           }       
       }  
  
        //将文件数据转化为对象  
       public static T DederializerFromFile<T>(string fileName)  
       {
           using (var file = File.OpenRead(fileName))  
           {  
               T obj = Serializer.Deserialize<T>(file);  
               return obj;  
           }  
       }  
    } 
   
}
