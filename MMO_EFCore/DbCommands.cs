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
                    CreateDate = DateTime.Now,
                    Owner = Nero
                },
                new Item()
                {
                    TemplateId = 102,
                    CreateDate = DateTime.Now,
                    Owner = faker,
                },
                new Item()
                {
                    TemplateId = 103,
                    CreateDate = DateTime.Now,
                    Owner = deft
                }
            };

            items[0].Reviews = new List<ItemReview>()
            {
                new ItemReview() {Score = 5},
                new ItemReview() {Score = 3},
                new ItemReview() {Score = 2},
            };

            items[1].Reviews = new List<ItemReview>()
            {
                new ItemReview() {Score = 1},
                new ItemReview() {Score = 1},
                new ItemReview() {Score = 0},
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

        public static void CalcAverage()
        {
            using (AppDbContext db = new AppDbContext())
            {
                foreach(var average in db.Items.Select(i => Program.GetAverageReviewScore(i.ItemId)))
                {
                    if (average == null)
                        Console.WriteLine("No Review!");
                    else
                        Console.WriteLine($"Average : {average.Value}");
                }
            }
        }

    }
}
