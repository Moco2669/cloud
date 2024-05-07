using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminToolsConsoleApp
{
    public class Executor
    {
        public void AddEmail(IAdminToolService adminToolService)
        {
            Console.WriteLine();
            Console.WriteLine("Enter ID: ");
            string id = Console.ReadLine();
            Console.WriteLine("Enter Email address: ");
            string email = Console.ReadLine();

            AlertEmailDTO alertEmail = new AlertEmailDTO(id, email);

            Console.WriteLine();
            if (adminToolService.AddEmail(alertEmail))
            {
                Console.WriteLine("Email added successfully.");
            }
            else
            {
                Console.WriteLine("Error adding email!");
            }
        }

        public void ListAllEmails(IAdminToolService adminToolService)
        {
            List<AlertEmailDTO> emails = adminToolService.ReadAllEmails();
            Console.WriteLine("\n========================== ALL EMAILS =========================== ");
            foreach(var email in emails)
            {
                Console.WriteLine($"ID: {email.Id}, Email: {email.Email}");
            }
            Console.WriteLine("===================================================================");
        }

        public void RemoveEmail(IAdminToolService adminToolService)
        {
            Console.WriteLine("\nEnter email ID: ");
            string id = Console.ReadLine();

            Console.WriteLine();
            if (adminToolService.RemoveEmail(id))
            {
                Console.WriteLine("Email remove successfully.");
            }
            else
            {
                Console.WriteLine("Email with entered ID does not exist!");
            }
        }
    }
}
