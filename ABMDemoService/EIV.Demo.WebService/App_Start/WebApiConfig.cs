/// <summary>
/// 
/// </summary>
namespace EIV.Demo.WebService
{
    using Model;
    using Microsoft.OData.Edm;
    using System.Web.Http;
    using System.Web.OData.Batch;
    using System.Web.OData.Builder;
    using System.Web.OData.Extensions;

    using System.Net.Http.Formatting;
    using System;
//    using System.Web.OData.Query;
    using Filters;

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //config.MapODataServiceRoute("odata", null, GetEdmModel(), new DefaultODataBatchHandler(GlobalConfiguration.DefaultServer));

            // hide .../$metadata & /
            // Then 'DataSvcUtil.exe' does not work any longer
            //var defaultConventions = ODataRoutingConventions.CreateDefault();
            //var conventions = defaultConventions.Except(
            //    defaultConventions.OfType<MetadataRoutingConvention>());

            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            //config.Filters.Add(new MenuExpandActionFilter());

            // Works Ok (for all entities?)
            // http://odata.github.io/WebApi/#13-01-modelbound-attribute
            // config.Count().Filter().OrderBy().Expand().Select().MaxTop(null);
            config.Count().Filter().OrderBy().Expand().Select();

            // People
            config.MapODataServiceRoute(
                "OData",
                "odata",
                GetEdmModel(config),
                new DefaultODataBatchHandler(GlobalConfiguration.DefaultServer));
            //routingConventions: conventions);

            config.EnableContinueOnErrorHeader();

            config.Formatters.Clear();                             //Remove all other formatters
            config.Formatters.Add(new JsonMediaTypeFormatter());   //Enable JSON in the web service

            // The ‘ObjectContent`1’ type failed to serialize the response body for content type ‘application/xml; charset=utf-8
            var json = config.Formatters.JsonFormatter;
            json.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;

            config.EnsureInitialized();

            //config.Filters.Add(new CustomAuthorize());
        }
        private static IEdmModel GetEdmModel(HttpConfiguration config)
        {
            //ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder(config);
            //builder.Namespace = "EIV.Demo.WebService";   // it will be used when generating the proxy class
            builder.Namespace = "EIV.Demo.WebService";
            //builder.ContainerName = "DefaultContainer"; // it will be used when generating the proxy class
            //builder.DataServiceVersion = new Version(2, 0);
            //builder.MaxDataServiceVersion = new Version(2, 0);

            builder.EntitySet<City>("Cities").EntityType.HasRequired<State>(x => x.State);

            builder.EntitySet<State>("States")
                .EntityType
                .HasRequired(o => o.Country, (o, c) => o.CountryId == c.Id);

            var countries = builder.EntitySet<Country>("Countries");

            /*builder.EntitySet<Country>("Country").EntityType
                .HasKey(k => k.Id)
                .HasMany(p => p.States);*/

            var states = builder.EntitySet<State>("States");

            states.HasRequiredBinding<Country>(p => p.Country, countries);

            //    .Select(new string[] { nameof(Country.Name) })
            //    .Filter(QueryOptionSetting.Allowed, new string[] { nameof(Country.Name) });

            builder.EntitySet<Client>("People").EntityType
                .HasKey(k => k.Id);
            //.Expand(new string[] { nameof(Client.Country) });

            //builder.EntityType<Client>().Filter("Name");

            // New
            builder.EntityType<MenuItem>();  // .Expand(new string[] { nameof(MenuItem.SubItems) }).Ignore(m => m.Name);
            //builder.EntitySet<MenuItem>("MenuItem");

            builder.EntitySet<User>("Users");

            // It appears in the $metadata
            var paisActionConfig = builder.EntityType<Country>().Action("PaisAction");

            paisActionConfig.Parameter<int>("sectorId");
            paisActionConfig.Parameter<DateTime>("fechaIngreso");
            paisActionConfig.CollectionParameter<State>("StateIDs");
            paisActionConfig.ReturnsFromEntitySet<Client>("People");

            var stateListActionConfig = builder.EntityType<Country>().Action("StateListAction");
            stateListActionConfig.CollectionParameter<State>("StateIDs");
            stateListActionConfig.Returns<int>();

            // CheckOut
            // URI: ~/odata/Movies(1)/ODataActionsSample.Models.CheckOut
            ActionConfiguration checkOutAction = builder.EntityType<Client>().Action("Transfer");
            //checkOutAction.ReturnsFromEntitySet<Client>("People");
            checkOutAction.ReturnsCollectionFromEntitySet<Client>("People");

            // Function here
            builder.EntityType<Country>().Collection
                .Function("TotalPaises")
                .Returns<int>();

            ActionConfiguration authenticateUser = builder.EntityType<EIV.Demo.Model.User>().Collection.Action("AuthenticateUser");
            authenticateUser.Parameter<string>("username");
            authenticateUser.Parameter<string>("password");
            //authenticateUser.EntityParameter<User>(typeof(User).Name);
            //authenticateUser.ReturnsCollectionFromEntitySet<EIV.Demo.Model.User>("Users");
            authenticateUser.ReturnsFromEntitySet<EIV.Demo.Model.User>("Users");
            //authenticateUser.Returns<IHttpActionResult>();

            //ActionConfiguration setUserMenu = builder.EntityType<User>().Action("SetMenu");
            ActionConfiguration setUserMenu = builder.EntityType<User>().Collection.Action("SetMenu");
            //setUserMenu.EntityParameter<User>(typeof(User).Name);
            setUserMenu.Parameter<string>("username");

            // Se ha leído un nodo de tipo 'StartObject' del lector JSON al intentar leer el principio del contenido de un conjunto de recursos; sin embargo, se esperaba un nodo de tipo 'StartArray'.
            //setUserMenu.CollectionEntityParameter<MenuItem>("menuitems");
            // Pass JSon string here from the client side!
            setUserMenu.Parameter<string>("menuitems");
            //setUserMenu.CollectionParameter<MenuItem>("menuitems");
            setUserMenu.Returns<bool>();

            //builder.Namespace = typeof(Country).Namespace;

            var edmModel = builder.GetEdmModel();

            return edmModel;
        }

        /*
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
        
        builder.EntitySet<Country>("Country")
                .EntityType
                .Select(new string[] { nameof(Country.Name) })
                .Filter(new string[] { nameof(Country.Name) });

            //builder.EntitySet<State>("State")
            //    .EntityType
            //    .Select(new string[] { nameof(State.Name) })
            //    .Filter(new string[] { nameof(State.Name) })
            //    .Expand(new string[] { nameof(State.Country) });

            builder.EntitySet<Client>("People")
                .EntityType
                .Select(new string[] { nameof(Client.FirstName) })
                .Filter(new string[] { nameof(Client.LastName) })
                .Expand(new string[] { nameof(Client.Country) });
         
         */
    }
}