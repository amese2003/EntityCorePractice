using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MMO_EFCore
{
    // Entity <-> DB Table 연동하는 다양한 방법
    // Entity Class 하나를 통으로 Read/Write -> 부담 (select Loading, DTO)
    

    // 1) Owned Type
    // - 일반 Class를 Entity에 추가하는 개념
    // a) 동일한 테이블 추가
    // - .OwnsOne()
    // - Relationship이 아닌 Ownership의 개념이기 때문에, .Include()
    // b) 다른 테이블에 추가
    // - .OwnsOne().ToTable()

    // 2) Table Per Hierarchy (TPM)
    // - 상속 관계의 여러 class <-> 하나의 테이블에 매핑
    // ex) Dog, Cat, Bird : Animal
    // a) Convention
    // - 일단 class 상속받아 만들고, DbSet 추가
    // - Discriminator?
    // b) Fluent Api
    // - .HasDiscriminator().HasValue()

    // 3) Table Splitting
    // - 다수의 Entity Class <-> 하나의 테이블에 매핑

    public class ItemOption
    {
        public int Str { get; set; }
        public int Dex { get; set; }
        public int Hp { get; set; }
    }

    public class ItemDetail
    {
        public int ItemDetailId { get; set; }
        public string Description { get; set; }
    }

    public enum ItemType
    {
        NormalItem,
        EventItem
    }


    [Table("Item")]
    public class Item
    {
        public ItemType Type { get; set; }
        public bool SoftDeleted { get; set; }
        public ItemOption Option { get; set; }
        public ItemDetail Detail { get; set; }

        // 이름 ->  Primary Key
        public int ItemId { get; set; }
        public int TemplateId { get; set; } // 101 = 집행검 (...)
        public DateTime CreateDate { get; set; }

        // 다른 클래스 참조 -> FK (Navigational Property)
        public int OwnerId { get; set; }        
        public Player Owner { get; set; }
    }
    public class EventItem : Item
    {
        public DateTime DestroyDate { get; set; }
    }

    // 클래스 이름 = 테이블 = Player
    [Table("Player")]
    public class Player
    {
        // 이름 id -> pk
        public int PlayerId { get; set; }
        [Required]
        [MaxLength(20)]
        public string Name { get; set; }

        public Item OwnedItem { get; set; } // 1 : 다
        public Guild Guild; 

    }

    [Table("Guild")]
    public class Guild
    {
        public int GuildId { get; set; }
        public string GuildName { get; set; }
        public ICollection<Player> Memebers { get; set; }
    }

    // DTO (Data Transfer Object)
    public class GuildDto
    {
        public int GuildId { get; set; }
        public string Name { get; set; }
        public int MemberCount { get; set; }
    }
}
