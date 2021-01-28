using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System;

namespace MMO_EFCore
{
    class Program
    {       
        static void Main(string[] args)
        {
            DbCommands.InitializeDB(forceReset: false);

            Console.WriteLine("명령어를 입력하세여");
            Console.WriteLine("[0] Force Reset");
            Console.WriteLine("[1] Read All");
            Console.WriteLine("[2] UpdateDate");
            Console.WriteLine("[3] Delete");

            while (true)
            {
                Console.WriteLine("> ");
                string command = Console.ReadLine();

                switch (command)
                {
                    case "0":
                        DbCommands.InitializeDB(forceReset: true);
                        break;
                    case "1":
                        DbCommands.ReadAll();
                        break;
                    case "2":
                        DbCommands.UpdateDate();
                        break;
                    case "3":
                        DbCommands.DeleteItem();
                        break;
                }

            }

            // CRUD (create - read - update - delete)
        }
    }
}
