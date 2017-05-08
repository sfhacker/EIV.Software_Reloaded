
namespace EIV.OData.Client.Operation
{
    using System;
    using EIV.OData.Client.Interface.Base;
    using EIV.OData.Client.Operation.Base;
    using Extension;
    using Context;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Collections.Generic;
    using Interface;

    public sealed class UpdateOperation<T> : ODataOperation<T>, IUpdateOperation where T : class
    {
        private const string MEDIA_TYPE_JSON = "application/json";

        private readonly IODataServiceContext serviceContext = null;

        private object entity;
        private string entitySet;
        private IDictionary<string, object> entityParams;
        public UpdateOperation(IODataServiceContext context)
        {
            this.entity = null;
            this.entitySet = null;
            this.entityParams = null;

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

        // Mandatory?
        public void SetEntityParams(IDictionary<string, object> entityParams)
        {
            if (entityParams == null)
            {
                return;
            }
            this.entityParams = entityParams;
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
            if (this.entityParams == null)
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

            string query = this.serviceContext.GenerateQueryUri<T>(entitySetName, this.entityParams);
            if (string.IsNullOrEmpty(query))
            {
                return null;
            }

            HttpContent postContent = new StringContent(this.entity.ObjectToJSon(), System.Text.Encoding.UTF8, MEDIA_TYPE_JSON);

            HttpClient _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MEDIA_TYPE_JSON));

            HttpResponseMessage response = _httpClient.PutAsync(query, postContent).Result;

            response.EnsureSuccessStatusCode();

            string result = response.Content.ReadAsStringAsync().Result;

            queryResponse = new ODataResponse(response);
            queryResponse.SetContent(result);

            return queryResponse;
        }

        public override bool Validate()
        {
            return true;
        }
    }
}