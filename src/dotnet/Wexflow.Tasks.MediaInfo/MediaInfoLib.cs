﻿/*  Copyright (c) MediaArea.net SARL. All Rights Reserved.
 *
 *  Use of this source code is governed by a BSD-style license that can
 *  be found in the License.html file in the root of the source tree.
 */

//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//
// Microsoft Visual C# wrapper for MediaInfo Library
// See MediaInfo.h for help
//
// To make it working, you must put MediaInfo.Dll
// in the executable folder
//
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

using System;
using System.Runtime.InteropServices;

#pragma warning disable 1591 // Disable XML documentation warnings

namespace Wexflow.Tasks.MediaInfo
{
    public enum StreamKind
    {
        General,
        Video,
        Audio,
        Text,
        Other,
        Image,
        Menu
    }

    public enum InfoKind
    {
        Name,
        Text,
        Measure,
        Options,
        NameText,
        MeasureText,
        Info,
        HowTo
    }

    public enum InfoOptions
    {
        ShowInInform,
        Support,
        ShowInSupported,
        TypeOfValue
    }

    public enum InfoFileOptions
    {
        FileOption_Nothing = 0x00,
        FileOption_NoRecursive = 0x01,
        FileOption_CloseAll = 0x02,
        FileOption_Max = 0x04
    }

    public enum Status
    {
        None = 0x00,
        Accepted = 0x01,
        Filled = 0x02,
        Updated = 0x04,
        Finalized = 0x08
    }

    public class MediaInfoLib
    {
        private readonly IntPtr Handle;
        private readonly bool MustUseAnsi;

        //MediaInfo class
        public MediaInfoLib()
        {
            try
            {
                Handle = MediaInfo_New();
            }
            catch
            {
                Handle = (IntPtr) 0;
            }
            if (Environment.OSVersion.ToString().IndexOf("Windows") == -1)
                MustUseAnsi = true;
            else
                MustUseAnsi = false;
        }

        //Import of DLL functions. DO NOT USE until you know what you do (MediaInfo DLL do NOT use CoTaskMemAlloc to allocate memory)
        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfo_New();

        [DllImport("MediaInfo.dll")]
        private static extern void MediaInfo_Delete(IntPtr Handle);

        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfo_Open(IntPtr Handle, [MarshalAs(UnmanagedType.LPWStr)] string FileName);

        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfoA_Open(IntPtr Handle, IntPtr FileName);

        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfo_Open_Buffer_Init(IntPtr Handle, long File_Size, long File_Offset);

        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfoA_Open(IntPtr Handle, long File_Size, long File_Offset);

        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfo_Open_Buffer_Continue(IntPtr Handle, IntPtr Buffer, IntPtr Buffer_Size);

        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfoA_Open_Buffer_Continue(IntPtr Handle, long File_Size, byte[] Buffer,
            IntPtr Buffer_Size);

        [DllImport("MediaInfo.dll")]
        private static extern long MediaInfo_Open_Buffer_Continue_GoTo_Get(IntPtr Handle);

        [DllImport("MediaInfo.dll")]
        private static extern long MediaInfoA_Open_Buffer_Continue_GoTo_Get(IntPtr Handle);

        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfo_Open_Buffer_Finalize(IntPtr Handle);

        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfoA_Open_Buffer_Finalize(IntPtr Handle);

        [DllImport("MediaInfo.dll")]
        private static extern void MediaInfo_Close(IntPtr Handle);

        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfo_Inform(IntPtr Handle, IntPtr Reserved);

        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfoA_Inform(IntPtr Handle, IntPtr Reserved);

        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfo_GetI(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber,
            IntPtr Parameter, IntPtr KindOfInfo);

        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfoA_GetI(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber,
            IntPtr Parameter, IntPtr KindOfInfo);

        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfo_Get(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber,
            [MarshalAs(UnmanagedType.LPWStr)] string Parameter, IntPtr KindOfInfo, IntPtr KindOfSearch);

        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfoA_Get(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber,
            IntPtr Parameter, IntPtr KindOfInfo, IntPtr KindOfSearch);

        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfo_Option(IntPtr Handle, [MarshalAs(UnmanagedType.LPWStr)] string Option,
            [MarshalAs(UnmanagedType.LPWStr)] string Value);

        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfoA_Option(IntPtr Handle, IntPtr Option, IntPtr Value);

        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfo_State_Get(IntPtr Handle);

        [DllImport("MediaInfo.dll")]
        private static extern IntPtr MediaInfo_Count_Get(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber);

        ~MediaInfoLib()
        {
            if (Handle == (IntPtr) 0) return;
            MediaInfo_Delete(Handle);
        }

        public int Open(string FileName)
        {
            if (Handle == (IntPtr) 0)
                return 0;
            if (MustUseAnsi)
            {
                var FileName_Ptr = Marshal.StringToHGlobalAnsi(FileName);
                var ToReturn = (int) MediaInfoA_Open(Handle, FileName_Ptr);
                Marshal.FreeHGlobal(FileName_Ptr);
                return ToReturn;
            }
            return (int) MediaInfo_Open(Handle, FileName);
        }

