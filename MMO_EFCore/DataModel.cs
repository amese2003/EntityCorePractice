using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MMO_EFCore
{
    // Shadow Property + Backing Field

    // a) Convention (관래)
    // - 각종 형식과 이름 등을 정해진 규칙에 맞게 만들면, EF Core에서 알아서 처리
    // - 쉽고 빠른데, 모든 경우를 처리하진 못함
    // B) Data Annotation (데이터 주석)
    // - class / property 등에 Attribute를 붙여 추가 정보
    // c) Fluent Api (직접 정리)
    // - OnModelCreating에서 직접 설정을 정의해서 만드는 '귀찮은' 방식
    // - 대신 활용 범위는 가장 넓음

    // Shadow Property
    // class에는 있지만 Db에 없음 -> [NotMapped] .Ignore()
    // DB에는 있지만 Class에는 없는 것? -> Shadow Property
    // 생성 -> .Property<DateTime>("RecoveredDate")
    // Read/Write -> Property("RecoveredDate").CurrentValue

    // Backing Field (EF Core)
    // private Field DB에 매핑하고, public getter로 가공해서 사용
    // ex) DB에는 JSON 형태로 string을 저장하고, getter는 json을 가공해서 사용
    // 일반적으로 Fluent Api

    // Entity 클래스 이름 = 테이블 이름 = Item

    public struct ItemOption
    {
        public int str;
        public int dex;
        public int hp;
    }

    [Table("Item")]
    public class Item
    {
        private string _jsonData;
        public string JsonData { 
            get { return _jsonData; }
        }

        public void SetOption(ItemOption option)
        {
            _jsonData = JsonConvert.SerializeObject(option);
        }

        public ItemOption GetOption()
        {
            return JsonConvert.DeserializeObject<ItemOption>(_jsonData);
        }

        public bool SoftDeleted { get; set; }

        // 이름 ->  Primary Key
        public int ItemId { get; set; }
        public int TemplateId { get; set; } // 101 = 집행검 (...)
        public DateTime CreateDate { get; set; }

        // 다른 크래스 참조 -> FK (Navigational Property)
        public int TestOwnerId { get; set; }        
        public Player Owner { get; set; }

        public int? TestCreatorId { get; set; }
        public Player Creator { get; set; }

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

        [InverseProperty("Owner")]
        public Item OwnedItem { get; set; } // 1 : 다
        [InverseProperty("Creator")]
        public ICollection<Item> CreatedItems { get; set; }
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
