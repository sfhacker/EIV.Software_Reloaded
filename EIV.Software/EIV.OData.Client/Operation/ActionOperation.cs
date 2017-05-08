
namespace EIV.OData.Client.Operation
{
    using System;
    using EIV.OData.Client.Operation.Base;
    using Interface.Base;
    using System.Net.Http;
    using System.Collections.Generic;
    using System.Net.Http.Headers;
    using Extension;
    using Context;
    using Newtonsoft.Json.Linq;
    using Interface;

    public sealed class ActionOperation<T> : ODataOperation<T>, IActionOperation where T : class
    {
        private const string MEDIA_TYPE_JSON = "application/json";

        private readonly IODataServiceContext serviceContext = null;

        private HttpClient _httpClient = null;

        private string entitySet = null;
        private object entity = null;

        private string actionName = null;
        private IDictionary<string, object> entityParams = null;
        private IDictionary<string, object> actionParams = null;

        public ActionOperation(IODataServiceContext context)
        {
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

        public string ActionName
        {
            set
            {
                this.actionName = value;
            }
        }
        // Optional?
        public void SetEntity(object entity)
        {
            if (entity != null)
            {
                this.entity = entity;
            }
        }

        // Optional
        public void SetEntityParams(IDictionary<string, object> queryFilters)
        {
            if (queryFilters == null)
            {
                return;
            }
            this.entityParams = queryFilters;
        }

        // Can they be optional?
        public void SetParameters(IDictionary<string, object> parameters)
        {
            if (parameters != null)
            {
                this.actionParams = parameters;
            }
        }

        public override IODataResponse Execute()
        {
            IODataResponse queryResponse = null;

            //HttpContent postContent = null;
            JObject postContent = null;
            string entitySetName = string.Empty;

            if (this.serviceContext == null)
            {
                return null;
            }
            if (!this.serviceContext.IsConnected)
            {
                return null;
            }
            if (string.IsNullOrEmpty(this.actionName))
            {
                return null;
            }
            // Optional ???
            /*
            if (this.entity == null)
            {
                return;
            }*/
            // Optional ???
            /*
            if (this.actionParams == null)
            {
                return;
            }*/
            if ((this.entity == null) && (this.actionParams == null))
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
            // this.filters: it does not work!
            string query = this.serviceContext.GenerateQueryUri<T>(entitySetName, this.entityParams);
            if (string.IsNullOrEmpty(query))
            {
                return null;
            }

            var queryString = string.Format("{0}/{1}", query, actionName);

            if (this.actionParams != null)
            {
                postContent = this.actionParams.DictionaryToJSon();
            } else
            {
                // Is this branch allowed?
                // I believe, yes
            }

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MEDIA_TYPE_JSON));

            HttpResponseMessage response = null;
            if (postContent == null)
            {
                response = _httpClient.PostAsync(queryString, null).Result;
            } else
            {
                response = _httpClient.PostAsJsonAsync(queryString, postContent).Result;
            }

            try
            {
                response.EnsureSuccessStatusCode();

                string result = response.Content.ReadAsStringAsync().Result;

                queryResponse = new ODataResponse(response);
                queryResponse.SetContent(result);
            }
            catch (Exception ex)
            {
                Type errorType = ex.GetType();

                // TODO:
                HttpRequestException reqEx = ex as HttpRequestException;

                if (reqEx.InnerException != null)
                {
                    Type testOne = ex.InnerException.GetType();
                }

                //this.StatusCode = -1;
            }

            _httpClient.Dispose();
            _httpClient = null;

            return queryResponse;
        }

        public override bool Validate()
        {
            return true;
        }

        public override string ToJSon()
        {
            throw new NotImplementedException();
        }
    }
}