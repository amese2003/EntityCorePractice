using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MMO_EFCore
{
    // Relationship Configuration

    // a) Convention (관래)
    // - 각종 형식과 이름 등을 정해진 규칙에 맞게 만들면, EF Core에서 알아서 처리
    // - 쉽고 빠른데, 모든 경우를 처리하진 못함
    // B) Data Annotation (데이터 주석)
    // - class / property 등에 Attribute를 붙여 추가 정보
    // c) Fluent Api (직접 정리)
    // - OnModelCreating에서 직접 설정을 정의해서 만드는 '귀찮은' 방식
    // - 대신 활용 범위는 가장 넓음


    // Convention을 이용한 PK 설정
    // 1) <PrincipalKeyName>                                    PlayerId
    // 2) <class>PrincipalKeyName>                              PlayerPlayerId
    // 3) <NavigationalPropertyName><PrincipalKeyName>          OwnerPlayerId OwnerId

    // FK와 nullable
    // 1) Required Relationship (Not-null)
    // 삭제할 때 OnDelete 인자를 Cascade 모드로 호출 -> Principal 삭제하면 Dependent도 삭제
    // 2) Optional Relationship (Nullable)
    // 삭제할 때 OnDelete 인자를 ClientSetNull 모드로 호출
    // -> Principal 삭제할 때 Dependent Tracking하고 있으면, FK를 null로 세팅
    // -> Principal 삭제할 때 Dependent Tracking하고 있지 않으면, Exception

    // Convention 방식으로 못하는 것들
    // 1) 복합 fk
    // 2) 다수의 Navigational Property가 같은 클래스를 참조할 때
    // 3) DB나 삭제 관련 커스터마이징 필요할 때

    // Data Annotation으로 Relationship 설정
    // [ForeignKey("Prop1")]
    // [InverseProperty]

    // Fluent Api로 Relationship 설정
    // .HasOne() .HasMany()
    // .WithOne() .WithMany()
    // .HasForeignKey() .IsRequired() .OnDelete()
    // .HasConstraintName() .HasPrinipalKey()

    // Entity 클래스 이름 = 테이블 이름 = Item
    [Table("Item")]
    public class Item
    {
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
