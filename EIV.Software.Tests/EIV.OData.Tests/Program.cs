
namespace EIV.OData.Tests
{
    using Client.Context;
    using Client.Interface.Base;
    using Client.Operation;
    using System;
    using System.Collections.Generic;

    public class Program
    {
        public const string ODATA_SERVICE_URL = "http://localhost:1860/odata/";

        private static IODataServiceContext service = null;

        static void Main(string[] args)
        {
            //service = new ODataServiceContext();
            service = ODataServiceContext.Instance;

            bool rst = service.Connect<EIV.Demo.WebService.Container>(ODATA_SERVICE_URL);
            if (!rst)
            {
                return;
            }

            //TestCreateOperation();
            //TestUpdateOperation();
            //TestDeleteOperation();
            //TestQueryOperation();
            //TestFunctionOperation();
            //TestActionOperation();

            TestBatchOperations();

            service.Disconnect();

            Console.ReadLine();
        }

        private static void TestCreateOperation()
        {
            IODataResponse queryResponse = null;

            EIV.Demo.Model.User newUser = new EIV.Demo.Model.User() { Id = 66, Name = "New User 2017" };

            CreateOperation<EIV.Demo.Model.User> queryEntity = new CreateOperation<EIV.Demo.Model.User>(service);
            queryEntity.EntitySet = "Users";
            queryEntity.Entity = newUser;

            queryResponse = queryEntity.Execute();

            Console.WriteLine("Query Response: " + queryResponse.StatusCode + " " + queryResponse.Message);
            Console.WriteLine("Query Content: " + queryResponse.Content);
        }

        private static void TestUpdateOperation()
        {
            IODataResponse queryResponse = null;

            EIV.Demo.Model.Country modCountry = new EIV.Demo.Model.Country() { Id = 2, Name = "Argentina (Mod)" };

            IDictionary<string, object> filters = new Dictionary<string, object>();
            filters.Add("Id", 2);

            UpdateOperation<EIV.Demo.Model.Country> queryEntity = new UpdateOperation<EIV.Demo.Model.Country>(service);
            queryEntity.EntitySet = "Countries";
            queryEntity.Entity = modCountry;
            queryEntity.SetEntityParams(filters);

            queryResponse = queryEntity.Execute();

            Console.WriteLine("Query Response: " + queryResponse.StatusCode + " " + queryResponse.Message);
            Console.WriteLine("Query Content: " + queryResponse.Content);
        }

        private static void TestDeleteOperation()
        {
            IODataResponse queryResponse = null;

            IDictionary<string, object> filters = new Dictionary<string, object>();
            filters.Add("Id", 3);

            DeleteOperation<EIV.Demo.Model.Country> queryEntity = new DeleteOperation<EIV.Demo.Model.Country>(service);
            queryEntity.EntitySet = "Countries";
            queryEntity.SetEntityParams(filters);

            queryResponse = queryEntity.Execute();

            Console.WriteLine("Query Response: " + queryResponse.StatusCode + " " + queryResponse.Message);
            Console.WriteLine("Query Content: " + queryResponse.Content);
        }

        private static void TestQueryOperation()
        {
            IODataResponse queryResponse = null;

            string query = service.GenerateQueryUri<EIV.Demo.Model.User>("Users", null);

            Console.WriteLine("Query Uri: " + query);

            IDictionary<string, object> filters = new Dictionary<string, object>();
            filters.Add("$expand", "MenuItems");
            filters.Add("$count", "true");
            filters.Add("$filter", "Id eq 1");

            QueryOperation<EIV.Demo.Model.User> queryEntity = new QueryOperation<EIV.Demo.Model.User>(service);
            queryEntity.EntitySet = "Users";
            queryEntity.SetFilters(filters);

            queryResponse = queryEntity.Execute();

            Console.WriteLine("Query Response: " + queryResponse.StatusCode + " " + queryResponse.Message);
            Console.WriteLine("Query Content: " + queryResponse.Content);

            var one = queryEntity.ParseResponse(queryResponse);

        }

