
namespace EIV.OData.Client.Operation
{
    using System;
    using EIV.OData.Client.Interface.Base;
    using EIV.OData.Client.Operation.Base;
    using Extension;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using Context;
    using System.Collections.Generic;
    using Interface;

    public sealed class FunctionOperation<T> : ODataOperation<T>, IFunctionOperation where T : class
    {
        private const string MEDIA_TYPE_JSON = "application/json";

        private readonly IODataServiceContext serviceContext = null;

        private object entity;
        private string entitySet;
        private string functionName;

        IDictionary<string, object> functionParameters;

        public FunctionOperation(IODataServiceContext context)
        {
            this.entity = null;
            this.entitySet = null;
            this.functionName = null;
            this.functionParameters = null;

            this.serviceContext = context;
        }

        public override string OperationUri
        {
            get
            {
                //return this.serviceContext.GenerateQueryUri<T>(this.entitySet, this.functionParameters);
                return this.serviceContext.GenerateFunctionQueryUri<T>(this.entitySet, this.functionName, this.functionParameters);
            }
        }

        // Optional
        // if EntitySet prop is null, then
        // use name that comes from 'T'
        // is this correct?
        public string EntitySet
        {
            /*get
            {
                return this.entitySet;
            }*/
            set
            {
                this.entitySet = value;
            }
        }

        public object Entity
        {
            /*get
            {
                return this.entity;
            }*/
            set
            {
                this.entity = value;
            }
        }

        public string FunctionName
        {
            set
            {
                this.functionName = value;
            }
        }

        // Optional
        public void SetParameters(IDictionary<string, object> queryParams)
        {
            if (queryParams == null)
            {
                return;
            }
            this.functionParameters = queryParams;
        }

        public override string ToJSon()
        {
            if (this.entity == null)
            {
                return null;
            }

            string rst = this.entity.ObjectToJSon();

            return rst;
        }

        public V ParseResponse<V>(IODataResponse response)
        {
            if (response == null)
            {
                return default(V);
            }
            if (string.IsNullOrEmpty(response.Content))
            {
                return default(V);
            }

            Newtonsoft.Json.Linq.JObject testObj = Newtonsoft.Json.Linq.JObject.Parse(response.Content);
            if (testObj == null)
            {
                return default(V);
            }

            Newtonsoft.Json.Linq.JToken jsonValue = testObj.GetValue("value");
            if (jsonValue == null)
            {
                return default(V);
            }
            if (jsonValue.Type == Newtonsoft.Json.Linq.JTokenType.Array)
            {

            }
            string jsonString = jsonValue.ToString();

            var checkObject = Newtonsoft.Json.JsonConvert.DeserializeObject<V>(jsonString);

            return checkObject;
        }
        public override IODataResponse Execute()
        {
            IODataResponse queryResponse = null;

            string entitySetName = string.Empty;

            if (this.serviceContext == null)
            {
                return null;
            }
            if (!this.serviceContext.IsConnected)
            {
                return null;
            }
            // Full Function Name
            if (string.IsNullOrEmpty(this.functionName))
            {
                return null;
            }

            if (string.IsNullOrEmpty(this.entitySet))
            {
                Type entityType = typeof(T);

                entitySetName = entityType.Name;
            }
            else
            {
                entitySetName = this.entitySet.Trim();
            }

            // TO DO
            string queryUri = this.serviceContext.GenerateFunctionQueryUri<T>(this.entitySet, this.functionName, this.functionParameters);
            if (string.IsNullOrEmpty(queryUri))
            {
                return null;
            }

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MEDIA_TYPE_JSON));

            Uri queryUriObject = new Uri(queryUri);

            HttpRequestMessage queryCustomers = new HttpRequestMessage(HttpMethod.Get, queryUriObject);

            HttpResponseMessage response = client.SendAsync(queryCustomers).Result;

            response.EnsureSuccessStatusCode();

            // {"@odata.context":"http://localhost:1860/odata/$metadata#Users","value":[{"Name":"Administrador","FechaAlta":"0001-01-01T00:00:00Z","FechaBaja":"0001-01-01T00:00:00Z","Id":1},{"Name":"Usuario 1","FechaAlta":"0001-01-01T00:00:00Z","FechaBaja":"0001-01-01T00:00:00Z","Id":2}]}
            string result = response.Content.ReadAsStringAsync().Result;

            queryResponse = new ODataResponse(response);
            queryResponse.SetContent(result);

            client.Dispose();
            client = null;

            return queryResponse;
        }

        public override bool Validate()
        {
            return true;
        }
    }
}