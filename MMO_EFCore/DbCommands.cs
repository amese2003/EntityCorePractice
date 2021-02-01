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
        }

        // Update 3단께
        // 1) Tracked Entity를 얻어온다
        // 2) Entity 클래스의 property를 변경 (set)
        // 3) SaveChanges를 호출!

        // Update를 할때 전체 수정을 할까? 수정 사항이 있는 애들만 할까?
        

        // 1) SaveChanges 호출 할 때 -> 내부적으로 DetectChanges라는 함수 호출
        // 2) DetectChanges에서 -> 최초 SnapShot / 현재 SnapShot 비교


        public static void UpdateTest()
        {
            using (AppDbContext db = new AppDbContext())
            {
                var guild = db.Guilds.Single(g => g.GuildName == "T1");

                guild.GuildName = "DWG";

                db.SaveChanges();
            }
        }
    }
}
