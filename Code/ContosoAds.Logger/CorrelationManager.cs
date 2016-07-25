using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContosoAds.Logger
{
    using System.Runtime.Remoting.Messaging;

    public static class CorrelationManager
    {
        private const string OperationIdKey = "OperationId";

        public static void SetOperationId(string operationId)
        {
            CallContext.LogicalSetData(OperationIdKey, operationId);
        }

        public static string GetOperationId()
        {
            var id = CallContext.LogicalGetData(OperationIdKey) as string;
            return id ?? Guid.NewGuid().ToString();
        }
    }
}
