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
            db.SaveChanges();
        }

        public static void ShowItems()
        {
            using (AppDbContext db = new AppDbContext())
            {
                foreach (var item in db.items.Include(i => i.Owner).ToList())
                {
                    if(item.Owner == null)
                        Console.WriteLine($"itemId({item.ItemId}) TemplateId({item.TemplateId}) Owner(0)");
                    else
                        Console.WriteLine($"itemId({item.ItemId}) TemplateId({item.TemplateId}) OwnerID({item.Owner.PlayerId}) Owner({item.Owner.Name})"   );
                }
            }
        }

        // 1) PK가 Nullable이 아니라면?
        // - Player가 지워지면 FK로 해당 Player 참조하는 item도 같이 삭제됨
        // 2) PK가 Nullable이라면?

        public static void Test()
        {
            ShowItems();

            Console.WriteLine("Input delete PlayerId");
            Console.Write(" > ");
            int id = int.Parse(Console.ReadLine());

            using (AppDbContext db = new AppDbContext())
            {
                 Player player = db.players
                    .Include(p => p.items)
                    .Single(p => p.PlayerId == id);

                db.players.Remove(player);
                db.SaveChanges();
            }

            Console.WriteLine("삭제 끝");
            ShowItems();
        }

    }

}
