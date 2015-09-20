using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Http;
using MessageArchive.Models;
using Nest;

namespace MessageArchive.Controllers
{
    public class MessageController : ApiController
    {
        public async Task<IEnumerable<Message>> Get(string q)
        {
            var url = ConfigurationManager.AppSettings["ES_URL"];
            var setting = new ConnectionSettings(new Uri(url));
            var client = new ElasticClient(setting);

            var searchResults = await client.SearchAsync<Message>(s => s
                .AllIndices()
                .AllTypes()
                .Size(50)
                .SortAscending(x => x.Date)
                .Query(query =>
                    query.MultiMatch(mm => 
                        mm.OnFields(
                            f => f.From, 
                            f => f.To, 
                            f => f.Text)
                        .Query(q))));

            return searchResults.Documents;
        }
    }
}
