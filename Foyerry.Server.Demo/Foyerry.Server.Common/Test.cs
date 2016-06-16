using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;

namespace Foyerry.Server.Common
{
    public class Test
    {
        protected static ILog _log = LogManager.GetLogger(typeof(Test));

        public static void Do()
        {
            _log.Info("测试其他类库");
            Logger.Instance.Write("新的测试日志记录",MessageType.Info);
            AOP.Define.Do(() =>
            {
                
            });
        }
    }
}
