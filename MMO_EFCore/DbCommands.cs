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

            Console.WriteLine("1번)" + db.Entry(Nero).State);            

            db.SaveChanges();


            // Add test
            {
                Item item = new Item()
                {
                    TemplateId = 500,
                    Owner = Nero
                };

                db.Items.Add(item);
                // 아이템 추가 -> 간접적으로 Player 영향
                // Player는 Tracking 상태, FK? 필요 없음
                Console.WriteLine("2번)" + db.Entry(Nero).State);
            }

            // Delete tEST
            {
                Player p = db.Players.First();

                // DB는 이 새로운 길드의 존재도 모름 (DB 키 없음 0);
                p.Guild = new Guild() { GuildName = "곧삭제될길드" };
                // 위에서 아이템이 이미 DB에 들어간 상태 (DB 키 있음)
                p.OwnedItem = items[0];

                db.Players.Remove(p);

                // Player를 직접적으로 삭제하니까...
                Console.WriteLine("3번)" + db.Entry(p).State); // Deleted
                Console.WriteLine("4번)" + db.Entry(p.Guild).State); // Added
                Console.WriteLine("5번)" + db.Entry(p.OwnedItem).State); //
            }

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
                // Update Test
                {
                    // Disconnected
                    Player p = new Player();
                    p.PlayerId = 2;
                    p.Name = "FakerSenpai";

                    // Attach 아직 DB는 이 새로운 길드의 존재도 모름 (DB키 없음)
                    p.Guild = new Guild() { GuildName = "Update Guild" };

                    Console.WriteLine("6번)" + db.Entry(p.Guild).State); // Detached
                    db.Players.Update(p);
                    Console.WriteLine("7번)" + db.Entry(p.Guild).State); // Added
                }


                // Attach Test
                {
                    Player p = new Player();

                    // Temp
                    p.PlayerId = 3;
                    
                    p.Guild = new Guild() { GuildName = "Attach Guild" };


                    Console.WriteLine("8번)" + db.Entry(p.Guild).State); // Detached
                    db.Players.Attach(p);
                    p.Name = "Deft-_-";
                    Console.WriteLine("9번)" + db.Entry(p.Guild).State); // Added
                }

                db.SaveChanges();
            }

           
        }

    }
}
