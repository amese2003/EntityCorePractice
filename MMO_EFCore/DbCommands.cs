using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;

namespace MMO_EFCore
{
    public class DbCommands
    {
        public static void InitializeDB(bool forceReset = false)
        {
            using (AppDbContext db = new AppDbContext())
            {
                if (!forceReset && (db.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists())
                    return;

                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                string command =
                    @" CREATE FUNCTION GetAverageReviewScore (@itemId INT) RETURNS FLOAT
                       AS
                       BEGIN
                       DECLARE @result AS FLOAT
                       SELECT @result = AVG(CAST([Score] AS FLOAT))
                       FROM ItemReview AS r
                       WHERE @itemId = r.itemId

                       RETURN @result

                       END";

                db.Database.ExecuteSqlRaw(command);

                CreateTestData(db);
                Console.WriteLine("DB Initialized");
            }
        }

        public static void CreateTestData(AppDbContext db)
        {
            var Nero = new Player() { Name = "Nero" };
            var faker = new Player() { Name = "Faker" };
            var deft = new Player() { Name = "Deft" };

            //Console.WriteLine(db.Entry(Nero).State);
            EntityState state = db.Entry(Nero).State;

            List<Item> items = new List<Item>()
            {
                new Item()
                {
                    TemplateId = 101,
                    Owner = Nero
                },
                
            };

            Guild guild = new Guild()
            {
                GuildName = "T1",
                Memebers = new List<Player>() { Nero, faker, deft }
            };

            db.Items.AddRange(items);
            db.Guilds.Add(guild);


            db.SaveChanges();
        }

        public static void ShowItems()
        {
            using (AppDbContext db = new AppDbContext())
            {
                foreach (var item in db.Items.Include(i => i.Owner).IgnoreQueryFilters().ToList())
                {
                    if (item.SoftDeleted)
                    {
                        if (item.Owner == null)
                            Console.WriteLine($"DELETED - itemId({item.ItemId}) TemplateId({item.TemplateId}) Owner(0)");
                        else
                            Console.WriteLine($"DELETED - itemId({item.ItemId}) TemplateId({item.TemplateId}) OwnerID({item.Owner.PlayerId}) Owner({item.Owner.Name})");
                    }
                    else
                    {
                        if (item.Owner == null)
                            Console.WriteLine($"itemId({item.ItemId}) TemplateId({item.TemplateId}) Owner(0)");
                        else
                            Console.WriteLine($"itemId({item.ItemId}) TemplateId({item.TemplateId}) OwnerID({item.Owner.PlayerId}) Owner({item.Owner.Name})");
                    }
                }
            }
        }

        public static void ShowGuild()
        {
            using (AppDbContext db = new AppDbContext())
            {
                foreach (var guild in db.Guilds.Include(g=>g.Memebers).ToList())
                {
                    Console.WriteLine($"GuildId ({guild.GuildId}) GuildName ({guild.GuildName}) MemberCount({guild.Memebers.Count})");
                }
            }
        }

        public static void Test()
        {
            using (AppDbContext db = new AppDbContext())
            {
                // FromSql
                {
                    string name = "Nero";
                    string name2 = "Anything OR [=]";
                    // SQL Injection (Web Hacking)

                    var list = db.Players
                        .FromSqlRaw("SELECT * FROM dbo.Player WHERE Name = {0}", name)
                        .Include(p => p.OwnedItem)
                        .ToList();

                    foreach(var p in list)
                    {
                        Console.WriteLine($"{p.Name} {p.PlayerId}");
                    }

                    // String Interpolation c# 6.0
                    var list2 = db.Players
                        .FromSqlInterpolated($"SELECT * FROM dbo.Player WHERE Name = {name}")
                        .ToList();

                    foreach (var p in list2)
                    {
                        Console.WriteLine($"{p.Name} {p.PlayerId}");
                    }
                }

                //  ExecuteSqlCommand (Non-Query SQL)
                {
                    Player p = db.Players.Single(p => p.Name == "Faker");

                    string prevName = "Faker";
                    string after = "Faker_New";
                    db.Database.ExecuteSqlInterpolated($"UPDATE dbo.Player SET Name={after} WHERE Name = {prevName}");

                    db.Entry(p).Reload();
                }
            }
        }
    }
}
