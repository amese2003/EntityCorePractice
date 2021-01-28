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
            var player = new Player()
            {
                Name = "Nero"
            };

            List<Item> items = new List<Item>()
            {
                new Item()
                {
                    TemplateId = 101,
                    CreateDate = DateTime.Now,
                    Owner = player
                },
                new Item()
                {
                    TemplateId = 102,
                    CreateDate = DateTime.Now,
                    Owner = player
                },
                new Item()
                {
                    TemplateId = 103,
                    CreateDate = DateTime.Now,
                    Owner = new Player() { Name = "Faker"}
                }
            };

            db.items.AddRange(items);
            db.SaveChanges();
        }

        public static void ReadAll()
        {
            using (var db = new AppDbContext())
            {
                // AsNoTracking : ReadOnly << Tracking snapshot이라고 데이터 변경을 탐지하는 기능 때문
                // include : Eager Loading (즉시 로딩)
                foreach (Item item in db.items.AsNoTracking().Include(i => i.Owner))
                {
                    Console.WriteLine($"TemplateId({item.TemplateId}) Owner({item.Owner.Name}) Created({item.CreateDate})");
                }
            }
        }

        // 특정 플레이어가 소지한 아이템의 CreateDate를 수정
        public static void UpdateDate()
        {
            Console.WriteLine("Input Player Name");
            Console.WriteLine(">");
            string name = Console.ReadLine();

            using (var db = new AppDbContext())
            {
                var items = db.items.Include(i => i.Owner).Where(i => i.Owner.Name == name);

                foreach(Item item in items)
                {
                    item.CreateDate = DateTime.Now;
                }

                db.SaveChanges();
            }

            ReadAll();
        }

        public static void DeleteItem()
        {
            Console.WriteLine("Input Player Name");
            Console.WriteLine(">");
            string name = Console.ReadLine();

            using (var db = new AppDbContext())
            {
                var items = db.items.Include(i => i.Owner).Where(i => i.Owner.Name == name);
                db.items.RemoveRange(items);
                db.SaveChanges();
            }

            ReadAll();
        }
    }
}
