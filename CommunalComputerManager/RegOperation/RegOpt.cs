﻿using System;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using CommunalComputerManager.Common;
using Microsoft.Win32;

namespace CommunalComputerManager.RegOperation
{
    /// <summary>
    /// 
    /// </summary>
    public static class RegOpt
    {
        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private const uint REG_OPENED_EXISTING_KEY = 0x2;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="regPath"></param>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public static RegKey RegGetValue(RegPath regPath)
        {
            RegKey regkey;
            try
            {
                var phkresult = RegOpenKey(regPath);
                uint lpcbdata = 0;
                NativeMethods.RegQueryValueEx(phkresult, regPath.LpValueName, UIntPtr.Zero, out RegistryValueKind lpkind, IntPtr.Zero, ref lpcbdata);
                if (lpcbdata == 0)
                {
                    NativeMethods.RegCloseKey(phkresult);
                    throw new Exception(@"注册表访问失败" + '\n' +@"无法获取缓冲区大小");
                }
                var lpdata = Marshal.AllocHGlobal((int)lpcbdata);
                var reggetvaluetemp = NativeMethods.RegQueryValueEx(phkresult, regPath.LpValueName, UIntPtr.Zero, out lpkind, lpdata, ref lpcbdata);
                NativeMethods.RegCloseKey(phkresult);
                if (reggetvaluetemp != (uint)ERROR_CODE.ERROR_SUCCESS)
                {
                    throw new Exception(@"注册表访问失败" + '\n' +reggetvaluetemp);
                }
                if (lpkind == RegistryValueKind.DWord)
                {
                    var lpdataint = Marshal.ReadInt32(lpdata);
                    regkey = new RegKey(regPath, lpkind, lpdataint, lpcbdata);
                }
                else if (lpkind == RegistryValueKind.QWord)
                {
                    var lpdataint = Marshal.ReadInt64(lpdata);
                    regkey = new RegKey(regPath, lpkind, lpdataint, lpcbdata);
                }
                else if (lpkind == RegistryValueKind.String)
                {
                    var lpdatastr = Marshal.PtrToStringUni(lpdata);
                    lpdatastr = lpdatastr?.Trim();
                    regkey = new RegKey(regPath, lpkind, lpdatastr, lpcbdata);
                }
                else
                {
                    throw new Exception(@"注册表访问失败" + '\n' +@"注册表数据类型异常");
                }
            }
            catch (Exception)
            {
                regkey = new RegKey(regPath);
            }
            return regkey;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="regKey"></param>
        public static void RegSetValue(RegKey regKey)
        {
            uint regsetvaluetmp, exists;
            var sa = new SECURITY_ATTRIBUTES();
            UIntPtr phkResult;
            if (Environment.Is64BitOperatingSystem)
            {
                regsetvaluetmp = NativeMethods.RegCreateKeyEx(regKey.HKey, regKey.LpSubKey, 0u, null,
                    (uint)Common.RegOpt.OPERATE_OPTION.REG_OPTION_NON_VOLATILE,
                    (uint)Common.RegOpt.KEY_SAM_FLAG.KEY_WOW64_64KEY | (uint)Common.RegOpt.KEY_ACCESS_TYPE.KEY_READ |
                    (uint)Common.RegOpt.KEY_ACCESS_TYPE.KEY_WRITE, ref sa, out phkResult, out exists);
            }
            else
            {
                regsetvaluetmp = NativeMethods.RegCreateKeyEx(regKey.HKey, regKey.LpSubKey, 0u, null,
                    (uint)Common.RegOpt.OPERATE_OPTION.REG_OPTION_NON_VOLATILE,
                    (uint)Common.RegOpt.KEY_ACCESS_TYPE.KEY_READ |
                    (uint)Common.RegOpt.KEY_ACCESS_TYPE.KEY_WRITE, ref sa, out phkResult, out exists);
            }
            if (regsetvaluetmp != (uint)ERROR_CODE.ERROR_SUCCESS && exists != REG_OPENED_EXISTING_KEY)
            {
                throw new Exception(@"注册表访问失败" + '\n' + regsetvaluetmp);
            }
            IntPtr lpdata;
            uint lpcbdata;
            if (regKey.LpKind == RegistryValueKind.String)
            {
                lpcbdata = (uint)((string)regKey.LpValue).Length + 1 << 1;
                lpdata = Marshal.StringToHGlobalUni((string)regKey.LpValue);
            }
            else if (regKey.LpKind == RegistryValueKind.DWord)
            {
                lpcbdata = (uint)Marshal.SizeOf(typeof(int));
                lpdata = Marshal.AllocHGlobal((int)lpcbdata);
                Marshal.WriteInt32(lpdata, (int)regKey.LpValue);
            }
            else if (regKey.LpKind == RegistryValueKind.QWord)
            {
                lpcbdata = (uint)Marshal.SizeOf(typeof(long));
                lpdata = Marshal.AllocHGlobal((int)lpcbdata);
                Marshal.WriteInt64(lpdata, (long)regKey.LpValue);
            }
            else
            {
                throw new Exception(@"注册表访问失败" + '\n' + regsetvaluetmp);
            }
            regsetvaluetmp =
                NativeMethods.RegSetValueEx(phkResult, regKey.LpValueName, 0u, regKey.LpKind, lpdata, lpcbdata);
            NativeMethods.RegCloseKey(phkResult);
            if (regsetvaluetmp != (uint)ERROR_CODE.ERROR_SUCCESS)
            {
                throw new Exception(@"注册表访问失败" + '\n' + regsetvaluetmp);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="regPath"></param>
        public static void RegDelKey(RegPath regPath)
        {
            uint regdelkeytmp;
            if (string.IsNullOrEmpty(regPath.LpValueName))
            {
                regdelkeytmp = NativeMethods.RegDeleteKeyEx(regPath.HKey, regPath.LpSubKey,
                    (uint)Common.RegOpt.KEY_SAM_FLAG.KEY_WOW64_64KEY | (uint)Common.RegOpt.KEY_ACCESS_TYPE.KEY_ALL_ACCESS, 0u);
                if (regdelkeytmp != (uint)ERROR_CODE.ERROR_SUCCESS)
                {
                    throw new Exception(@"注册表访问失败" + '\n' + regdelkeytmp);
                }
            }
            else
            {
                regdelkeytmp = NativeMethods.RegOpenKeyEx(regPath.HKey, regPath.LpSubKey, 0u,
                    (uint)Common.RegOpt.KEY_SAM_FLAG.KEY_WOW64_64KEY |
                    (uint)Common.RegOpt.KEY_ACCESS_TYPE.KEY_ALL_ACCESS, out UIntPtr phkresult);
                if (regdelkeytmp != (uint)ERROR_CODE.ERROR_SUCCESS)
                {
                    throw new Exception(@"注册表访问失败" + '\n' + regdelkeytmp);
                }
                regdelkeytmp = NativeMethods.RegDeleteValueEx(phkresult, regPath.LpValueName);
                if (regdelkeytmp != (uint)ERROR_CODE.ERROR_SUCCESS)
                {
                    throw new Exception(@"注册表访问失败" + '\n' +regdelkeytmp);
                }
                NativeMethods.RegCloseKey(phkresult);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="regPath"></param>
        /// <returns></returns>
        public static RegPath[] RegEnumName(RegPath regPath)
        {
            uint index = 0, size = 1023;
            var phkresult = RegOpenKey(regPath);
            var sc = new StringCollection();
            var sb = new StringBuilder(1024);
            while (NativeMethods.RegEnumKeyEx(phkresult, index, sb, ref size, IntPtr.Zero, IntPtr.Zero,
                       IntPtr.Zero, IntPtr.Zero) == (int)ERROR_CODE.ERROR_SUCCESS)
            {
                index++;
                sc.Add(sb.ToString());
            }
            var regpath = new RegPath[sc.Count];
            var str = new string[sc.Count];
            sc.CopyTo(str, 0);
            Array.Sort(str);
            for (var i = 0; i < str.Length; i++)
            {
                regpath[i] = new RegPath(regPath.HKey, regPath.LpSubKey, str[i]);
            }
            return regpath;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="regPath"></param>
        /// <returns></returns>
        private static UIntPtr RegOpenKey(RegPath regPath)
        {
            uint regopenkeytmp;
            UIntPtr phkresult;
            if (Environment.Is64BitOperatingSystem)
            {
                regopenkeytmp = NativeMethods.RegOpenKeyEx(regPath.HKey, regPath.LpSubKey, 0,
                    (uint)Common.RegOpt.KEY_SAM_FLAG.KEY_WOW64_64KEY |
                    (uint)Common.RegOpt.KEY_ACCESS_TYPE.KEY_READ, out phkresult);
            }
            else
            {
                regopenkeytmp = NativeMethods.RegOpenKeyEx(regPath.HKey, regPath.LpSubKey, 0,
                    (uint)Common.RegOpt.KEY_ACCESS_TYPE.KEY_READ, out phkresult);
            }
            if (regopenkeytmp == (uint) ERROR_CODE.ERROR_FILE_NOT_FOUND)
            {
                throw new NullReferenceException(@"注册表访问失败" + '\n' +regopenkeytmp);
            }
            if (regopenkeytmp != (uint)ERROR_CODE.ERROR_SUCCESS)
            {
                throw new Exception(@"注册表访问失败" + '\n' +regopenkeytmp);
            }
            return phkresult;
        }
    }
}
