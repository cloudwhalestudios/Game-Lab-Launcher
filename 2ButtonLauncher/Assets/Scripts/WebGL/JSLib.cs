using UnityEngine;
using System.Runtime.InteropServices;

namespace WebGLIntegration
{
    public static class JSLib
    {
        [DllImport("__Internal")] public static extern void Redirect(string str_location);
        [DllImport("__Internal")] public static extern void Refresh();
    }
}
