using Microsoft.WindowsAzure.Storage.Table;
using RedditDataRepository.classes.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RedditDataRepository.posts.Read
{
    /// <summary>
    /// Provides functionality to retrieve and filter posts from a cloud table for a single user.
    /// </summary>
    public class ReadUsersPosts
    {
        /// <summary>
        /// Executes a query to retrieve and filter posts from a cloud table.
        /// </summary>
        /// <param name="table">The cloud table containing the posts.</param>
        /// <param name="postId">The ID of the last loaded post.</param>
        /// <param name="remaining">The number of posts to return.</param>
        /// <param name="searchKeywords">Keywords to filter posts by.</param>
        /// <param name="sort">The sorting criterion for the posts.</param>
        /// <param name="time">A timestamp to filter posts by.</param>
        /// <param name="email">An email of a user whose posts to filter.</param>
        /// <returns>A list of posts matching the specified criteria.</returns>
        public static async Task<List<Post>> Execute(CloudTable table, string postId, int remaining, string searchKeywords, int sort, DateTimeOffset time, string email)
        {
            // Retrieve all posts from the cloud table for a specified user
            var query = new TableQuery<Post>().Where(TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Post"),
                TableOperators.And,
                TableQuery.GenerateFilterCondition("Author", QueryComparisons.Equal, email)));

            List<Post> allPosts = new List<Post>();
            var queryResult = await table.ExecuteQuerySegmentedAsync(query, null);
            allPosts.AddRange(queryResult.Results);

            // Retrieve details of the latest post if postId is provided
            string postTitle;
            DateTimeOffset timestamp;
            if (postId.Equals("0"))
            {
                postTitle = "~"; // Placeholder value for non-existent post title
                timestamp = DateTime.Now;
            }
            else
            {
                string partitionFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Post");
                string authorFilter = TableQuery.GenerateFilterCondition("Author", QueryComparisons.Equal, email);
                string postIdFilter = TableQuery.GenerateFilterCondition("Id", QueryComparisons.Equal, postId);
                string combinedFilter = TableQuery.CombineFilters(partitionFilter, TableOperators.And, postIdFilter);
                combinedFilter = TableQuery.CombineFilters(combinedFilter, TableOperators.And, authorFilter);
                var singleQuery = new TableQuery<Post>().Where(combinedFilter);
                var currentPost = await table.ExecuteQuerySegmentedAsync(singleQuery, null);
                var result = currentPost.FirstOrDefault();
                postTitle = result.Title;
                timestamp = result.Timestamp;
            }

            // Filter posts based on search keywords
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

                // Sort and return filtered posts based on specified criteria
                if (sort == 0)
                {
                    if (postTitle.Equals("~"))
                    {
                        return posts.OrderByDescending(post => post.Timestamp).Take(remaining).ToList();
                    }
                    else
                    {
                        return posts.OrderByDescending(post => post.Timestamp).SkipWhile(post => !post.Timestamp.Equals(timestamp)).Skip(1).Take(remaining).ToList();
                    }
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

            // Return posts without filtering with searchKeywords
            if (sort == 0)
            {
                if (postTitle.Equals("~"))
                {
                    return allPosts.OrderByDescending(post => post.Timestamp).Take(remaining).ToList();
                }
                else
                {
                    return allPosts.OrderByDescending(post => post.Timestamp).SkipWhile(post => !post.Timestamp.Equals(timestamp)).Skip(1).Take(remaining).ToList();
                }
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
