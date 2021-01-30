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
            Console.WriteLine("[1] Eager Loading"); // 즉시
            Console.WriteLine("[2] Explict Loading"); // 명시적
            Console.WriteLine("[3] Select Loading"); // Select

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
                        DbCommands.EagerLoading();
                        break;
                    case "2":
                        DbCommands.ExplictLoading();
                        break;
                    case "3":
                        DbCommands.SelectLoading();
                        break;
                }

            }

            // CRUD (create - read - update - delete)
        }
    }
}
