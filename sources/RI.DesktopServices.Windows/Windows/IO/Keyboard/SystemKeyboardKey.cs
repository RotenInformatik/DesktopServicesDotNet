using System;
using System.Diagnostics.CodeAnalysis;




namespace RI.Framework.IO.Keyboard
{
    /// <summary>
    ///     Describes the keyboard keys recognizable by <see cref="SystemKeyboard" />.
    /// </summary>
    [Serializable]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum SystemKeyboardKey
    {
        /// <summary>
        ///     A
        /// </summary>
        AsciiA = 'A',

        /// <summary>
        ///     B
        /// </summary>
        AsciiB = 'B',

        /// <summary>
        ///     C
        /// </summary>
        AsciiC = 'C',

        /// <summary>
        ///     D
        /// </summary>
        AsciiD = 'D',

        /// <summary>
        ///     E
        /// </summary>
        AsciiE = 'E',

        /// <summary>
        ///     F
        /// </summary>
        AsciiF = 'F',

        /// <summary>
        ///     G
        /// </summary>
        AsciiG = 'G',

        /// <summary>
        ///     H
        /// </summary>
        AsciiH = 'H',

        /// <summary>
        ///     I
        /// </summary>
        AsciiI = 'I',

        /// <summary>
        ///     J
        /// </summary>
        AsciiJ = 'J',

        /// <summary>
        ///     K
        /// </summary>
        AsciiK = 'K',

        /// <summary>
        ///     L
        /// </summary>
        AsciiL = 'L',

        /// <summary>
        ///     M
        /// </summary>
        AsciiM = 'M',

        /// <summary>
        ///     N
        /// </summary>
        AsciiN = 'N',

        /// <summary>
        ///     O
        /// </summary>
        AsciiO = 'O',

        /// <summary>
        ///     P
        /// </summary>
        AsciiP = 'P',

        /// <summary>
        ///     Q
        /// </summary>
        AsciiQ = 'Q',

        /// <summary>
        ///     R
        /// </summary>
        AsciiR = 'R',

        /// <summary>
        ///     S
        /// </summary>
        AsciiS = 'S',

        /// <summary>
        ///     T
        /// </summary>
        AsciiT = 'T',

        /// <summary>
        ///     U
        /// </summary>
        AsciiU = 'U',

        /// <summary>
        ///     V
        /// </summary>
        AsciiV = 'V',

        /// <summary>
        ///     W
        /// </summary>
        AsciiW = 'W',

        /// <summary>
        ///     X
        /// </summary>
        AsciiX = 'X',

        /// <summary>
        ///     Y
        /// </summary>
        AsciiY = 'Y',

        /// <summary>
        ///     Z
        /// </summary>
        AsciiZ = 'Z',

        /// <summary>
        ///     1
        /// </summary>
        Ascii1 = '1',

        /// <summary>
        ///     2
        /// </summary>
        Ascii2 = '2',

        /// <summary>
        ///     3
        /// </summary>
        Ascii3 = '3',

        /// <summary>
        ///     4
        /// </summary>
        Ascii4 = '4',

        /// <summary>
        ///     5
        /// </summary>
        Ascii5 = '5',

        /// <summary>
        ///     6
        /// </summary>
        Ascii6 = '6',

        /// <summary>
        ///     7
        /// </summary>
        Ascii7 = '7',

        /// <summary>
        ///     8
        /// </summary>
        Ascii8 = '8',

        /// <summary>
        ///     9
        /// </summary>
        Ascii9 = '9',

        /// <summary>
        ///     0
        /// </summary>
        Ascii0 = '0',

        /// <summary>
        ///     Left button
        /// </summary>
        LButton = 0x01,

        /// <summary>
        ///     Right button
        /// </summary>
        RButton = 0x02,

        /// <summary>
        ///     Cancel
        /// </summary>
        Cancel = 0x03,

        /// <summary>
        ///     Middle button
        /// </summary>
        MButton = 0x04,

        /// <summary>
        ///     Extended button 1
        /// </summary>
        XButton1 = 0x05,

        /// <summary>
        ///     Extended button 2
        /// </summary>
        XButton2 = 0x06,

        /// <summary>
        ///     Back
        /// </summary>
        Back = 0x08,

        /// <summary>
        ///     Tab
        /// </summary>
        Tab = 0x09,

        /// <summary>
        ///     Clear
        /// </summary>
        Clear = 0x0C,

        /// <summary>
        ///     Return
        /// </summary>
        Return = 0x0D,

        /// <summary>
        ///     Shift
        /// </summary>
        Shift = 0x10,

        /// <summary>
        ///     Control
        /// </summary>
        Control = 0x11,

        /// <summary>
        ///     Menu
        /// </summary>
        Menu = 0x12,

        /// <summary>
        ///     Pause
        /// </summary>
        Pause = 0x13,

        /// <summary>
        ///     Capital
        /// </summary>
        Capital = 0x14,

        /// <summary>
        ///     Kana
        /// </summary>
        Kana = 0x15,

        /// <summary>
        ///     Hangeul
        /// </summary>
        Hangeul = 0x15,

        /// <summary>
        ///     Hangul
        /// </summary>
        Hangul = 0x15,

        /// <summary>
        ///     Junja
        /// </summary>
        Junja = 0x17,

        /// <summary>
        ///     Final
        /// </summary>
        Final = 0x18,

        /// <summary>
        ///     Hanja
        /// </summary>
        Hanja = 0x19,

        /// <summary>
        ///     Kanji
        /// </summary>
        Kanji = 0x19,

        /// <summary>
        ///     Escape
        /// </summary>
        Escape = 0x1B,

        /// <summary>
        ///     Convert
        /// </summary>
        Convert = 0x1C,

        /// <summary>
        ///     Non-convert
        /// </summary>
        NonConvert = 0x1D,

        /// <summary>
        ///     Accept
        /// </summary>
        Accept = 0x1E,

        /// <summary>
        ///     Mode change
        /// </summary>
        ModeChange = 0x1F,

        /// <summary>
        ///     Space
        /// </summary>
        Space = 0x20,

        /// <summary>
        ///     Prior
        /// </summary>
        Prior = 0x21,

        /// <summary>
        ///     Next
        /// </summary>
        Next = 0x22,

        /// <summary>
        ///     End
        /// </summary>
        End = 0x23,

        /// <summary>
        ///     Home
        /// </summary>
        Home = 0x24,

        /// <summary>
        ///     Left
        /// </summary>
        Left = 0x25,

        /// <summary>
        ///     Up
        /// </summary>
        Up = 0x26,

        /// <summary>
        ///     Right
        /// </summary>
        Right = 0x27,

        /// <summary>
        ///     Down
        /// </summary>
        Down = 0x28,

        /// <summary>
        ///     Select
        /// </summary>
        Select = 0x29,

        /// <summary>
        ///     Print
        /// </summary>
        Print = 0x2A,

        /// <summary>
        ///     Execute
        /// </summary>
        Execute = 0x2B,

        /// <summary>
        ///     Snapshot
        /// </summary>
        Snapshot = 0x2C,

        /// <summary>
        ///     Insert
        /// </summary>
        Insert = 0x2D,

        /// <summary>
        ///     Delete
        /// </summary>
        Delete = 0x2E,

        /// <summary>
        ///     Help
        /// </summary>
        Help = 0x2F,

        /// <summary>
        ///     Left Windows
        /// </summary>
        LWin = 0x5B,

        /// <summary>
        ///     Right Windows
        /// </summary>
        RWin = 0x5C,

        /// <summary>
        ///     Applications
        /// </summary>
        Apps = 0x5D,

        /// <summary>
        ///     Sleep
        /// </summary>
        Sleep = 0x5F,

        /// <summary>
        ///     Num pad 0
        /// </summary>
        NumPad0 = 0x60,

        /// <summary>
        ///     Num pad 1
        /// </summary>
        NumPad1 = 0x61,

        /// <summary>
        ///     Num pad 2
        /// </summary>
        NumPad2 = 0x62,

        /// <summary>
        ///     Num pad 3
        /// </summary>
        NumPad3 = 0x63,

        /// <summary>
        ///     Num pad 4
        /// </summary>
        NumPad4 = 0x64,

        /// <summary>
        ///     Num pad 5
        /// </summary>
        NumPad5 = 0x65,

        /// <summary>
        ///     Num pad 6
        /// </summary>
        NumPad6 = 0x66,

        /// <summary>
        ///     Num pad 7
        /// </summary>
        NumPad7 = 0x67,

        /// <summary>
        ///     Num pad 8
        /// </summary>
        NumPad8 = 0x68,

        /// <summary>
        ///     Num pad 9
        /// </summary>
        NumPad9 = 0x69,

        /// <summary>
        ///     Multiply
        /// </summary>
        Multiply = 0x6A,

        /// <summary>
        ///     Add
        /// </summary>
        Add = 0x6B,

        /// <summary>
        ///     Separator
        /// </summary>
        Separator = 0x6C,

        /// <summary>
        ///     Subtract
        /// </summary>
        Subtract = 0x6D,

        /// <summary>
        ///     Decimal
        /// </summary>
        Decimal = 0x6E,

        /// <summary>
        ///     Divide
        /// </summary>
        Divide = 0x6F,

        /// <summary>
        ///     F1
        /// </summary>
        F1 = 0x70,

        /// <summary>
        ///     F2
        /// </summary>
        F2 = 0x71,

        /// <summary>
        ///     F3
        /// </summary>
        F3 = 0x72,

        /// <summary>
        ///     F4
        /// </summary>
        F4 = 0x73,

        /// <summary>
        ///     F5
        /// </summary>
        F5 = 0x74,

        /// <summary>
        ///     F6
        /// </summary>
        F6 = 0x75,

        /// <summary>
        ///     F7
        /// </summary>
        F7 = 0x76,

        /// <summary>
        ///     F8
        /// </summary>
        F8 = 0x77,

        /// <summary>
        ///     F9
        /// </summary>
        F9 = 0x78,

        /// <summary>
        ///     F10
        /// </summary>
        F10 = 0x79,

        /// <summary>
        ///     F11
        /// </summary>
        F11 = 0x7A,

        /// <summary>
        ///     F12
        /// </summary>
        F12 = 0x7B,

        /// <summary>
        ///     F13
        /// </summary>
        F13 = 0x7C,

        /// <summary>
        ///     F14
        /// </summary>
        F14 = 0x7D,

        /// <summary>
        ///     F15
        /// </summary>
        F15 = 0x7E,

        /// <summary>
        ///     F16
        /// </summary>
        F16 = 0x7F,

        /// <summary>
        ///     F17
        /// </summary>
        F17 = 0x80,

        /// <summary>
        ///     F18
        /// </summary>
        F18 = 0x81,

        /// <summary>
        ///     F19
        /// </summary>
        F19 = 0x82,

        /// <summary>
        ///     F20
        /// </summary>
        F20 = 0x83,

        /// <summary>
        ///     F21
        /// </summary>
        F21 = 0x84,

        /// <summary>
        ///     F22
        /// </summary>
        F22 = 0x85,

        /// <summary>
        ///     F23
        /// </summary>
        F23 = 0x86,

        /// <summary>
        ///     F24
        /// </summary>
        F24 = 0x87,

        /// <summary>
        ///     Num lock
        /// </summary>
        NumLock = 0x90,

        /// <summary>
        ///     Scroll lock
        /// </summary>
        Scroll = 0x91,

        /// <summary>
        ///     Num pad equal
        /// </summary>
        OemNECEqual = 0x92, // '=' key on numpad

        //OeamFJJisho = 0x92, // 'Dictionary' key

        //OeamFJMasshou = 0x93, // 'Unregister word' key

        //OeamFJTouroku = 0x94, // 'Register word' key

        //OeamFJLOya = 0x95, // 'Left OYAYUBI' key

        //OeamFJROya = 0x96, // 'Right OYAYUBI' key

        /// <summary>
        ///     Left shift
        /// </summary>
        LShift = 0xA0,

        /// <summary>
        ///     Right shift
        /// </summary>
        RShift = 0xA1,

        /// <summary>
        ///     Left control
        /// </summary>
        LControl = 0xA2,

        /// <summary>
        ///     Right control
        /// </summary>
        RControl = 0xA3,

        /// <summary>
        ///     Left menu
        /// </summary>
        LMenu = 0xA4,

        /// <summary>
        ///     Right menu
        /// </summary>
        RMenu = 0xA5,

        /// <summary>
        ///     Browser back
        /// </summary>
        BrowserBack = 0xA6,

        /// <summary>
        ///     Browser forward
        /// </summary>
        BrowserForward = 0xA7,

        /// <summary>
        ///     Browser refresh
        /// </summary>
        BrowserRefresh = 0xA8,

        /// <summary>
        ///     Browser stop
        /// </summary>
        BrowserStop = 0xA9,

        /// <summary>
        ///     Browser search
        /// </summary>
        BrowserSearch = 0xAA,

        /// <summary>
        ///     Browser favorites
        /// </summary>
        BrowserFavorites = 0xAB,

        /// <summary>
        ///     Browser home
        /// </summary>
        BrowserHome = 0xAC,

        /// <summary>
        ///     Volume mute
        /// </summary>
        VolumeMute = 0xAD,

        /// <summary>
        ///     Volume down
        /// </summary>
        VolumeDown = 0xAE,

        /// <summary>
        ///     Volume up
        /// </summary>
        VolumeUp = 0xAF,

        /// <summary>
        ///     Media next track
        /// </summary>
        MediaNextTrack = 0xB0,

        /// <summary>
        ///     Media previous track
        /// </summary>
        MediaPrevTrack = 0xB1,

        /// <summary>
        ///     Media stop
        /// </summary>
        MediaStop = 0xB2,

        /// <summary>
        ///     Media play/pause
        /// </summary>
        MediaPlayPause = 0xB3,

        /// <summary>
        ///     Launch mail
        /// </summary>
        LaunchMail = 0xB4,

        /// <summary>
        ///     Launch media
        /// </summary>
        LaunchMediaSelect = 0xB5,

        /// <summary>
        ///     Launch application 1
        /// </summary>
        LaunchApp1 = 0xB6,

        /// <summary>
        ///     Launch application 2
        /// </summary>
        LaunchApp2 = 0xB7,

        //Oem1 = 0xBA, // ';:' for US

        /// <summary>
        ///     Plus
        /// </summary>
        OemPlus = 0xBB, // '+' any country

        /// <summary>
        ///     Comma
        /// </summary>
        OemComma = 0xBC, // ',' any country

        /// <summary>
        ///     Minus
        /// </summary>
        OemMinus = 0xBD, // '-' any country

        /// <summary>
        ///     Period
        /// </summary>
        OemPeriod = 0xBE, // '.' any country

        //Oem2 = 0xBF, // '/?' for US

        //Oem3 = 0xC0, // '`~' for US

        //Oem4 = 0xDB, // '[{' for US

        //Oem5 = 0xDC, // '\|' for US

        //Oem6 = 0xDD, // ']}' for US

        //Oem7 = 0xDE, // ''"' for US

        //Oem8 = 0xDF,

        //OemAX = 0xE1, // 'AX' key on Japanese AX kbd

        //Oem102 = 0xE2, // "<>" or "\|" on RT 102-key kbd.

        //IcoHelp = 0xE3, // Help key on ICO

        //Ico00 = 0xE4, // 00 key on ICO

        /// <summary>
        ///     Process key
        /// </summary>
        ProcessKey = 0xE5,

        //IcoClear = 0xE6,

        /// <summary>
        ///     Packet
        /// </summary>
        Packet = 0xE7,

        //OemReset = 0xE9,

        //OemJump = 0xEA,

        //OemPA1 = 0xEB,

        //OemPA2 = 0xEC,

        //OemPA3 = 0xED,

        //OemWSCtrl = 0xEE,

        //OemCUSel = 0xEF,

        //OemAttn = 0xF0,

        //OemFinish = 0xF1,

        //OemCopy = 0xF2,

        //OemAuto = 0xF3,

        //OemENLW = 0xF4,

        //OemBackTab = 0xF5,

        //Attn = 0xF6,

        //CRSel = 0xF7,

        //EXSel = 0xF8,

        //EREOF = 0xF9,

        /// <summary>
        ///     Play
        /// </summary>
        Play = 0xFA,

        /// <summary>
        ///     Zoom
        /// </summary>
        Zoom = 0xFB,

        //NoName = 0xFC,

        //PA1 = 0xFD,

        //OemClear = 0xFE,
    }
}
