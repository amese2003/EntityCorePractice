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

        public static void TestUpdateAttach()
        {
            using (AppDbContext db = new AppDbContext())
            {
                // State 조작
                {
                    Player p = new Player() { Name = "StateTest" };
                    db.Entry(p).State = EntityState.Added; // Tracked로 변환
                    db.SaveChanges();
                }

                // TrackGraph
                {
                    // Disconnected 상태에서,
                    // 모두 갱신하는게 아니라 플레이어 이름'만' 갱신하고 싶다면?

                    Player p = new Player()
                    {
                        PlayerId = 2,
                        Name = "Faker_New"
                    };

                    p.OwnedItem = new Item() { TemplateId = 777 }; // 아이템 정보 가정
                    p.Guild = new Guild() { GuildName = "TrackGraphGuild" }; // 길드 정보 가정

                    db.ChangeTracker.TrackGraph(p, e =>
                    {
                        if (e.Entry.Entity is Player)
                        {
                            e.Entry.State = EntityState.Unchanged;
                            e.Entry.Property("Name").IsModified = true;
                        }
                        else if (e.Entry.Entity is Guid)
                        {
                            e.Entry.State = EntityState.Unchanged;
                        }
                        else if (e.Entry.Entity is Item)
                        {
                            e.Entry.State = EntityState.Unchanged;
                        }
                    });

                    db.SaveChanges();
                }
            }           
        }
    }
}
