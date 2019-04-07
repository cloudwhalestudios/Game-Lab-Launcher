﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


namespace WebGLIntegration
{
    public class WebGLRedirect
    {
        private static void Redirect(string url)
        {
            JSLib.Redirect(Config.BASE_URL + url);
        }

        public static void OpenPlatform()
        {
            Redirect(Config.LAUNCHER_URL);
        }
    }
}