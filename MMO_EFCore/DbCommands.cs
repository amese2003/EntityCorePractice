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

        public static void ShowGuilds()
        {
            using (AppDbContext db = new AppDbContext())
            {
                foreach(var guild in db.Guilds.MapGuildToDto().ToList())
                {
                    Console.WriteLine($"GuildId({guild.GuildId})  GuildName({guild.Name}) MemberCount({guild.MemberCount})");
                }
            }
        }

        // 장점 : 최소 정보로 Update
        // 단점 : Read가 두번!

        public static void UpdateByReload()
        {
            ShowGuilds();

            // 외부에서 수정 원하는 데이터의 ID / 정보 넘겨줬다 가정
            Console.WriteLine("Input GuildId");
            Console.Write(" > ");
            int id = int.Parse(Console.ReadLine());
            Console.WriteLine("Input GuildName");
            Console.Write(" > ");
            string name = Console.ReadLine();

            using (AppDbContext db = new AppDbContext())
            {
                Guild guild = db.Find<Guild>(id);

                guild.GuildName = name;

                db.SaveChanges();
            }

            Console.WriteLine("업데이트!");
            ShowGuilds();
        }

        public static string MakeUpdateJsonStr()
        {
            var jsonStr = "{\"GuildId\":1, \"GuildName\":\"Hello\", \"Members\":null}";
            return jsonStr;
        }

        // 장점 : DB에 다시 Read를 할 필요 없이 바로 Update
        // 단점 : 모든 정보를 끌어와야됨. 보안 문제도 있음.

        public static void UpdateByFull()
        {
            ShowGuilds();

            string jsonStr = MakeUpdateJsonStr();
            Guild guild = JsonConvert.DeserializeObject<Guild>(jsonStr);

            using (AppDbContext db = new AppDbContext())
            {
                db.Guilds.Update(guild);
                db.SaveChanges();
            }

            Console.WriteLine("업데이트!");
            ShowGuilds();
        }
    }
}
