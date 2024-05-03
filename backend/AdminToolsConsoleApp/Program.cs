using Common;
using RedditDataRepository.tables;
using RedditDataRepository.tables.entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminToolsConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceConnector<IAdminToolService> serviceConnector = new ServiceConnector<IAdminToolService>();
            serviceConnector.Connect("net.tcp://localhost:10100/AdminToolService");
            IAdminToolService adminToolService = serviceConnector.GetProxy();

            Executor executor = new Executor();

            Console.WriteLine("Enter password to proceed:");
            string password = Console.ReadLine();

            if (password != "8279531640")
            {
                Console.WriteLine("Incorrect password. Exiting...");
                Console.ReadKey();
                return;
            }
            else
            {
                Console.WriteLine("Welcome Admin!");
            }

            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("Choose an option:");
                Console.WriteLine("1. List all emails");
                Console.WriteLine("2. Add a new email");
                Console.WriteLine("3. Delete an existing email");
                Console.WriteLine("4. Exit");

                string option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        executor.ListAllEmails(adminToolService);
                        break;
                    case "2":
                        executor.AddEmail(adminToolService);
                        break;
                    case "3":
                        executor.RemoveEmail(adminToolService);
                        break;
                    case "4":
                        exit = true;
                        Console.WriteLine("Exiting...");
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }
    }
}
