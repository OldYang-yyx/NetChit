using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;
using System;

public class ProtoHelper 
{

    //���л��ӿ�
    public static byte[] ToBytes(object message) {

        return ((Google.Protobuf.IMessage)message).ToByteArray();
    }

    //�����л��ӿ�
    public static T ToObject<T>(byte[] bytes) where T: Google.Protobuf.IMessage{
        var message = Activator.CreateInstance<T>();
        message.MergeFrom(bytes);
        return message;
    }
}
    