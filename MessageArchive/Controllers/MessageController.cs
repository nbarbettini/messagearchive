using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using MessageArchive.Models;
using Nest;

namespace MessageArchive.Controllers
{
    public class MessageController : ApiController
    {
        public async Task<IEnumerable<Message>> Get(string q, int from, int size)
        {
            var url = ConfigurationManager.AppSettings["ES_URL"];
            var setting = new ConnectionSettings(new Uri(url));
            var client = new ElasticClient(setting);

            // Parse the search query
            string[] terms = q.Split(' ');
            var personTerm = terms.SingleOrDefault(x => x.StartsWith("person:"));
            if (!string.IsNullOrEmpty(personTerm))
            {
                terms = terms.Except(new string[] { personTerm }).ToArray();
                personTerm = personTerm.Replace("person:", string.Empty);
            }
            var textTerms = string.Join(" ", terms);

            var searchResults = await client.SearchAsync<Message>(s => s
                .AllIndices()
                .AllTypes()
                .Size(size)
                .From(from)
                .SortAscending(f => f.Date)
                .Query(qry =>
                    (qry.Term("from", personTerm) ||
                    qry.Term("to", personTerm)) &&
                    qry.Match(m => m
                        .OnField(f => f.Text)
                        .Query(textTerms)
                        .Operator(Operator.And))));

            return searchResults.Documents;
        }
    }
}
