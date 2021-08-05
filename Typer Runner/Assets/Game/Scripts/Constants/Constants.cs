using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public static class ActionConstants
{
    #region Action Keywords

    public static readonly string[] jump = { "jump", "Jump" };
    public static readonly string[] slide = { "slide", "Slide" };
    public static readonly string[] dash = { "dash", "Dash", "zoom", "Zoom" };
    public static readonly string[] start = { "start", "Start", "go", "Go" };
    public static readonly string[] @throw = { "throw", "Throw", "yeet", "Yeet" };
    public static readonly string[] flip = { "slide", "Slide" };
    public static readonly string[] reset = { "reset", "Reset", "restart", "Restart" };

    #endregion
}

public static class TagConstants
{
    public static readonly string SD_Jump = "SlowDownJump";
    public static readonly string SD_Slide = "SlowDownSlide";
}