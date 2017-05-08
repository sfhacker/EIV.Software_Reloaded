
namespace EIV.OData.Client.Operation
{
    using System;
    using EIV.OData.Client.Interface.Base;
    using EIV.OData.Client.Operation.Base;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using Context;
    using System.Collections.ObjectModel;
    using Interface;

    // Entity / GET
    public sealed class QueryOperation<T> : ODataOperation<T>, IQueryOperation where T : class
    {
        private const string MEDIA_TYPE_JSON = "application/json";

        private readonly IODataServiceContext serviceContext = null;

        private string entitySet;
        private IDictionary<string, object> entityParams;
        private IDictionary<string, object> filters;
        public QueryOperation(IODataServiceContext context)
        {
            this.entitySet = null;
            this.entityParams = null;
            this.filters = null;

            this.serviceContext = context;
        }

        public override string OperationUri
        {
            get
            {
                return this.serviceContext.GenerateQueryUri<T>(this.entitySet, this.entityParams);
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

        // Optional
        public void SetEntityParams(IDictionary<string, object> queryParams)
        {
            if (queryParams == null)
            {
                return;
            }
            this.entityParams = queryParams;
        }

        // Optional
        public void SetFilters(IDictionary<string, object> queryFilters)
        {
            if (queryFilters == null)
            {
                return;
            }
            this.filters = queryFilters;
        }

        // Not used here!
        public override string ToJSon()
        {
            return null;
        }

        public ObservableCollection<T> ParseResponse(IODataResponse response)
        {
            if (response == null)
            {
                return null;
            }
            if (string.IsNullOrEmpty(response.Content))
            {
                return null;
            }

            Newtonsoft.Json.Linq.JObject testObj = Newtonsoft.Json.Linq.JObject.Parse(response.Content);
            if (testObj == null)
            {
                return null;
            }

            Newtonsoft.Json.Linq.JToken jsonValue = testObj.GetValue("value");
            if (jsonValue == null)
            {
                return null;
            }
            if (jsonValue.Type == Newtonsoft.Json.Linq.JTokenType.Array)
            {

            }
            string jsonString = jsonValue.ToString();

            var checkObject = Newtonsoft.Json.JsonConvert.DeserializeObject<ObservableCollection<T>>(jsonString);

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
            string queryUri = this.serviceContext.GenerateQueryUri<T>(entitySetName, this.entityParams);
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