        private static void TestFunctionOperation()
        {
            IODataResponse queryResponse = null;

            FunctionOperation<EIV.Demo.Model.Country> queryEntity = new FunctionOperation<EIV.Demo.Model.Country>(service);
            queryEntity.EntitySet = "Countries";
            queryEntity.FunctionName = "EIV.Demo.WebService.TotalPaises";

            queryResponse = queryEntity.Execute();

            Console.WriteLine("Query Response: " + queryResponse.StatusCode + " " + queryResponse.Message);
            Console.WriteLine("Query Content: " + queryResponse.Content);

            var one = queryEntity.ParseResponse<int>(queryResponse);

        }

        private static void TestActionOperation()
        {
            IODataResponse queryResponse = null;

            IList<EIV.Demo.Model.State> stateList = new List<Demo.Model.State>();
            stateList.Add(new Demo.Model.State() { Id = 1 });
            stateList.Add(new Demo.Model.State() { Id = 34 });
            stateList.Add(new Demo.Model.State() { Id = 78 });

            IDictionary<string, object> actionParams = new Dictionary<string, object>();
            actionParams.Add("sectorId", 123);
            actionParams.Add("fechaIngreso", DateTime.UtcNow);
            actionParams.Add("StateIDs", stateList);

            IDictionary<string, object> filters = new Dictionary<string, object>();
            filters.Add("Id", 2);

            string query = service.GenerateFunctionQueryUri<EIV.Demo.Model.Country>("Countries", null, filters);

            Console.WriteLine("Query Uri: " + query);

            ActionOperation<EIV.Demo.Model.Country> queryEntity = new ActionOperation<EIV.Demo.Model.Country>(service);
            queryEntity.EntitySet = "Countries";
            queryEntity.ActionName = "EIV.Demo.WebService.PaisAction";
            queryEntity.SetEntityParams(filters);
            queryEntity.SetParameters(actionParams);

            queryResponse = queryEntity.Execute();

            Console.WriteLine("Query Response: " + queryResponse.StatusCode + " " + queryResponse.Message);
            Console.WriteLine("Query Content: " + queryResponse.Content);

            //var one = queryEntity.ParseResponse<int>(queryResponse);

        }

        private static void TestBatchOperations()
        {
            // Operation 1: Query
            QueryOperation<EIV.Demo.Model.User> queryEntity = new QueryOperation<EIV.Demo.Model.User>(service);
            queryEntity.EntitySet = "Users";

            // Operation 2: Update
            EIV.Demo.Model.Country modCountry = new EIV.Demo.Model.Country() { Id = 2, Name = "Argentina (Mod)" };

            IDictionary<string, object> filters = new Dictionary<string, object>();
            filters.Add("Id", 2);

            UpdateOperation<EIV.Demo.Model.Country> updateEntity = new UpdateOperation<EIV.Demo.Model.Country>(service);
            updateEntity.EntitySet = "Countries";
            updateEntity.Entity = modCountry;
            updateEntity.SetEntityParams(filters);

            // Operation 3: Function
            FunctionOperation<EIV.Demo.Model.Country> funcEntity = new FunctionOperation<EIV.Demo.Model.Country>(service);
            funcEntity.EntitySet = "Countries";
            funcEntity.FunctionName = "EIV.Demo.WebService.TotalPaises";

            // Operation 4: Delete
            IDictionary<string, object> delFilters = new Dictionary<string, object>();
            delFilters.Add("Id", 3);

            DeleteOperation<EIV.Demo.Model.Country> delEntity = new DeleteOperation<EIV.Demo.Model.Country>(service);
            delEntity.EntitySet = "Countries";
            delEntity.SetEntityParams(delFilters);

            // Execute all operations
            IList<IODataOperation> operList = new List<IODataOperation>();
            operList.Add(delEntity);
            operList.Add(updateEntity);
            operList.Add(funcEntity);
            operList.Add(queryEntity);

            var batchResponse = service.ExecuteBatch(operList);
            foreach (IODataResponse resp in batchResponse)
            {
                Console.WriteLine("Query Uri     : " + resp.QueryUri);
                Console.WriteLine("Query Response: " + resp.StatusCode + " " + resp.Message);
                Console.WriteLine("Query Content : " + resp.Content);
                Console.WriteLine();
                Console.WriteLine();
            }

        }

    }
}