        public int Open_Buffer_Init(long File_Size, long File_Offset)
        {
            if (Handle == (IntPtr) 0) return 0;
            return (int) MediaInfo_Open_Buffer_Init(Handle, File_Size, File_Offset);
        }

        public int Open_Buffer_Continue(IntPtr Buffer, IntPtr Buffer_Size)
        {
            if (Handle == (IntPtr) 0) return 0;
            return (int) MediaInfo_Open_Buffer_Continue(Handle, Buffer, Buffer_Size);
        }

        public long Open_Buffer_Continue_GoTo_Get()
        {
            if (Handle == (IntPtr) 0) return 0;
            return MediaInfo_Open_Buffer_Continue_GoTo_Get(Handle);
        }

        public int Open_Buffer_Finalize()
        {
            if (Handle == (IntPtr) 0) return 0;
            return (int) MediaInfo_Open_Buffer_Finalize(Handle);
        }

        public void Close()
        {
            if (Handle == (IntPtr) 0) return;
            MediaInfo_Close(Handle);
        }

        public string Inform()
        {
            if (Handle == (IntPtr) 0)
                return "Unable to load MediaInfo library";
            if (MustUseAnsi)
                return Marshal.PtrToStringAnsi(MediaInfoA_Inform(Handle, (IntPtr) 0));
            return Marshal.PtrToStringUni(MediaInfo_Inform(Handle, (IntPtr) 0));
        }

        public string Get(StreamKind StreamKind, int StreamNumber, string Parameter, InfoKind KindOfInfo,
            InfoKind KindOfSearch)
        {
            if (Handle == (IntPtr) 0)
                return "Unable to load MediaInfo library";
            if (MustUseAnsi)
            {
                var Parameter_Ptr = Marshal.StringToHGlobalAnsi(Parameter);
                var ToReturn = Marshal.PtrToStringAnsi(MediaInfoA_Get(Handle, (IntPtr) StreamKind,
                    (IntPtr) StreamNumber, Parameter_Ptr, (IntPtr) KindOfInfo, (IntPtr) KindOfSearch));
                Marshal.FreeHGlobal(Parameter_Ptr);
                return ToReturn;
            }
            return Marshal.PtrToStringUni(MediaInfo_Get(Handle, (IntPtr) StreamKind, (IntPtr) StreamNumber, Parameter,
                (IntPtr) KindOfInfo, (IntPtr) KindOfSearch));
        }

        public string Get(StreamKind StreamKind, int StreamNumber, int Parameter, InfoKind KindOfInfo)
        {
            if (Handle == (IntPtr) 0)
                return "Unable to load MediaInfo library";
            if (MustUseAnsi)
                return Marshal.PtrToStringAnsi(MediaInfoA_GetI(Handle, (IntPtr) StreamKind, (IntPtr) StreamNumber,
                    (IntPtr) Parameter, (IntPtr) KindOfInfo));
            return Marshal.PtrToStringUni(MediaInfo_GetI(Handle, (IntPtr) StreamKind, (IntPtr) StreamNumber,
                (IntPtr) Parameter, (IntPtr) KindOfInfo));
        }

        public string Option(string Option, string Value)
        {
            if (Handle == (IntPtr) 0)
                return "Unable to load MediaInfo library";
            if (MustUseAnsi)
            {
                var Option_Ptr = Marshal.StringToHGlobalAnsi(Option);
                var Value_Ptr = Marshal.StringToHGlobalAnsi(Value);
                var ToReturn = Marshal.PtrToStringAnsi(MediaInfoA_Option(Handle, Option_Ptr, Value_Ptr));
                Marshal.FreeHGlobal(Option_Ptr);
                Marshal.FreeHGlobal(Value_Ptr);
                return ToReturn;
            }
            return Marshal.PtrToStringUni(MediaInfo_Option(Handle, Option, Value));
        }

        public int State_Get()
        {
            if (Handle == (IntPtr) 0) return 0;
            return (int) MediaInfo_State_Get(Handle);
        }

        public int Count_Get(StreamKind StreamKind, int StreamNumber)
        {
            if (Handle == (IntPtr) 0) return 0;
            return (int) MediaInfo_Count_Get(Handle, (IntPtr) StreamKind, (IntPtr) StreamNumber);
        }

        //Default values, if you know how to set default values in C#, say me
        public string Get(StreamKind StreamKind, int StreamNumber, string Parameter, InfoKind KindOfInfo)
        {
            return Get(StreamKind, StreamNumber, Parameter, KindOfInfo, InfoKind.Name);
        }

        public string Get(StreamKind StreamKind, int StreamNumber, string Parameter)
        {
            return Get(StreamKind, StreamNumber, Parameter, InfoKind.Text, InfoKind.Name);
        }

        public string Get(StreamKind StreamKind, int StreamNumber, int Parameter)
        {
            return Get(StreamKind, StreamNumber, Parameter, InfoKind.Text);
        }

        public string Option(string Option_)
        {
            return Option(Option_, "");
        }

        public int Count_Get(StreamKind StreamKind)
        {
            return Count_Get(StreamKind, -1);
        }
    }
} //NameSpace