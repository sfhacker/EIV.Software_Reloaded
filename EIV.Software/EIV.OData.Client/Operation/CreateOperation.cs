
namespace EIV.OData.Client.Operation
{
    using System;
    using EIV.OData.Client.Operation.Base;
    using Interface.Base;
    using System.Net.Http;
    using Extension;
    using System.Net.Http.Headers;
    using Context;
    using Interface;

    public sealed class CreateOperation<T> : ODataOperation<T>, ICreateOperation where T : class
    {
        private const string MEDIA_TYPE_JSON = "application/json";

        private readonly IODataServiceContext serviceContext = null;

        private object entity;
        private string entitySet;
        public CreateOperation(IODataServiceContext context)
        {
            this.entity = null;
            this.entitySet = null;

            this.serviceContext = context;
        }
        public override string OperationUri
        {
            get
            {
                return this.serviceContext.GenerateQueryUri<T>(this.entitySet, null);
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
            if (this.entity == null)
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
            string query = this.serviceContext.GenerateQueryUri<T>(entitySetName, null);
            if (string.IsNullOrEmpty(query))
            {
                return null;
            }

            HttpContent postContent = new StringContent(this.entity.ObjectToJSon(), System.Text.Encoding.UTF8, MEDIA_TYPE_JSON);

            HttpClient _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MEDIA_TYPE_JSON));

            HttpResponseMessage response = _httpClient.PostAsync(query, postContent).Result;

            response.EnsureSuccessStatusCode();

            string result = response.Content.ReadAsStringAsync().Result;

            queryResponse = new ODataResponse(response);
            queryResponse.SetContent(result);

            return queryResponse;
        }

        public override bool Validate()
        {
            throw new NotImplementedException();
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
    }
}