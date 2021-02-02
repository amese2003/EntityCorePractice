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
            Console.WriteLine("[1] Update_1v1");
            Console.WriteLine("[2] Update_1vN");

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
                        DbCommands.Update_1v1();
                        break;
                    case "2":
                        DbCommands.Update_1vN();
                        break;
                    case "3":
                        break;
                }

            }

            // CRUD (create - read - update - delete)
        }
    }
}
