using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System;

namespace MMO_EFCore
{
    class Program
    {   
        // Annotatiton (Attribute)
        [DbFunction()]
        public static double? GetAverageReviewScore(int itemId)
        {
            throw new NotImplementedException("사용 금지!");
        }

        static void Main(string[] args)
        {
            DbCommands.InitializeDB(forceReset: false);

            Console.WriteLine("명령어를 입력하세여");
            Console.WriteLine("[0] Force Reset");
            Console.WriteLine("[1] ShowItems");
            Console.WriteLine("[2] Test");

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
                        DbCommands.ShowItems();
                        break;
                    case "2":
                        DbCommands.Test();
                        break;
                    case "3":
                        break;
                }

            }
        }
    }
}
