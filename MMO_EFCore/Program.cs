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
            Console.WriteLine("[1] UpdateTest"); // 즉시

            while (true)
            {
                Console.WriteLine("> ");
                string command = Console.ReadLine();

                switch (command)
                {
                    case "0":
                        DbCommands.InitializeDB(true);
                        break;
                    case "1":
                        DbCommands.UpdateTest();
                        break;
                    case "2":
                        break;
                    case "3":
                        break;
                }

            }

            // CRUD (create - read - update - delete)
        }
    }
}
