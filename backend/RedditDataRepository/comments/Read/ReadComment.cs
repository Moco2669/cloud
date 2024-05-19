﻿using Microsoft.WindowsAzure.Storage.Table;
using RedditDataRepository.classes.Comments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditDataRepository.comments.Read
{
    public class ReadComment
    {
        public static async Task<Comment> Run(CloudTable table, string commentId)
        {
            var tableQuery = new TableQuery<Comment>().Where(TableQuery.CombineFilters(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Comment"), TableOperators.And, TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, commentId)));
            var result = table.ExecuteQuery(tableQuery);
            if (result != null)
            {
                Comment comment = result.Count() == 0 ? null : result.First();
                return await Task.FromResult(comment);
            }
            return null;
        }
    }
}
