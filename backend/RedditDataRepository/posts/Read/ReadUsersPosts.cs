﻿using Microsoft.WindowsAzure.Storage.Table;
using RedditDataRepository.classes.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RedditDataRepository.posts.Read
{
    public class ReadUsersPosts
    {
        public static async Task<List<Post>> Execute(CloudTable table, string postId, int remaining, string searchKeywords, int sort, DateTime time, string email)
        {
            TableQuery<Post> query;
            if (postId.Equals("0") || sort != 0)
            {
                query = new TableQuery<Post>().Where(TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Post"),
                TableOperators.And,
                TableQuery.GenerateFilterCondition("Author", QueryComparisons.Equal, email)));
            }
            else
            {
                string partitionFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Post");
                string authorFilter = TableQuery.GenerateFilterCondition("Author", QueryComparisons.Equal, email);
                string postIdFilter = TableQuery.GenerateFilterCondition("PostId", QueryComparisons.LessThan, postId);
                string combinedFilter = TableQuery.CombineFilters(partitionFilter, TableOperators.And, postIdFilter);
                combinedFilter = TableQuery.CombineFilters(combinedFilter, TableOperators.And, authorFilter);
                query = new TableQuery<Post>().Where(combinedFilter);
            }

            List<Post> allPosts = new List<Post>();
            var queryResult = await table.ExecuteQuerySegmentedAsync(query, null);
            allPosts.AddRange(queryResult.Results);

            string postTitle;
            if (postId.Equals("0"))
            {
                postTitle = "~";
            }
            else
            {
                string partitionFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Post");
                string authorFilter = TableQuery.GenerateFilterCondition("Author", QueryComparisons.Equal, email);
                string postIdFilter = TableQuery.GenerateFilterCondition("PostId", QueryComparisons.Equal, postId);
                string combinedFilter = TableQuery.CombineFilters(partitionFilter, TableOperators.And, postIdFilter);
                combinedFilter = TableQuery.CombineFilters(combinedFilter, TableOperators.And, authorFilter);
                var singleQuery = new TableQuery<Post>().Where(combinedFilter);
                var currentPost = await table.ExecuteQuerySegmentedAsync(singleQuery, null);
                var result = currentPost.FirstOrDefault();
                postTitle = result.Title;
            }

            if (!searchKeywords.Contains('~'))
            {
                List<Post> posts = new List<Post>();
                string[] searchTerms = searchKeywords.ToLower().Split(' ');
                foreach (Post p in allPosts)
                {
                    foreach (string s in searchTerms)
                    {
                        string title = Regex.Replace(p.Title.ToLower(), @"\s+", "");
                        if (title.Contains(s))
                        {
                            posts.Add(p);
                        }
                    }
                }
                if (sort == 0)
                {
                    return posts.OrderByDescending(post => post.Timestamp).Take(remaining).ToList();
                }
                else if (sort == 1)
                {
                    if (postTitle.Equals("~"))
                    {
                        return posts.OrderBy(post => post.Title).Take(remaining).ToList();
                    }
                    else
                    {
                        return posts.OrderBy(post => post.Title).SkipWhile(post => !post.Title.Equals(postTitle)).Skip(1).Where(post => post.Timestamp < time).Take(remaining).ToList();
                    }
                }
                else
                {
                    if (postTitle.Equals("~"))
                    {
                        return posts.OrderByDescending(post => post.Title).Take(remaining).ToList();
                    }
                    else
                    {
                        return posts.OrderByDescending(post => post.Title).SkipWhile(post => !post.Title.Equals(postTitle)).Skip(1).Where(post => post.Timestamp < time).Take(remaining).ToList();
                    }
                }
            }

            if (sort == 0)
            {
                return allPosts.OrderByDescending(allPost => allPost.Timestamp).Take(remaining).ToList();
            }
            else if (sort == 1)
            {
                if (postTitle.Equals("~"))
                {
                    return allPosts.OrderBy(post => post.Title).Take(remaining).ToList();
                }
                else
                {
                    return allPosts.OrderBy(post => post.Title).SkipWhile(post => !post.Title.Equals(postTitle)).Skip(1).Where(post => post.Timestamp < time).Take(remaining).ToList();
                }
            }
            else
            {
                if (postTitle.Equals("~"))
                {
                    return allPosts.OrderByDescending(post => post.Title).Take(remaining).ToList();
                }
                else
                {
                    return allPosts.OrderByDescending(post => post.Title).SkipWhile(post => !post.Title.Equals(postTitle)).Skip(1).Where(post => post.Timestamp < time).Take(remaining).ToList();
                }
            }
        }
    }
}
