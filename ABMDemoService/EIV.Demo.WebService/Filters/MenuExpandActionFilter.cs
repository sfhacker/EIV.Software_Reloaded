
namespace EIV.Demo.WebService.Filters
{
    using Model;
    using System;
    using System.Collections.Specialized;
    using System.Data.Objects;
    using System.Linq;
    using System.Net.Http;
    using System.Reflection;
    using System.Web;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;
    public class MenuExpandActionFilter : ActionFilterAttribute
    {
        private const string ODataSelectOption = "$expand=";
        private string selectValue = "MenuItems";

        // testing $expand here
        // http://stackoverflow.com/questions/27781607/exclude-property-from-webapi-odata-ef-response-in-c-sharp
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);

            var request = actionContext.Request;

            /*
            var parts = ODataSelectOption + this.selectValue;

            var modifiedRequestUri = new UriBuilder(request.RequestUri);
            modifiedRequestUri.Query += parts;
            request.RequestUri = modifiedRequestUri.Uri; */

            base.OnActionExecuting(actionContext);
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            HttpResponseMessage response = actionExecutedContext.Response;

            if (response != null && response.IsSuccessStatusCode && response.Content != null)
            {
                ObjectContent responseContent = response.Content as ObjectContent;
                if (responseContent == null)
                {
                    throw new ArgumentException("HttpRequestMessage's Content must be of type ObjectContent", "actionExecutedContext");
                }

                // Take the query returned to us by the EnableQueryAttribute and run it through out
                // NavigationPropertyFilterExpressionVisitor.

                var type = responseContent.ObjectType; //type of the returned object
                var value = responseContent.Value; //holding the returned value

                var thisUser = responseContent.Value as User;
                if (thisUser != null)
                {

                }
                // Always null
                IQueryable query = responseContent.Value as IQueryable;
                if (query != null)
                {
                    /*
                    var visitor = new NavigationPropertyFilterExpressionVisitor(_navigationFilterType);
                    var expressionWithFilter = visitor.Visit(query.Expression);
                    if (visitor.ModifiedExpression)
                        responseContent.Value = query.Provider.CreateQuery(expressionWithFilter);
                        */
                }
            }
            /*
            NameValueCollection querystringParams = HttpUtility.ParseQueryString(actionExecutedContext.Request.RequestUri.Query);
            string expandsQueryString = querystringParams["$expand"];

            if (string.IsNullOrWhiteSpace(expandsQueryString))
            {
                return;
            }

            object responseObject;

            actionExecutedContext.Response.TryGetContentValue(out responseObject);

            //ObjectQuery query = responseObject as ObjectQuery;
            IQueryable query = responseObject as IQueryable;
            if (query == null)
            {
                return;
            }

            MethodInfo info = query.GetType().GetMethod("Include", new Type[] { typeof(string) });

            expandsQueryString.Split(',').Select(s => s.Trim()).ToList().ForEach(
                expand => {
                    query = info.Invoke(query, new object[] { expand }) as ObjectQuery;
                });

            ((ObjectContent) actionExecutedContext.Response.Content).Value = query;
            */
        }
    }
}