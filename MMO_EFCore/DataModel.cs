using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace MMO_EFCore
{
    // User Defined Function
    // 우리가 직접 만든 SQL을 호출하게 하는 기능
    // - 연산을 DB쪽에서 하도록 떠넘기고 싶다.
    // - EF Core 쿼리가 약간 비효율적이다?

    // Steps
    // 1) Configuration
    // - static 함수를 만들고 EF Core 들고
    // 2) Database Setup
    // 3) 사용

    public class ItemReview
    {
        public int ItemReviewId { get; set; }
        public int Score { get; set; } // 0-5점
    }

    [Table("Item")]
    public class Item
    {
        public bool SoftDeleted { get; set; }

        // 이름 ->  Primary Key
        public int ItemId { get; set; }
        public int TemplateId { get; set; } // 101 = 집행검 (...)
        public DateTime CreateDate { get; set; }

        // 다른 클래스 참조 -> FK (Navigational Property)
        public int OwnerId { get; set; }        
        public Player Owner { get; set; }

        public ICollection<ItemReview> Reviews { get; set; }

    }

    public class Knight
    {
        private int _hp;

        public void SetHP(int hp)
        {
            _hp = hp;
        }
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
