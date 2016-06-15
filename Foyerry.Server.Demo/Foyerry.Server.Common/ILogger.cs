using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Foyerry.Server.Common
{
    public interface ILogger
    {
        void Log(string message);
        void Log(string[] categories, string message);
        void LogException(Exception x);
    }
}
