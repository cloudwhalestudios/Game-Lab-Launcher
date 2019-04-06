using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class TestWebGL : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void OpenGame(string location);
    [DllImport("__Internal")]
    private static extern void Reload();
    [DllImport("__Internal")]
    private static extern void Crash();

    [DllImport("__Internal")]
    private static extern void Hello();


    private void Start()
    {
        Hello();
    }

    public void WebGLOpenGame(string location)
    {
        OpenGame(location);
    }

    public void WebGLReload()
    {
        Reload();
    }

    public void WebGLCrash()
    {
        Crash();
    }
}
