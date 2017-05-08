
namespace EIV.OData.Client.Interface.Base
{
    public interface IODataResponse
    {
        int StatusCode { get; }
        string Message { get; }
        string Content { get; }
        string QueryUri { get; }

        void SetContent(string content);
    }
}