using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;


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

                CreateTestData(db);
                Console.WriteLine("DB Initialized");
            }
        }

        public static void CreateTestData(AppDbContext db)
        {
            var Nero = new Player() { Name = "Nero" };
            var faker = new Player() { Name = "Faker" };
            var deft = new Player() { Name = "Deft" };

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
                    Owner = faker
                },
                new Item()
                {
                    TemplateId = 103,
                    CreateDate = DateTime.Now,
                    Owner = deft
                }
            };

            Guild guild = new Guild()
            {
                GuildName = "T1",
                Memebers = new List<Player>() { Nero, faker, deft }
            };

            db.items.AddRange(items);
            db.Guilds.Add(guild);
            db.SaveChanges();
        }


        // 1 + 2 } 특정 길드에 있는 길드원들이 소지한 모든 아이템을 보고싶다.
        // 장점 : DB 접근 한번으로 다 로딩 (JOIN)
        // 단점 : 다 필요한지 모르겠는데 다 가져옴
        public static void EagerLoading()
        {
            Console.WriteLine("길드 이름을 입력하세요");
            Console.Write(">");
            string name = Console.ReadLine(); 

            using (var db = new AppDbContext())
            {
                Guild guild = db.Guilds.AsNoTracking()
                    .Where(g => g.GuildName == name)
                    .Include(g => g.Memebers)
                        .ThenInclude(p => p.items)
                        .First();

                foreach(Player player in guild.Memebers)
                {
                    Console.WriteLine($"TemplateId({player.items.TemplateId}) Owner({player.Name})");
                }

            }
        }
        
        // 장점 : 필요한 시점에 필요한 데이터만 로딩 가능
        // 단점 : DB 접근 비용이 너무 쌤
        public static void ExplictLoading()
        {
            Console.WriteLine("길드 이름을 입력하세요");
            Console.Write(">");
            string name = Console.ReadLine();

            using (var db = new AppDbContext())
            {
                Guild guild = db.Guilds
                    .Where(g=>g.GuildName == name)
                    .First();

                // 명시적
                db.Entry(guild).Collection(g => g.Memebers).Load();

                foreach(Player player in guild.Memebers)
                {
                    db.Entry(player).Reference(p => p.items).Load();
                }


                foreach (Player player in guild.Memebers)
                {
                    Console.WriteLine($"TemplateId({player.items.TemplateId}) Owner({player.Name})");
                }

            }
        }


        // 3) 특정 길드에 있는 길드원의 수는?


        // SELECT COUNT(*)
        // 장점 : 필요한 정보만 쏘옥- 빼올수 있음.
        // 단점 : 일일히 Select 안에 만들어 줘야함

        public static void SelectLoading()
        {
            Console.WriteLine("길드 이름을 입력하세요");
            Console.Write(">");
            string name = Console.ReadLine();

            using (var db = new AppDbContext())
            {
               var info = db.Guilds
                    .Where(g => g.GuildName == name)
                    .Select(g=>new {
                        Name = g.GuildName,
                        MemberCount = g.Memebers.Count
                    })
                    .First();

                Console.WriteLine($"GuildName({info.Name}), MemberCount({info.MemberCount})");
            }
        }

    }
}
