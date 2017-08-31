﻿using System;
using Microsoft.Win32;
using System.Xml;
using Vexease.Controllers.Cryptography;
using Vexease.Models.Enums;

namespace Vexease.Models.Registrys
{
    /// <inheritdoc cref="RegKey" />
    /// <summary>
    /// </summary>
    [Serializable]
    public class RegStore : RegKey
    {
        /// <summary>
        /// 
        /// </summary>
        public bool IsNull { get; protected set; }
        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public RegStore() { }
        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="hKey"></param>
        /// <param name="lpSubKey"></param>
        /// <param name="lpValueName"></param>
        /// <param name="lpKind"></param>
        /// <param name="lpValue"></param>
        /// <param name="isNull"></param>
        public RegStore(
            REG_ROOT_KEY hKey,
            string lpSubKey,
            string lpValueName = "",
            RegistryValueKind lpKind = RegistryValueKind.Unknown,
            object lpValue = null,
            bool isNull = true) :
            base(hKey, lpSubKey, lpValueName, lpKind, lpValue)
        {
            IsNull = isNull;
        }
        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="regPath"></param>
        /// <param name="lpKind"></param>
        /// <param name="lpValue"></param>
        /// <param name="isNull"></param>
        public RegStore(
            RegPath regPath,
            RegistryValueKind lpKind = RegistryValueKind.Unknown,
            object lpValue = null,
            bool isNull = true) :
            base(regPath, lpKind, lpValue)
        {
            IsNull = isNull;
        }
        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="regKey"></param>
        /// <param name="isNull"></param>
        public RegStore(RegKey regKey, bool isNull = true) :
            base(regKey)
        {
            IsNull = isNull;
        }
        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="regStore"></param>
        public RegStore(RegStore regStore) :
            base(regStore.HKey, regStore.LpSubKey, regStore.LpValueName, regStore.LpKind, regStore.LpValue)
        {
            IsNull = regStore.IsNull;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="name"></param>
        protected new void MidExport(XmlTextWriter writer, string name)
        {
            base.MidExport(writer, name);
            writer.WriteAttributeString("isnull", AESCrypt.Encrypt(IsNull.ToString()));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public RegKey GetRegKey()
        {
            return new RegKey(GetRegPath(), LpKind, LpValue);
        }
        /// <summary>
        /// </summary>
        /// <returns></returns>
        public new object Clone()
        {
            return MemberwiseClone();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public new int CompareTo(object obj)
        {
            var regkey = obj as RegKey;
            if (regkey is null) throw new NullReferenceException();
            var flag = base.CompareTo(obj);
            if (flag != 0) return flag;
            return IsNull ? 1 : -1;
        }
    }
}
