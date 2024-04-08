﻿using Microsoft.WindowsAzure.Storage.Table;
using RedditDataRepository.classes.UserDTO;
using RedditDataRepository.users.Read;
using System.Threading.Tasks;

namespace RedditDataRepository.users.Create
{
    /// <summary>
    /// Provides functionality to insert a user entity into an Azure Table Storage.
    /// </summary>
    public class InsertUser
    {
        /// <summary>
        /// Inserts a user entity into an Azure Table Storage.
        /// </summary>
        /// <param name="table">The CloudTable object representing the Azure Table Storage.</param>
        /// <param name="user">The user entity to be inserted.</param>
        /// <returns>A boolean indicating whether the insertion was successful.</returns>
        public static async Task<bool> Add(CloudTable table, RegisteredUser user)
        {
            // Check if user or table is null
            if (user == null || table == null)
                return false;

            // Check if user already exists
            if (await IsUserExists.RunCheckAsync(table, user.Email))
                return false;

            try
            {
                // Create a TableOperation object to insert or replace the entity
                TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(user);

                // Execute the operation asynchronously
                TableResult result = await table.ExecuteAsync(insertOrReplaceOperation);

                // Check if the operation was successful
                return result.HttpStatusCode == 200 || result.HttpStatusCode == 204;
            }
            catch
            {
                // An error occurred during insertion
                return false;
            }
        }
    }
}
