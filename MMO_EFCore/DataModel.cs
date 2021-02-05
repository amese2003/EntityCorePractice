using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace MMO_EFCore
{
    // Default Value

    // 기본값 설정하는 방법이 여러가지 있다
    // 1) Entity Class 자체의 초기값?
    // 2) DB Table 차원에서 초기값?
    // - 결과는 같은거 아닐까?
    // - EF <-> DB외 다른 경로로 db 사용하면 차이가 날 수 있다.
    // ex) SQL Script

    // 1) Auto-Property Initializer (c# 6.0)
    // - Entity 차원의 초기값 -> SaveChanges로 DB 적용
    // 2) Fluent API
    // - DB Table Default를 적용
    // - DateTime Now
    // 3) SQL Fragment (새로운 값이 추가되는 시점에 DB쪽서 실행)
    // - .HasDefaultValueSql
    // 4) Value Generator (EF Core에서 실행됨)
    // - 일종의 Generator 규칙

    [Table("Item")]
    public class Item
    {
        public bool SoftDeleted { get; set; }

        // 이름 ->  Primary Key
        public int ItemId { get; set; }
        public int TemplateId { get; set; } // 101 = 집행검 (...)
        public DateTime CreateDate { get; private set; }

        // 다른 클래스 참조 -> FK (Navigational Property)
        public int OwnerId { get; set; }        
        public Player Owner { get; set; }

    }

    public class PlayerNameGenerator : ValueGenerator<String>
    {
        public override bool GeneratesTemporaryValues => false;

        public override string Next(EntityEntry entry)
        {
            string name = $"Player_{DateTime.Now.ToString("yyyyMMdd")}";
            return name;
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
