using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MessageArchive.Controllers
{
    public class MessageController : ApiController
    {
        public IEnumerable<string> Get(string q)
        {
            return new string[] { "value1", "value2" };
        }
    }
}
