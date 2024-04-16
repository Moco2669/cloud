﻿using Microsoft.WindowsAzure.Storage.Table;
using RedditDataRepository.classes.Comments;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedditDataRepository.comments.Read
{
    public class CountComments
    {
        public static async Task<List<Comment>> Execute(CloudTable table, string postId)
        {
            var query = new TableQuery<Comment>().Where(TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Comment"),
                TableOperators.And,
                TableQuery.GenerateFilterCondition("PostId", QueryComparisons.Equal, postId)));

            List<Comment> comments = new List<Comment>();
            var queryResult = await table.ExecuteQuerySegmentedAsync(query, null);
            comments.AddRange(queryResult.Results);


            return comments;
        }
    }
}
