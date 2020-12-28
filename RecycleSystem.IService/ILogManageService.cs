using RecycleSystem.Data.Data.LogDTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecycleSystem.IService
{
    public interface ILogManageService
    {
        IEnumerable<LoginLogOutput> GetLoginLogOutputs(int page, int limit, out int count);
        IEnumerable<OperateLogOutput> GetOperateLogOutputs(int page, int limit, out int count);
    }
}
