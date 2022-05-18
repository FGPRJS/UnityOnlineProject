using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

public class JSWebSocket : MonoBehaviour
{
    public delegate void OpenEventActivated();
    public static event OpenEventActivated OpenEvent;
    public delegate void CloseEventActivated();
    public static event CloseEventActivated CloseEvent;
    public delegate void ErrorEventActivated();
    public static event ErrorEventActivated ErrorEvent;
    public delegate void MessageEventActivated(string message);
    public static event MessageEventActivated MessageEvent;
    
    
    [DllImport("__Internal")]
    static extern void Connect(string url);

    delegate void OnOpenCallback();
    [MonoPInvokeCallback(typeof(OnOpenCallback))]
    static void OnOpenAction()
    {
        OpenEvent?.Invoke();
    }
    [DllImport("__Internal")]
    static extern void SetOnOpenCallback(OnOpenCallback callback);
    
    delegate void OnCloseCallback();
    [MonoPInvokeCallback(typeof(OnCloseCallback))]
    static void OnCloseAction()
    {
        CloseEvent?.Invoke();
    }
    [DllImport("__Internal")]
    static extern void SetOnCloseCallback(OnCloseCallback callback);
    
    delegate void OnErrorCallback();
    [MonoPInvokeCallback(typeof(OnErrorCallback))]
    static void OnErrorAction()
    {
        ErrorEvent?.Invoke();
    }
    [DllImport("__Internal")]
    static extern void SetOnErrorCallback(OnErrorCallback callback);
    
    delegate void OnMessageCallback(string message);
    [MonoPInvokeCallback(typeof(OnMessageCallback))]
    static void OnMessageAction(string message)
    {
        MessageEvent?.Invoke(message);
    }
    [DllImport("__Internal")]
    static extern void SetOnMessageCallback(OnMessageCallback callback);
    
    
    
    [DllImport("__Internal")]
    public static extern void Send(string message);
    [DllImport("__Internal")]
    public static extern void Close(int code, string message);
    
    public static void ConnectToServer(string uri)
    {
        SetOnOpenCallback(OnOpenAction);
        SetOnCloseCallback(OnCloseAction);
        SetOnErrorCallback(OnErrorAction);
        SetOnMessageCallback(OnMessageAction);

        Connect(uri);
    }
}
