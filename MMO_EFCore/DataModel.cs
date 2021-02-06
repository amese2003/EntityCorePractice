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
    // DbContext 심화 과정 (최적화? 등등)
    // 1) ChangeTracker
    // - Tracking State 관련
    // 2) Database
    // - Transection
    // - DB Creation/Migration
    // - Raw SqL
    // 3) Model
    // - DB 모델링 관련

    // State
    // 1) Detached (No Tracking : 추적되지 않은 상태. SaveChange를 하더라도 모름)
    // 2) Unchanged (DB에 있고, 딱히 수정사항도 없음. SaveChanges를 해도 아무것도 변하지 않음)
    // 3) Deleted (DB에는 아직 있지만, 삭제되어야함. SaveChanged로 DB에 적용)
    // 4) Modifeid (DB에 있고, 클라에서 수정된 상태. SaveChanges로 DB에 적용)
    // 5) Added (DB에는 아직 없음. SaveChanges로 db에 적용)

    // State 체크 방법
    // - Entry().State
    // - Entry().Property().IsModified
    // - Entry().Navigation().IsModified

    // State가 대부분 '직관적'이지만 Relationship이 개입하면 살짝 더 복잡함.
    // - 1) Add / AddRange 사용할 때의 상태 변화
    // -- NotTracking 상태라면 Added
    // -- Tracking 상태인데, FK 설정이 필요한지 따라 Modified / 기존 상태 유지
    // - 2) Remove/RemoveRange 사용할 때의 상태 변화
    // - (DB에 의해 생선된 Key) && (C# 기본값 아님) -> 필요에 따라 Unchanged / Modified / Delete
    // - (DB에 의해 생성된 Key 없음) || C# 기본값 -> Added

    [Table("Item")]
    public class Item
    {
        public bool SoftDeleted { get; set; }

        // 이름 ->  Primary Key
        public int ItemId { get; set; }
        public int TemplateId { get; set; } // 101 = 집행검 (...)
        public DateTime CreateDate { get; private set; }

        public int ItemGrade { get; set; }

        // 다른 클래스 참조 -> FK (Navigational Property)
        public int OwnerId { get; set; }        
        public Player Owner { get; set; }

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
