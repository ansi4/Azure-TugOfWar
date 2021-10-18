using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Linq;
using System;

namespace LDI_Demo
{
    public static class Vote
    {
        [FunctionName("GetVotesForTeam")]
        public static IActionResult GetVotesForTeam(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "vote/{team}")] HttpRequest req,
            string team,
            [Table("votes")] 
            CloudTable votesTable,
            ILogger log)
        {            
            var query = votesTable.CreateQuery<TableEntity>();
            query.SelectColumns = new[] { nameof(TableEntity.PartitionKey) };

            var count = query.Where(v => v.PartitionKey == team).ToList().Count;

            return new OkObjectResult(new { Count = count });
        }

        [FunctionName("VoteForTeam")]
        [return: Table("votes")]
        public static TableEntity VoteForTeam(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "vote/{team}")] HttpRequest req,
            string team,
            ILogger log)
            {
                return new TableEntity(team, Guid.NewGuid().ToString());
            }
        }
}
