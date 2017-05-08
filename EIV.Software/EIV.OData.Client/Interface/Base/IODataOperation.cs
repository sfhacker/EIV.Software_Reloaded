
namespace EIV.OData.Client.Interface.Base
{
    public interface IODataOperation
    {
        string OperationUri { get; }
        string ToJSon();
        IODataResponse Execute();
        bool Validate();
    }
}