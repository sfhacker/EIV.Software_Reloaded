
namespace EIV.OData.Client.Operation.Base
{
    using EIV.OData.Client.Interface.Base;
    public abstract class ODataOperation<T> : IODataOperation where T : class
    {
        public abstract string OperationUri { get; }

        public abstract IODataResponse Execute();
        public abstract string ToJSon();
        public abstract bool Validate();
    }
}