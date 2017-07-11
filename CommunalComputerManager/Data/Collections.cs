﻿using System;
using CommunalComputerManager.Controllers.Status;
using CommunalComputerManager.Models.Enums;
using CommunalComputerManager.Models.Registrys;
using Microsoft.Win32;

namespace CommunalComputerManager.Data
{
    public class Collections
    {
        public RegStatus LimitProc { get; }
        public RegStatus LimitSys { get; }
        public RegStatus Mmc { get; }
        public RegStatus Registry { get; }
        public RegStatus Tasksch { get; }
        public RegStatus CtrlPal { get; }
        public RegStatus Launcher { get; }
        public RegStatus Cmd { get; }
        public RegStatus Taskmgr { get; }
        public RegStatus PwrShell { get; }

        public Collections()
        {
            LimitProc = InitLimitProc();
            LimitSys = InitLimitSys();
            Registry = InitRegistry();
            Cmd = InitCmd();
            Taskmgr = InitTaskmgr();
            Launcher = InitLauncher();
            CtrlPal = InitCtrlPal();
        }

        private static RegStatus InitLimitProc()
        {
            var regp = new RegPath(new UIntPtr((uint)REG_ROOT_KEY.HKEY_LOCAL_MACHINE), @"Software\Policies\Microsoft\Windows\safer\codeidentifiers", @"DefaultLevel");
            var onreg = new RegStore[1];
            var offreg = new RegStore[1];
            onreg[0] = new RegStore(regp, RegistryValueKind.DWord, 0x0, sizeof(int), false);
            offreg[0] = new RegStore(regp, RegistryValueKind.DWord, 0x40000, sizeof(int));
            return new RegStatus(onreg, offreg);
        }

        private static RegStatus InitLimitSys()
        {
            var regp = new RegPath(new UIntPtr((uint)REG_ROOT_KEY.HKEY_CURRENT_USER), @"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", @"NoDrives");
            var onreg = new RegStore[1];
            var offreg = new RegStore[1];
            onreg[0] = new RegStore(regp, RegistryValueKind.DWord, 0x4, sizeof(int), false);
            offreg[0] = new RegStore(regp, RegistryValueKind.DWord, 0x0, sizeof(int));
            return new RegStatus(onreg, offreg);
        }

        private static RegStatus InitRegistry()
        {
            var regp = new RegPath(new UIntPtr((uint)REG_ROOT_KEY.HKEY_CURRENT_USER), @"Software\Microsoft\Windows\CurrentVersion\Policies\System", @"DisableRegistryTools");
            var onreg = new RegStore[1];
            var offreg = new RegStore[1];
            onreg[0] = new RegStore(regp, RegistryValueKind.DWord, 0x0, sizeof(int));
            offreg[0] = new RegStore(regp, RegistryValueKind.DWord, 0x2, sizeof(int), false);
            return new RegStatus(onreg, offreg);
        }

        private static RegStatus InitCmd()
        {
            var regp = new RegPath(new UIntPtr((uint)REG_ROOT_KEY.HKEY_CURRENT_USER), @"Software\Microsoft\Windows\CurrentVersion\Policies\System", @"DisableCMD");
            var onreg = new RegStore[1];
            var offreg = new RegStore[1];
            onreg[0] = new RegStore(regp, RegistryValueKind.DWord, 0x0, sizeof(int));
            offreg[0] = new RegStore(regp, RegistryValueKind.DWord, 0x2, sizeof(int), false);
            return new RegStatus(onreg, offreg);
        }
        private static RegStatus InitTaskmgr()
        {
            var regp = new RegPath(new UIntPtr((uint)REG_ROOT_KEY.HKEY_CURRENT_USER), @"Software\Microsoft\Windows\CurrentVersion\Policies\System", @"DisableTaskMgr");
            var onreg = new RegStore[1];
            var offreg = new RegStore[1];
            onreg[0] = new RegStore(regp, RegistryValueKind.DWord, 0x0, sizeof(int));
            offreg[0] = new RegStore(regp, RegistryValueKind.DWord, 0x1, sizeof(int), false);
            return new RegStatus(onreg, offreg);
        }
        private static RegStatus InitLauncher()
        {
            var regp = new RegPath(new UIntPtr((uint)REG_ROOT_KEY.HKEY_CURRENT_USER), @"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", @"NoRun");
            var onreg = new RegStore[1];
            var offreg = new RegStore[1];
            onreg[0] = new RegStore(regp, RegistryValueKind.DWord, 0x0, sizeof(int));
            offreg[0] = new RegStore(regp, RegistryValueKind.DWord, 0x2, sizeof(int), false);
            return new RegStatus(onreg, offreg);
        }
        private static RegStatus InitCtrlPal()
        {
            var regp = new RegPath(new UIntPtr((uint)REG_ROOT_KEY.HKEY_CURRENT_USER), @"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", @"NoControlPanel");
            var onreg = new RegStore[1];
            var offreg = new RegStore[1];
            onreg[0] = new RegStore(regp, RegistryValueKind.DWord, 0x0, sizeof(int));
            offreg[0] = new RegStore(regp, RegistryValueKind.DWord, 0x2, sizeof(int), false);
            return new RegStatus(onreg, offreg);
        }
    }
}