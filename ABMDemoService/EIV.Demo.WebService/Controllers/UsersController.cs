
namespace EIV.Demo.WebService.Controllers
{
    using Data.Interface;
    using Data.Repository;
    using Filters;
    using Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Web.Http;
    using System.Web.OData;
    using System.Web.OData.Extensions;
    using System.Web.OData.Routing;

    [EnableQuery]
    public class UsersController : ODataController
    {
        private static IUserRepository userRepository = null;

        public UsersController()
        {
            if (userRepository == null)
            {
                userRepository = new UserRepository();
            }
        }

        [HttpGet]
        public IHttpActionResult Get()
        // HttpResponseMessage
        //public HttpResponseMessage Get()
        //public IList<User> Get()
        {
            var rst = userRepository.GetAll();

            // JsonMediaTypeFormatter.DefaultMediaType
            // var response = Request.CreateResponse(HttpStatusCode.OK, rst);
            var response = Request.CreateResponse(HttpStatusCode.OK, rst);

            return Ok(rst);

            //return rst;
            //return response;
            //return ResponseMessage(response);

            //return clientRepository.GetAll();
        }

        [HttpGet]
        [EnableQuery(MaxExpansionDepth = 3)]
        public User Get([FromODataUri]int key)
        {
            User user = userRepository.GetById(key);
            if (user == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            else
                return user;
        }

        [EnableQuery(MaxExpansionDepth = 3)]
        public IHttpActionResult GetMenuItems(int key)
        {
            //var payinPIs = _accounts.Single(a => a.AccountID == key).PayinPIs;
            var menuItems = userRepository.GetById(key).MenuItems;
            return Ok(menuItems);
        }

        [HttpPost]
        public IHttpActionResult Post(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // For this sample, we aren't enforcing unique keys.
            userRepository.Add(user);
            return Created(user);
            //var newUser = user;

            //return Ok();
        }

        [HttpPost]
        public IHttpActionResult AuthenticateUser(ODataActionParameters parameters)
        {
            HttpResponseMessage response = null;

            object param1 = null;
            object param2 = null;

            string userName = default(string);
            string userPwd = default(string);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (parameters == null)
            {
                response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid parameters", new ArgumentNullException("username, password"));

                return ResponseMessage(response);
            }

            if (parameters.Count != 2)
            {
                response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid parameters", new ArgumentNullException("username, password"));

                return ResponseMessage(response);
            }
            if (!parameters.TryGetValue("username", out param1))
            {
                response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid username!");

                return ResponseMessage(response);
            }

            if (!parameters.TryGetValue("password", out param2))
            {
                response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid password!");

                return ResponseMessage(response);
            }

            userName = (string) param1;
            userPwd = (string) param2;
            if (string.IsNullOrEmpty(userPwd))
            {
                response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid password!");

                return ResponseMessage(response);
            }

            EIV.Demo.Model.User rst = userRepository.Authenticate(userName, userPwd);
            if (rst == null)
            {
                response = Request.CreateResponse(HttpStatusCode.Forbidden);
            } else
            {
                response = Request.CreateResponse(HttpStatusCode.OK, rst);
            }

            var path = new DefaultODataPathHandler().Parse("http://localhost:1860/odata", "Users", Request.GetRequestContainer());

            Request.ODataProperties().Path = path;

            return ResponseMessage(response);
        }

        [HttpPost]
        [EnableQuery(MaxExpansionDepth = 3)]  // Can I use this on a POST method ?
        //public IHttpActionResult SetMenu([FromODataUri] int key, ODataActionParameters parameters)
        public IHttpActionResult SetMenu(ODataActionParameters parameters)
        {
            HttpResponseMessage response = null;

            object param1 = null;
            object param2 = null;

            string userName = default(string);
            string menuItems = default(string);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (parameters == null)
            {
                response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid parameters", new ArgumentNullException("username, menuitems"));

                return ResponseMessage(response);
            }

            if (parameters.Count != 2)
            {
                response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid parameters", new ArgumentNullException("username, password"));

                return ResponseMessage(response);
            }
            if (!parameters.TryGetValue("username", out param1))
            {
                response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid username!");

                return ResponseMessage(response);
            }

            if (!parameters.TryGetValue("menuitems", out param2))
            {
                response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid menuitems!");

                return ResponseMessage(response);
            }

            userName = (string) param1;
            menuItems = (string) param2;

            var foos = menuItems.JSonToCollection<MenuItem>();

            //menuItems = parameters["menuitems"] as IList<MenuItem>;

            if (string.IsNullOrEmpty(userName))
            {
                response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid username!");

                return ResponseMessage(response);
            }

            User thisUser = userRepository.GetByName(userName);
            if (thisUser == null)
            {
                response = Request.CreateErrorResponse(HttpStatusCode.NotFound, "Invalid username!");

                return ResponseMessage(response);
            }

            // Is this correct?
            thisUser.MenuItems = foos;
            
            response = Request.CreateResponse<bool>(HttpStatusCode.OK, true);

            return ResponseMessage(response);
        }

    }
}