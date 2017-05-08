
namespace EIV.OData.Client.Interface.Base
{
    using System;
    using System.Collections.Generic;

    public interface IODataServiceContext : IDisposable
    {
        bool IsConnected { get; }

        bool Connect<X>(string serviceUri) where X : class;
        void Disconnect();

        // params: Id=2
        string GenerateQueryUri<T>(string entitySet, IDictionary<string, object> entityParams) where T : class;
        string GenerateFunctionQueryUri<T>(string entitySet, string functionName, IDictionary<string, object> functionParams) where T : class;

        IList<IODataResponse> ExecuteBatch(IList<IODataOperation> operations);
    }
}