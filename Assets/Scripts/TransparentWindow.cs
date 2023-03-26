using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparentWindow : MonoBehaviour
{
    [DllImport("user32.dll")]
    private static extern IntPtr SetActiveWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern IntPtr FindWindow(string className, string windowName);

    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow(); 

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong); //Set atributes for a window

    [DllImport("user32.dll")]
    static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInserAfter, int X, int Y, int cx, int xy, uint uFlags);

    private struct MARGINS
    { public int cxLeftWidth; public int cxRightWidth; public int xyTopHeight; public int cyBottomHeight; }

    [DllImport("Dwmapi.dll")]
    private static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins); //Allows to set window margins

    const int GWL_EXSTYLE = -20;

    const uint WS_EX_LAYERED = 0x00080000;
    const uint WS_EX_TRANSPARENT = 0x00000020;

    static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

    private void Start() {
#if !UNITY_EDITOR //Only run if outside of unity editor
        IntPtr hWnd = FindWindow(null,"KiteTrainer"); //Makes unity program the active window
        MARGINS margins = new MARGINS { cxLeftWidth = -1 }; //Negative value makes the window transparent
        DwmExtendFrameIntoClientArea(hWnd, ref margins); //Makes the window transparant


        SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT); //Makes the window clickthrough

        SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, 0); //Unity always stay on top
#endif
        Application.runInBackground = true; //Program runs in the background
    }
}
