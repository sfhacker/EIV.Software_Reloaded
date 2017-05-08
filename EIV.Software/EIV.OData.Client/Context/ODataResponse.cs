
namespace EIV.OData.Client.Context
{
    using EIV.OData.Client.Interface.Base;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Net.Http;

    public sealed class ODataResponse : IODataResponse
    {
        private int statusCode = 0;
        private string queryUri = string.Empty;
        private string reasonPhrase = string.Empty;
        private string responseContent = string.Empty;

        // it throws an exception here:
        // 'El valor de Count no forma parte del flujo de respuesta.'
        // this.totalCount = response.TotalCount;
        private long totalCount = -1;

        private Type entityType = null;
        private IEnumerator items = null;
        private IEnumerable<object> itemList = null;

        private string entityState = string.Empty;

        private string errorMessage = string.Empty;

        //private IList<IEnumerable> queries = null;

        internal ODataResponse(HttpResponseMessage response)
        {
            if (response == null)
            {
                return;
            }

            this.statusCode = (int)response.StatusCode;
            this.reasonPhrase = response.ReasonPhrase;

            // Always null here!
            if (response.RequestMessage != null)
            {
                this.queryUri = response.RequestMessage.RequestUri.ToString();
            }

            // 
            HttpContent httpContent = response.Content;
            if (httpContent == null)
            {
                return;
            }

            long? len = httpContent.Headers.ContentLength;
            if (len != null)
            {
                if (len > 0)
                {
                    //httpContent.
                }
            }
        }

        public int StatusCode
        {
            get
            {
                return this.statusCode;
            }
        }

        public Type EntityType
        {
            get
            {
                return this.entityType;
            }
            internal set
            {
                this.entityType = value;
            }
        }

        public string EntityState
        {
            get
            {
                return this.entityState;
            }
            internal set
            {
                this.entityState = value;
            }
        }

        public IEnumerator Items
        {
            get
            {
                return this.items;
            }
            internal set
            {
                this.items = value;
            }
        }

        public IEnumerable<object> ItemList
        {
            get
            {
                return this.itemList;
            }
            internal set
            {
                this.itemList = value;
            }
        }
        public string ErrorMessage
        {
            get
            {
                return this.errorMessage;
            }
            internal set
            {
                this.errorMessage = value;
            }
        }
        public string QueryUri
        {
            get
            {
                return this.queryUri;
            }
            internal set
            {
                this.queryUri = value;
            }
        }

        public string Message
        {
            get
            {
                return this.reasonPhrase;
            }
        }

        public string Content
        {
            get
            {
                return this.responseContent;
            }
        }

        public void SetContent(string content)
        {
            this.responseContent = content;
        }
    }
}