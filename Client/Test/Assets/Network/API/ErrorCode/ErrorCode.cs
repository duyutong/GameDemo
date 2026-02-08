using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Network.API.ErrorCode
{
    public enum ErrorCode
    {
        None = 0,
        InvalidAccount = 1001,
        InvalidPassword = 1002,
        AccountNotExists = 1003,
        PermissionDenied = 2001,
    }
}
