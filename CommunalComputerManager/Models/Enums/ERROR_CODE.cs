﻿// ReSharper disable InconsistentNaming
namespace CommunalComputerManager.Models.Enums
{
    public enum ERROR_CODE : uint
    {
        ERROR_SUCCESS = 0x0,
        ERROR_FILE_NOT_FOUND = 0x2,
        ERROR_PATH_NOT_FOUND = 0x3,
        ERROR_ACCESS_DENIED = 0x5,
        ERROR_INVALID_HANDLE = 0x6,
        ERROR_INVALID_PARAMETER = 0x57,
        ERROR_NOACCESS = 0x3E6,
        ERROR_MORE_DATA = 0xEA,
        ERROR_NO_MORE_ITEMS = 0x103
    }
}