
namespace EIV.OData.Client.Context
{
    using System;
    using System.Collections.Generic;
    using EIV.OData.Client.Interface.Base;
    using Microsoft.OData.Client;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using Interface;

    public sealed class ODataServiceContext : IODataServiceContext
    {
        private const string MEDIA_TYPE_JSON = "application/json";

        private static volatile ODataServiceContext _instance = null;
        private static readonly object InstanceLoker = new object();

        // Microsoft.OData.Client
        private DataServiceContext container = null;

        private bool isConnected;
        private string batchRelativePath;

        private string userName = null;
        private string password = null;
        private string domainName = null;

        private ODataServiceContext()
        {
            this.isConnected = false;
            this.batchRelativePath = null;

            this.container = null;

            this.userName = Environment.UserName;
            this.domainName = Environment.UserDomainName;
        }

        public static ODataServiceContext Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (InstanceLoker)
                    {
                        if (_instance == null)
                            _instance = new ODataServiceContext();
                    }
                }

                return _instance;
            }
        }

        public string UserName
        {
            get
            {
                return this.userName;
            }
            set
            {
                this.userName = value;
            }
        }

        public string Password
        {
            internal get
            {
                return this.password;
            }
            set
            {
                this.password = value;
            }
        }

        public string DomainName
        {
            get
            {
                return this.domainName;
            }
            set
            {
                this.domainName = value;
            }
        }

        public bool IsConnected
        {
            get
            {
                return this.isConnected;
            }
        }

        public string BatchRelativePath
        {
            set
            {
                this.batchRelativePath = value;
            }
        }
        public bool Connect<X>(string serviceUri) where X : class
        {
            Uri thisUri;

            if (string.IsNullOrEmpty(serviceUri))
            {
                return false;
            }

            bool rst = Uri.TryCreate(serviceUri, UriKind.Absolute, out thisUri);
            if (!rst)
            {
                return false;
            }
            if (this.isConnected)
            {
                return true;
            }

            Type contextType = typeof(X);

            // Paranoic!
            if (!this.ValidateObjectType(contextType))
            {
                return false;
            }

            // TODO
            //DataServiceContext serviceCtxt = (DataServiceContext)X;
            try
            {
                // http://odata.github.io/odata.net/04-06-use-client-hooks-in-odata-client/
                this.container = Activator.CreateInstance(contextType, thisUri) as DataServiceContext;

                this.container.Format.UseJson();

                this.container.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;

                this.isConnected = true;

                return true;
            }
            catch (Exception ex)
            {
                // TODO
                //this.errors.Add(ex);
            }

            return false;
        }

        public void Disconnect()
        {
            if (!this.isConnected)
            {
                return;
            }

            this.container = null;

            this.isConnected = false;
        }

        public void Dispose()
        {
        }

        public IList<IODataResponse> ExecuteBatch(IList<IODataOperation> operations)
        {
            int contentId;
            IList<IODataResponse> rst;

            if (operations == null)
            {
                return null;
            }
            if (operations.Count < 1)
            {
                return null;
            }

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MEDIA_TYPE_JSON));

            // GenerateBatchUri
            Uri webServiceUri = new Uri(this.GenerateBatchUri());

            // Content-Type: {multipart/mixed; boundary="batch_fd5c0bfe-2775-4ab3-933c-34b3e913edc0"}
            MultipartContent content = new MultipartContent("mixed", "batch_" + Guid.NewGuid().ToString());

            MultipartContent changeSet = null;

            contentId = 1;

            Uri queryUri = null;
            string queryOperation = null;
            foreach (IODataOperation oper in operations)
            {
                queryOperation = oper.OperationUri;
                if (string.IsNullOrEmpty(queryOperation))
                {
                    continue;
                }

                queryUri = new Uri(queryOperation);
                if (!this.IsChangeSetOperation(oper))
                {
                    // Always GET here ?
                    HttpRequestMessage queryCustomers = new HttpRequestMessage(HttpMethod.Get, queryUri);
                    queryCustomers.Headers.Add("Accept", "application/json");

                    // Complains here!
                    //queryCustomers.Headers.Add("Content-Type", "application/json");
                    queryCustomers.Headers.Add("OData-MaxVersion", "4.0");
                    queryCustomers.Headers.Add("OData-Version", "4.0");

                    HttpMessageContent queryContent = new HttpMessageContent(queryCustomers);

                    // Default Content-Type: {application/http; msgtype=request}
                    //queryContent.Headers.Add("Content-Type", "application/http");

                    // Error 500 if we comment out the two lines below!
                    queryContent.Headers.Remove("Content-Type");
                    queryContent.Headers.Add("Content-Type", "application/http");
                    queryContent.Headers.Add("Content-Transfer-Encoding", "binary");

                    content.Add(queryContent);

                } else
                {
                    if (changeSet == null)
                    {
                        // First Time
                        // 1: a multipart content that represents the changeset container
                        changeSet = new MultipartContent("mixed", "changeset_" + Guid.NewGuid().ToString());
                    }

                    HttpMethod queryMethod = this.GetHttpMethod(oper);

                    HttpRequestMessage postRequest = new HttpRequestMessage(queryMethod, queryUri);
                    if (!queryMethod.Equals(HttpMethod.Delete))
                    {
                        HttpContent postContent = new StringContent(oper.ToJSon(), System.Text.Encoding.UTF8, MEDIA_TYPE_JSON);
                        postRequest.Content = postContent;
                    }

                    // 3: one message content per corresponding post request
                    HttpMessageContent postRequestContent = new HttpMessageContent(postRequest);
                    postRequestContent.Headers.Remove("Content-Type");
                    postRequestContent.Headers.Add("Content-Type", "application/http");
                    postRequestContent.Headers.Add("Content-Transfer-Encoding", "binary");
                    postRequestContent.Headers.Add("Content-ID", contentId.ToString());

                    // Add this POST content to the changeset
                    changeSet.Add(postRequestContent);

                    //content.Add(changeSet);

                    contentId++;
                }

            }
            if (changeSet != null)
            {
                if (contentId > 1)
                {
                    content.Add(changeSet);
                }
            }
            HttpRequestMessage batchRequest = new HttpRequestMessage(HttpMethod.Post, webServiceUri);
            batchRequest.Headers.ExpectContinue = false;
            //Associate the content with the message
            batchRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "Basic sfagsfdgtry5767657647874856859859yjdfhjhfjfjhfjf");

            batchRequest.Content = content;


            HttpResponseMessage response = client.SendAsync(batchRequest).Result;

            response.EnsureSuccessStatusCode();

            HttpResponseMessage queryResponse = null;

            rst = new List<IODataResponse>();

            MultipartMemoryStreamProvider responseContents = response.Content.ReadAsMultipartAsync().Result;

            foreach (var part in responseContents.Contents)
            {
                queryResponse = null;
                if (part.Headers.ContentType.MediaType.Equals("application/json"))
                {
                    queryResponse = part.ReadAsHttpResponseMessageAsync().Result;
                }
                if (part.Headers.ContentType.MediaType.Equals("application/http"))
                {
                    // is this correct???
                    part.Headers.Remove("Content-Type");
                    part.Headers.Add("Content-Type", "application/http; msgtype=response");

                    queryResponse = part.ReadAsHttpResponseMessageAsync().Result;
                }
                if (part.Headers.ContentType.MediaType.Equals("multipart/mixed"))
                {
                    // an exception here
                    //queryResponse = part.ReadAsHttpResponseMessageAsync().Result;
                    var testMultipart = part.ReadAsMultipartAsync().Result;
                    foreach (var item in testMultipart.Contents)
                    {
                        item.Headers.Remove("Content-Type");
                        item.Headers.Add("Content-Type", "application/http; msgtype=response");

                        queryResponse = item.ReadAsHttpResponseMessageAsync().Result;

                        IODataResponse oDataResponse = new ODataResponse(queryResponse);
                        if (queryResponse.Content != null)
                        {
                            string result_multi = queryResponse.Content.ReadAsStringAsync().Result;
                            if (!string.IsNullOrEmpty(result_multi))
                            {
                                oDataResponse.SetContent(result_multi);
                            }
                        }

                        rst.Add(oDataResponse);
                    }
                    queryResponse = null;
                }
                if (queryResponse != null)
                {
                    string result = queryResponse.Content.ReadAsStringAsync().Result;

                    IODataResponse oDataResponse = new ODataResponse(queryResponse);
                    oDataResponse.SetContent(result);

                    rst.Add(oDataResponse);
                }
                
            }

            return rst;
        }

        public string GenerateQueryUri<T>(string entitySet, IDictionary<string, object> entityParams) where T : class
        {
            UriOperationParameter[] myParams = null;

            DataServiceQuery<T> query = null;

            if (!this.isConnected)
            {
                return null;
            }
            if (string.IsNullOrEmpty(entitySet))
            {
                return null;
            }

            if (entityParams == null)
            {
                query = this.container.CreateQuery<T>(entitySet);
            } else {
                int count = entityParams.Count;
                if (count > 0)
                {
                    myParams = new UriOperationParameter[count];

                    int i = 0;
                    foreach (string paramKey in entityParams.Keys)
                    {
                        myParams[i++] = new UriOperationParameter(paramKey, entityParams[paramKey]);
                    }

                    query = this.container.CreateFunctionQuery<T>("", entitySet, false, myParams);
                }
            }
            if (query == null)
            {
                return null;
            }

            return query.ToString();
        }

        public string GenerateFunctionQueryUri<T>(string entitySet, string functionName, IDictionary<string, object> functionParams) where T : class
        {
            UriOperationParameter[] myParams = null;

            DataServiceQuery<T> query = null;

            if (!this.isConnected)
            {
                return null;
            }
            if (string.IsNullOrEmpty(functionName))
            {
                return null;
            }
            // Are they manadatory?
            /*if (functionParams == null)
            {
                return null;
            }*/

            if (functionParams != null)
            {
                int count = functionParams.Count;
                if (count > 0)
                {
                    myParams = new UriOperationParameter[count];

                    int i = 0;
                    foreach (string paramKey in functionParams.Keys)
                    {
                        myParams[i++] = new UriOperationParameter(paramKey, functionParams[paramKey]);
                    }
                }
            }
            //query = this.container.CreateFunctionQuery<T>(entitySet, "EIV.Demo.WebService.PaisAction", false, myParams);
            // First param cannot be null, but empty
            // Last param cannot be null
            if (myParams == null)
            {
                query = this.container.CreateFunctionQuery<T>(entitySet, functionName, false);
            } else
            {
                query = this.container.CreateFunctionQuery<T>(entitySet, functionName, false, myParams);
            }
            if (query == null)
            {
                return null;
            }

            return query.ToString();
        }

        private string GenerateBatchUri()
        {
            string batchUri = string.Format("{0}{1}", this.container.BaseUri, "$batch/");

            return batchUri;
        }

        private string GetEntityToJson(IODataOperation operation)
        {
            if (operation == null)
            {
                return null;
            }
            if (!operation.Validate())
            {
                return null;
            }
            if (operation is ICreateOperation)
            {
                //Operation.CreateOperation<E> one = (operation.to as ICreateOperation);
                //object entity = 
            }

            return null;
        }
        // Please, refactor
        private HttpMethod GetHttpMethod(IODataOperation operation)
        {
            if (operation == null)
            {
                return null;
            }
            if (!operation.Validate())
            {
                return null;
            }
            if (operation is IQueryOperation)
            {
                return HttpMethod.Get;
            }
            if (operation is IUpdateOperation)
            {
                // There is not Patch here!
                return HttpMethod.Put;
            }
            if (operation is IFunctionOperation)
            {
                return HttpMethod.Get;
            }
            if (operation is IActionOperation)
            {
                return HttpMethod.Post;
            }
            if (operation is IDeleteOperation)
            {
                return HttpMethod.Delete;
            }

            return null;
        }

        private bool IsChangeSetOperation(IODataOperation operation)
        {
            if (operation == null)
            {
                // should throw an exception
                return false;
            }
            if (!operation.Validate())
            {
                // should throw an exception
                return false;
            }

            if ((operation is IActionOperation) || (operation is IFunctionOperation) || (operation is IQueryOperation))
            {
                return false;
            }

            return true;
        }
        // BaseType = {Name = "DataServiceContext" FullName = "Microsoft.OData.Client.DataServiceContext"}
        private bool ValidateObjectType(Type objectType)
        {
            if (objectType == null)
            {
                return false;
            }

            if (objectType.BaseType != null)
            {
                string baseTypeName = objectType.BaseType.Name;
                string baseTypeFullName = objectType.BaseType.FullName;

                if (string.IsNullOrEmpty(baseTypeName) || string.IsNullOrEmpty(baseTypeFullName))
                {
                    return false;
                }

                // This should be a constant
                if (!baseTypeName.Equals("DataServiceContext"))
                {
                    return false;
                }

                // This should be a constant
                if (!baseTypeFullName.Equals("Microsoft.OData.Client.DataServiceContext"))
                {
                    return false;
                }

                return true;
            }

            return false;
        }
    }
}